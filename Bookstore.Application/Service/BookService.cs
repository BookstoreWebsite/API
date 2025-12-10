using Bookstore.Application.DTO;
using Bookstore.Application.IService;
using Bookstore.Domain.IRepositories;
using Bookstore.Domain.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Application.Service
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _repository;

        public BookService(IBookRepository repository) 
        {
            _repository = repository;   
        }

        public async Task<bool> CreateAsync(BookDto bookDto)
        {
            var newBook = new Book()
            {
                Title = bookDto.Title,
                Description = bookDto.Description,
                ImageUrl = bookDto.ImageUrl,
                Author = bookDto.Author,
                Rating = null
            };

            await _repository.CreateAsync(newBook);
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
                var bookDto = new BookDto
                {
                    Title = book.Title,
                    Description = book.Description,
                    ImageUrl = book.ImageUrl,
                    Author = book.Author,
                    Rating = book.Rating
                };

                bookDtos.Add(bookDto);
            }
            return bookDtos;
        }

        public async Task<BookDto> GetByIdAsync(Guid id)
        {
            var book = await _repository.GetByIdAsync(id);

            var bookDto = new BookDto
            {
                Title = book.Title,
                Description = book.Description,
                ImageUrl = book.ImageUrl,
                Author = book.Author,
                Rating = book.Rating
            };

            return bookDto;
        }

        public async Task<bool> UpdateAsync(Guid id, BookDto bookDto)
        {
            var oldBook = await _repository.GetByIdAsync(id);

            oldBook.Title = bookDto.Title;
            oldBook.Description = bookDto.Description;
            oldBook.Author = bookDto.Author;
            oldBook.ImageUrl = bookDto.ImageUrl;
            oldBook.Rating = bookDto.Rating;

            await _repository.UpdateAsync(oldBook);
            return true;
        }
    }
}
