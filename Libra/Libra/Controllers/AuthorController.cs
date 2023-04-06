using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Azure;
using Libra.DATA;
using Libra.Models;
using Libra.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Libra.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class AuthorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private ApiResponse _response;
        
        public AuthorsController(ApplicationDbContext context)
        {
            _context = context;
            _response = new ApiResponse();
        }
        private bool AuthorExists(int id)
        {
            return _context.Author.Any(e => e.Id == id);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Author>>> GetAuthors() 
        {
            _response.Result = await _context.Author
                .Include(a => a.Book)
                .ToListAsync();
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);

        }

        [HttpGet("{id:int}", Name = "GetAuthor")]
        public async Task<ActionResult<Author>> GetAuthor(int id)
        {
            var author = await _context.Author
                .Include(a => a.Book)
                .Where(a => a.Id == id)
                .Select(a => new Author
                {
                    Id = a.Id,
                    Name = a.Name,
                    Bio = a.Bio,
                    Book = a.Book.Select(b => new Books
                    {
                        Id = b.Id,
                        Name = b.Name,
                        AuthorId = b.AuthorId
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (author == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                return NotFound(_response);
            }


            _response.Result = _context.Author;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(author);
        }
        [HttpGet("GetBooksByAuthor")]
        public async Task<ActionResult<IEnumerable<AuthorUpdateDTO>>> GetBooksByAuthor()
        {
            var booksByAuthor = await _context.Author
                .Select(a => new AuthorUpdateDTO
                {
                   Id = a.Id,
                   Name = a.Name,
                   Bio = a.Bio,
                   CreatedBy = a.CreatedBy,
                   BookCount = a.Book.Count()
                })
                .OrderByDescending(a => a.BookCount)
                .ToListAsync();

            _response.Result = booksByAuthor;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(booksByAuthor);
        }

        
        [HttpPost]
        public async Task<ActionResult<Author>> CreateAuthor([FromForm] AuthorCreateDTO authorDTO)
        {
            // check if the input data is valid
            if (!ModelState.IsValid)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(ModelState);
            }

            try
            {
                // create a new Author object from the DTO and set the CreatedBy property
                var author = new Author
                {
                    Name = authorDTO.Name,
                    Bio = authorDTO.Bio,
                    CreatedBy = authorDTO.CreatedBy,
                    CreatedAt = DateTime.Now.ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'"),
            };
                // add the books to the author's collection
                foreach (var bookDTO in author.Book)
                {
                    var book = new Books
                    {
                        Name = bookDTO.Name,
                        Description = bookDTO.Description
                    };
                    author.Book.Add(book);
                }

                // add the author to the context and save changes
                _context.Author.Add(author);
                await _context.SaveChangesAsync();

                // return the created author
                var result = CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author);
                _response.Result = result.Value;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(result);

            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(ex.Message);
            }
        }

        
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAuthor(int id, [FromForm] AuthorUpdateDTO authorDTO)
        {
           
            var author = await _context.Author
                .Include(a => a.Book)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                return NotFound(_response);
            }

            //  updated fields
            author.Name = authorDTO.Name;
            author.Bio = authorDTO.Bio;
            author.CreatedBy = author.CreatedBy;
            author.CreatedAt = DateTime.Now.ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'");


            // Deletes books that are not in the updated list
            var bookIds = author.Book.Select(b => b.Id).ToList();
            var booksToDelete = author.Book.Where(b => !bookIds.Contains(b.Id)).ToList();
            foreach (var book in booksToDelete)
            {
                _context.Remove(book);
            }

            foreach (var bookDTO in author.Book)
            {
                var book = author.Book.FirstOrDefault(b => b.Id == bookDTO.Id);
                if (book != null)
                {
                    book.Name = bookDTO.Name;
                    book.Description = bookDTO.Description;
                    book.CreatedBy = bookDTO.CreatedBy;
                    book.CreatedAt = DateTime.Now.ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'");
                }
            }

            foreach (var bookDTO in author.Book)
            {
                var book = new Books
                {
                    Name = bookDTO.Name,
                    Description = bookDTO.Description,
                    Author = author,
                    CreatedBy = bookDTO.CreatedBy,
                    CreatedAt = DateTime.Now.ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'"),
                };
                author.Book.Add(book);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorExists(id))
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    return NotFound(_response);
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteAuthor(int id)
        {
            // Find the author to delete
            var author = await _context.Author.FindAsync(id);
            if (author == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                return NotFound(_response);
            }

            try
            {
                _context.Author.Remove(author); // Remove the author from the context
                await _context.SaveChangesAsync(); 
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response); 
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);          
            }
        }

    }
}

