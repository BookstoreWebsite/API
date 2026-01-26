using Bookstore.Domain.IRepositories;
using Bookstore.Domain.Model;
using Bookstore.Infrastructure.Data;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;

namespace Bookstore.Infrastructure.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly EmailSettings _emailSettings;

        public BookRepository(AppDbContext context,
                              IUserRepository userRepository,
                              IHttpClientFactory httpClientFactory,
                              IOptions<EmailSettings> emailOptions) 
        {
            _context = context;
            _userRepository = userRepository;
            _httpClientFactory = httpClientFactory;
            _emailSettings = emailOptions.Value;
        }

        public async Task CreateAsync(Book book, List<Guid> genreIds, decimal? price)
        {
            if (book != null)
            {
                var genres = await _context.Genres
                    .Where(g => genreIds.Contains(g.Id))
                    .ToListAsync();

                foreach (var genre in genres)
                {
                    book.Genres.Add(genre);
                }

                _context.Books.Add(book);

                var priceListEntry = new PriceListEntry
                {
                    ProductId = book.Id,
                    ValidFrom = DateTime.UtcNow,
                    Price = (decimal)price
                };

                _context.PriceListEntries.Add(priceListEntry);
            }
            await _context.SaveChangesAsync();
        }



        public async Task DeleteAsync(Guid id)
        {
            var book = await GetByIdAsync(id);
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Book>> GetAllAsync()
        {
            return await _context.Books.ToListAsync(); 
        }

        public async Task<Book> GetByIdAsync(Guid id)
        {
            return await _context.Books
                                 .Include(b => b.Genres)
                                 .Include(b => b.Subscribers)
                                 .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task UpdateAsync(Book book, List<Guid> genreIds, bool isBackInStock)
        {
            var genres = await _context.Genres
                    .Where(g => genreIds.Contains(g.Id))
                    .ToListAsync();

            book.Genres.Clear();
            foreach (var genre in genres)
            {
                book.Genres.Add(genre);
            }

            if(isBackInStock)
                await NotifySubscribersAsync(book);

            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Book>> GetAllGenreBooksAsync(Guid genreId) 
        {
            return await _context.Books
                .Include(b => b.Reviews)
                .Where(b => b.Genres.Any(g => g.Id == genreId))
                .ToListAsync();
        }

        public async Task CreateReviewAsync(Review review)
        {
            if (review == null) throw new ArgumentNullException(nameof(review));

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            var avg = await _context.Reviews
                .Where(r => r.BookId == review.BookId)
                .AverageAsync(r => (double?)r.Rating);

            var book = await _context.Books.FirstAsync(b => b.Id == review.BookId);
            book.Rating = avg;
            await _context.SaveChangesAsync();
        }


        public async Task<List<Review>> GetAllBookReviewsAsync(Guid bookId)
        {
            return await _context.Reviews
                .Include(r => r.Reader)
                .Where(r => r.BookId == bookId)
                .ToListAsync();
        }

        public async Task<Review> GetReviewByIdAsync(Guid reviewId)
        {
            return await _context.Reviews
                                 .Include(r => r.Reader)
                                 .FirstOrDefaultAsync(r => r.Id == reviewId);
        }

        public async Task CreateCommentAsync(Comment comment) 
        {
            if (comment != null)
            {
                _context.Comments.Add(comment);

                if (comment.ParentCommentId != null)
                {
                    var parentComment = await GetCommentById((Guid)comment.ParentCommentId);
                    parentComment.HasReplies = true;
                    _context.Comments.Update(parentComment);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<Comment> GetCommentById(Guid id) 
        {
            return await _context.Comments.FindAsync(id);
        }

        public async Task<List<Comment>> GetAllReviewCommentsAsync(Guid reviewId) 
        {
            var comments = await _context.Comments.
                Where(c => c.ReviewId == reviewId && c.ParentCommentId == null).
                Include(c => c.Reader).
                ToListAsync();
            return comments;
        }

        public async Task<List<Comment>> GetAllCommentRepliesAsync(Guid parentCommentId) 
        {
            var comments = await _context.Comments.
                Where(c => c.ParentCommentId == parentCommentId).
                Include(c => c.Reader).
                ToListAsync();
            return comments;
        }

        public async Task CreateReportAsync(Report report) 
        {
            if(report!= null)
                _context.Reports.Add(report);

            await _context.SaveChangesAsync();
        }

        public async Task<List<Report>> GetAllReportsAsync() 
        {
            var reports = await _context.Reports
                                        .Include(r => r.Review)
                                        .Include(r => r.Comment)
                                        .ToListAsync();
            return reports;
        }

        public async Task AddToWishedAsync(Guid readerId, Guid bookId) 
        {
            var book = await GetByIdAsync(bookId);
            var user = await _userRepository.GetByIdAsync(readerId);

            user.Wished.Add(book);
            await _context.SaveChangesAsync(); 
        }

        public async Task AddToReadAsync(Guid readerId, Guid bookId)
        {
            var book = await GetByIdAsync(bookId);
            var user = await _userRepository.GetByIdAsync(readerId);

            user.Read.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromReadAsync(Guid readerId, Guid bookId)
        {
            var book = await GetByIdAsync(bookId);
            var user = await _userRepository.GetByIdAsync(readerId);

            user.Read.Remove(book);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromWishedAsync(Guid readerId, Guid bookId)
        {
            var book = await GetByIdAsync(bookId);
            var user = await _userRepository.GetByIdAsync(readerId);

            user.Wished.Remove(book);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Book>> GetAllWishedAsync(Guid readerId)
        {
            var user = await _context.Users
                .Include(u => u.Wished)
                .FirstOrDefaultAsync(u => u.Id == readerId);

            if (user == null)
                return new List<Book>();

            return user.Wished.ToList();
        }

        public async Task<List<Book>> GetAllReadAsync(Guid readerId)
        {
            var user = await _context.Users
                .Include(u => u.Read)
                .FirstOrDefaultAsync(u => u.Id == readerId);

            if (user == null)
                return new List<Book>();

            return user.Read.ToList();
        }

        public async Task<List<Book>> GetAllSubscriptionsAsync(Guid readerId)
        {
            var user = await _context.Users
                .Include(u => u.Subscriptions)
                .FirstOrDefaultAsync(u => u.Id == readerId);

            if (user == null)
                return new List<Book>();

            return user.Subscriptions.ToList();
        }

        public async Task<List<Book>> GetRecommendedBooksAsync(Guid readerId) 
        {
            var user = await _userRepository.GetByIdAsync(readerId);
            var favoriteGenreBooks = new List<Book>();

            foreach (var genre in user.FavoriteGenres) 
            {
                var genreBooks = await GetAllGenreBooksAsync(genre.Id);
                favoriteGenreBooks.AddRange(genreBooks);
            }

            var recommendedBooks = favoriteGenreBooks
                .Where(b => b.Rating != null && b.Reviews.Count != 0)
                .GroupBy(b => b.Id).Select(g => g.First())
                .OrderByDescending(b => b.Rating)
                .ThenByDescending(b => b.Reviews.Count)
                .Take(5)
                .ToList();


            return recommendedBooks;

        }

        public async Task SubscribeAsync(Guid readerId, Guid bookId) 
        {
            var book = await GetByIdAsync(bookId);
            var user = await _userRepository.GetByIdAsync(readerId);

            user.Subscriptions.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task UnsubscribeAsync(Guid readerId, Guid bookId) 
        {
            var book = await GetByIdAsync(bookId);
            var user = await _userRepository.GetByIdAsync(readerId);

            user.Subscriptions.Remove(book);
            await _context.SaveChangesAsync();
        }

        private async Task NotifySubscribersAsync(Book book) 
        {
            foreach(var subscriber in book.Subscribers) 
            {
                await SendBackInStockEmailAsync(book.Id, book, subscriber);
            }
        }

        private async Task SendBackInStockEmailAsync(Guid bookId, Book book, User reader)
        {
            if (book == null)
                throw new InvalidOperationException($"Book {bookId} not found.");

            var subject = $"{book.Name} subscription";
            var bodyText = $"We are pleased to inform you that {book.Name} is back in stock!\nBuy it while it's here!";

            await SendEmailAsync(
                toEmail: reader.Email,
                toName: $"{reader.FirstName} {reader.LastName}",
                subject: subject,
                bodyText: bodyText
            );
        }

        private async Task SendEmailAsync(string toEmail, string toName, string subject, string bodyText)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = subject;

            message.Body = new TextPart(TextFormat.Plain) { Text = bodyText };

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailSettings.SmtpUser, _emailSettings.SmtpPass);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }
    }
}
