using Libra.DATA;
using Libra.Models;
using Libra.Models.DTO;
using Libra.Services;
using Libra.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;
using static System.Net.Mime.MediaTypeNames;

namespace Libra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private ApiResponse _response;
        private readonly IBlobService _blobService;

        public BooksController(ApplicationDbContext context, IBlobService blobService)
        {
            _context = context;
            _response = new ApiResponse();
            _blobService = blobService;
        }

        [HttpGet]
        //public async Task<ActionResult<IEnumerable<Books>>> GetBooks()
        //{
        //    try
        //    {
        //        var books = await _context.Books
        //            .Include(b => b.CategoryBooks)
        //                .ThenInclude(cb => cb.Category)
        //            .ToListAsync();

        //        _response.Result = books;
        //        _response.StatusCode = HttpStatusCode.OK;
        //        return Ok(books);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving books: {ex.Message}");
        //    }
        //}
        public async Task<IActionResult> GetBooks()
        {
            var books = await _context.Books
                .Include(b => b.CategoryBooks)
                .ThenInclude(c => c.Category)
                .ToListAsync();
            _response.Result = books;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);

        }
        [HttpGet("books/{id}/categories")]
        public async Task<ActionResult<IEnumerable<Category>>> GetBookCategories(int id)
        {
            var book = await _context.Books
                .Include(b => b.CategoryBooks)
                .ThenInclude(bc => bc.Category)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                return NotFound(_response);
            }

            var categories = book.CategoryBooks.Select(bc => bc.Category).ToList();

            _response.IsSuccess = categories.Any();
            _response.IsSuccess = true;
            _response.Result = _context.CategoryBooks;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(categories);

        }


        [HttpGet("{id:int}", Name = "GetBook")]
        public async Task<ActionResult<Books>> GetBook(int id)
        {
            try
            {
                var book = await _context.Books
                    .Include(b => b.CategoryBooks)
                        .ThenInclude(cb => cb.Category)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (book == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    return NotFound(_response);
                }

                return book;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
       
        [HttpPost]
        public async Task<ActionResult<Books>> PostBook([FromForm] BooksDTO bookUpdateDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest(ModelState);
                }

                var author = await _context.Author.FindAsync(bookUpdateDTO.AuthorId);
                if (author == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest("Invalid author ID");
                }

                var categories = await _context.Category
                    .Where(c => bookUpdateDTO.CategoryId.Contains(c.Id))
                    .ToListAsync();
                if (categories.Count != bookUpdateDTO.CategoryId.Length)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest("Invalid category ID(s)");
                }

                using (var stream = new MemoryStream())
                {
                    await bookUpdateDTO.File.CopyToAsync(stream);
                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(bookUpdateDTO.File.FileName)}";
                    var book = new Books
                    {
                        Name = bookUpdateDTO.Name,
                        Description = bookUpdateDTO.Description,
                        Author = author,
                        CategoryBooks = categories.Select(c => new CategoryBooks { Category = c }).ToList(),
                        Image = await _blobService.UploadBlob(fileName, SD.SD_Storage_Container, bookUpdateDTO.File),
                        CreatedAt = DateTime.Now.ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'"),
                        CreatedBy = bookUpdateDTO.CreatedBy,
             
                    };

                    _context.Books.Add(book);
                    await _context.SaveChangesAsync();

                    return CreatedAtRoute("GetBook", new { id = book.Id }, book);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error creating book: {ex.Message}");
            }
        }

       
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutBook(int id, [FromForm] BooksDTO bookUpdateDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest(ModelState);
                }

                var book = await _context.Books
                    .Include(b => b.CategoryBooks)
                    .FirstOrDefaultAsync(b => b.Id == id);
                if (book == null)
                {
                    return NotFound();
                }

                // Ensure that the user can only update their own books
                if (!User.IsInRole("Admin") && book.CreatedBy != bookUpdateDTO.CreatedBy)
                {
                    return Forbid();
                }

                var author = await _context.Author.FindAsync(bookUpdateDTO.AuthorId);
                if (author == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest("Invalid author ID");
                }

                var categories = await _context.Category
                    .Where(c => bookUpdateDTO.CategoryId.Contains(c.Id))
                    .ToListAsync();
                if (categories.Count != bookUpdateDTO.CategoryId.Length)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest("Invalid category ID(s)");
                }

                if (bookUpdateDTO.File != null)
                {
                    using (var stream = new MemoryStream())
                    {
                        await bookUpdateDTO.File.CopyToAsync(stream);
                        string fileName = $"{Guid.NewGuid()}{Path.GetExtension(bookUpdateDTO.File.FileName)}";
                        book.Image = await _blobService.UploadBlob(fileName, SD.SD_Storage_Container, bookUpdateDTO.File);
                    }
                }

                book.Name = bookUpdateDTO.Name;
                book.Description = bookUpdateDTO.Description;
                book.Author = author;
                book.CategoryBooks = categories.Select(c => new CategoryBooks { Category = c }).ToList();

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating book: {ex.Message}");
            }
        }

      
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse>> DeleteBook(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest();
                }

                var book = await _context.Books
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (book == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest();
                }

                if (!string.IsNullOrEmpty(book.Image))
                {
                    await _blobService.DeleteBlob(book.Image.Split('/').Last(), SD.SD_Storage_Container);
                }

                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
                _response.StatusCode = HttpStatusCode.NoContent;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _response;
        }


    }

}

