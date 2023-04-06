using Libra.Models.DTO;
using Microsoft.Exchange.WebServices.Data;

namespace Libra.Services.CategoryBooksService
{
    public interface ICategoryBooksService
    {
        
            Task<ServiceResponse<List<Book>>> GetAllBooks();
            Task<ServiceResponse<Book>> GetBookById(int id);
            Task<ServiceResponse<List<Book>>> AddBook(BooksDTO newBook);
            Task<ServiceResponse<Book>> UpdateBook(int id, BooksDTO updatedBook);
            Task<ServiceResponse<List<Book>>> DeleteBook(int id);
            Task<ServiceResponse<List<Category>>> GetAllCategories();
            Task<ServiceResponse<Category>> GetCategoryById(int id);
            Task<ServiceResponse<List<Category>>> AddCategory(CategoryDTO newCategory);
            Task<ServiceResponse<Category>> UpdateCategory(int id, CategoryDTO updatedCategory);
            Task<ServiceResponse<List<Category>>> DeleteCategory(int id);
            Task<ServiceResponse<CategoryBooks>> AddBookToCategory(CategoryBooksDTO categoryBook);
            Task<ServiceResponse<CategoryBooks>> RemoveBookFromCategory(CategoryBooksDTO categoryBook);
       

    }
}
