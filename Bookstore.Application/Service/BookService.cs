using Bookstore.Application.DTO;
using Bookstore.Application.IService;
using Bookstore.Domain.IRepositories;
using Bookstore.Domain.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace Bookstore.Application.Service
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _repository;
        private readonly IGenreRepository _genreRepository;
        private readonly IProductRepository _productRepository;

        public BookService(IBookRepository repository, IGenreRepository genreRepository, IProductRepository productRepository) 
        {
            _repository = repository;   
            _genreRepository = genreRepository;
            _productRepository = productRepository;
        }

        public async Task<bool> CreateAsync(BookDto bookDto, List<Guid> genreIds)
        {
            var newBook = new Book()
            {
                Name = bookDto.Name,
                Description = bookDto.Description,
                ImageUrl = bookDto.ImageUrl,
                Author = bookDto.Author,
                Rating = null,
            };

            await _repository.CreateAsync(newBook, genreIds, bookDto.Price);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
            return true;
        }

        public async Task<List<BookDto>> GetAllAsync()
        {
            var books = await _repository.GetAllAsync();

            var bookDtos = new List<BookDto>();

            foreach (var book in books)
            {
                var priceListEntry = await _productRepository.GetCurrentPriceListEntryAsync(book.Id);

                var bookDto = new BookDto
                {
                    Id = book.Id,
                    Name = book.Name,
                    Description = book.Description,
                    ImageUrl = book.ImageUrl,
                    Author = book.Author,
                    Rating = book.Rating,
                    Price = priceListEntry.Price,
                };

                bookDtos.Add(bookDto);
            }
            return bookDtos;
        }

        public async Task<BookDto> GetByIdAsync(Guid id)
        {
            var book = await _repository.GetByIdAsync(id);
            var priceListEntry = await _productRepository.GetCurrentPriceListEntryAsync(book.Id);

            var bookDto = new BookDto
            {
                Id = book.Id,
                Name = book.Name,
                Description = book.Description,
                ImageUrl = book.ImageUrl,
                Author = book.Author,
                Rating = book.Rating,
                Price = priceListEntry.Price
            };

            return bookDto;
        }

        public async Task<bool> UpdateAsync(Guid id, BookDto bookDto)
        {
            var oldBook = await _repository.GetByIdAsync(id);

            oldBook.Name = bookDto.Name;
            oldBook.Description = bookDto.Description;
            oldBook.Author = bookDto.Author;
            oldBook.ImageUrl = bookDto.ImageUrl;
            oldBook.Rating = bookDto.Rating;

            if(bookDto.Price != null) 
            {
                await _productRepository.AddNewPriceListEntryAsync(id, (decimal)bookDto.Price);
            }

            await _repository.UpdateAsync(oldBook);
            return true;
        }

        public async Task<List<BookDto>> GetAllGenreBooksAsync(Guid id) 
        {
            var books = await _repository.GetAllGenreBooksAsync(id);
            var bookDtos = new List<BookDto>();

            foreach (var book in books)
            {
                var priceListEntry = await _productRepository.GetCurrentPriceListEntryAsync(book.Id);
                var bookDto = new BookDto
                {
                    Id = book.Id,
                    Name = book.Name,
                    Description = book.Description,
                    ImageUrl = book.ImageUrl,
                    Author = book.Author,
                    Rating = book.Rating,
                    Price = priceListEntry.Price
                };

                bookDtos.Add(bookDto);
            }
            return bookDtos;
        }

        public async Task<List<ReviewDto>> GetAllBookReviewsAsync(Guid bookId)
        {
            var reviews = await _repository.GetAllBookReviewsAsync(bookId);
            List<ReviewDto> reviewDtos= new List<ReviewDto>();
            foreach(var review in reviews) 
            {
                var reviewDto = new ReviewDto
                {
                    Id = review.Id,
                    Title = review.Title,
                    Text = review.Text,
                    Rating = review.Rating
                };
                reviewDtos.Add(reviewDto);
            }
            return reviewDtos;
        }

        public async Task<ReviewDto> GetReviewByIdAsync(Guid reviewId)
        {
            var review = await _repository.GetReviewByIdAsync(reviewId);
            var reviewDto = new ReviewDto()
            {
                Id = review.Id,
                Title = review.Title,
                Text = review.Text,
                Rating = review.Rating
            };
            return reviewDto;
        }

        public async Task<bool> CreateReviewAsync(ReviewDto reviewDto, Guid bookId, Guid readerId)
        {
            var review = new Review
            {
                Title = reviewDto.Title,
                Text = reviewDto.Text,
                Rating = reviewDto.Rating,
                BookId = bookId,
                ReaderId = readerId
            };

            await _repository.CreateReviewAsync(review);
            return true;
        }

        public async Task<bool> CreateCommentAsync(CommentDto commentDto, Guid readerId, Guid reviewId, Guid bookId, Guid? parentCommentId = null)
        {
            var comment = new Comment
            {
                Text = commentDto.Text,
                ReaderId = readerId,
                ReviewId = reviewId,
                BookId = bookId,
                ParentCommentId = parentCommentId,
                HasReplies = false
            };
            await _repository.CreateCommentAsync(comment);
            return true;
        }

        public async Task<List<CommentDto>> GetAllReviewCommentsAsync(Guid reviewId) 
        {
            var comments = await _repository.GetAllReviewCommentsAsync(reviewId);
            List<CommentDto> commentDtos = new List<CommentDto>();

            foreach (var comment in comments) 
            {
                var commentDto = new CommentDto
                {
                    Id = comment.Id,
                    Text = comment.Text,
                    Username = comment.Reader.Username,
                    ProfilePicture = comment.Reader.ProfilePictureUrl,
                    HasReplies = comment.HasReplies
                };
                commentDtos.Add(commentDto);
            }

            return commentDtos;
        }

        public async Task<List<CommentDto>> GetAllCommentRepliesAsync(Guid parentCommentId)
        {
            var comments = await _repository.GetAllCommentRepliesAsync(parentCommentId);
            List<CommentDto> commentDtos = new List<CommentDto>();

            foreach (var comment in comments)
            {
                var commentDto = new CommentDto
                {
                    Id = comment.Id,
                    Text = comment.Text,
                    Username = comment.Reader.Username,
                    ProfilePicture = comment.Reader.ProfilePictureUrl,
                    HasReplies = comment.HasReplies
                };
                commentDtos.Add(commentDto);
            }

            return commentDtos;
        }

        public async Task<bool> CreateReportAsync(ReportDto reportDto, Guid readerId, Guid parentId)
        {
            var report = new Report();

            if ((bool)reportDto.IsReviewReport)
            {
                report = new Report
                {
                    Text = reportDto.Text,
                    Reason = reportDto.Reason,
                    ReaderId = readerId,
                    ReviewId = parentId
                };
            }
            else 
            {
                report = new Report
                {
                    Text = reportDto.Text,
                    Reason = reportDto.Reason,
                    ReaderId = readerId,
                    CommentId = parentId
                };
            }

            await _repository.CreateReportAsync(report);
            return true;
        }

        public async Task<List<ReportDto>> GetAllReportsAsync()
        {
            var reports = await _repository.GetAllReportsAsync();
            List<ReportDto> reportDtos = new List<ReportDto>();

            foreach(var report in reports) 
            {
                var reportDto = new ReportDto
                {
                    Id = report.Id,
                    Text = report.Text,
                    Reason = report.Reason,
                    ReaderId = report.ReaderId,
                    ReviewId = report.ReviewId,
                    CommentId = report.CommentId,
                };
                reportDtos.Add(reportDto);
            }

            return reportDtos;
        }

        public async Task<bool> AddToWishedAsync(Guid readerId, Guid bookId) 
        {
            await _repository.AddToWishedAsync(readerId, bookId);
            return true;
        }

        public async Task<bool> AddToReadAsync(Guid readerId, Guid bookId)
        {
            await _repository.AddToReadAsync(readerId, bookId);
            return true;
        }

        public async Task<List<BookDto>> GetAllWishedAsync(Guid readerId) 
        {
            var wished = await _repository.GetAllWishedAsync(readerId);

            var bookDtos = new List<BookDto>();

            foreach (var book in wished)
            {
                var priceListEntry = await _productRepository.GetCurrentPriceListEntryAsync(book.Id);

                var bookDto = new BookDto
                {
                    Id = book.Id,
                    Name = book.Name,
                    Description = book.Description,
                    ImageUrl = book.ImageUrl,
                    Author = book.Author,
                    Rating = book.Rating,
                    Price = priceListEntry.Price,
                };

                bookDtos.Add(bookDto);
            }
            return bookDtos;
        }

        public async Task<List<BookDto>> GetAllReadAsync(Guid readerId)
        {
            var read = await _repository.GetAllReadAsync(readerId);

            var bookDtos = new List<BookDto>();

            foreach (var book in read)
            {
                var priceListEntry = await _productRepository.GetCurrentPriceListEntryAsync(book.Id);

                var bookDto = new BookDto
                {
                    Id = book.Id,
                    Name = book.Name,
                    Description = book.Description,
                    ImageUrl = book.ImageUrl,
                    Author = book.Author,
                    Rating = book.Rating,
                    Price = priceListEntry.Price,
                };

                bookDtos.Add(bookDto);
            }
            return bookDtos;
        }

    }
}
