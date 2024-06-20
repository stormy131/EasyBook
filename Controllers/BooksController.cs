using EasyBook.Models;
using EasyBook.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyBook.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase {
        private readonly EasyBookContext _db;

        public BooksController(EasyBookContext context){
            _db = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookItem>>> GetAllBooks(){
            return await _db.BookItems.ToListAsync();
        }

        [HttpGet("{book_id}")]
        [IdFilterAsync]
        public async Task<ActionResult<BookItem>> GetBook(long book_id){
            var book = await _db.BookItems.FindAsync(book_id);

            if (book is null){
                return NotFound();
            }

            return book;
        }

        [HttpPost]
        public async Task<ActionResult> AddBook(BookItem book_data){
            _db.BookItems.Add(book_data);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(AddBook), book_data);
        }

        [IdFilterAsync]
        [HttpPut("{book_id}")]
        public async Task<ActionResult<BookItem>> PutBook(long book_id, BookItem new_data){
            if(book_id != new_data.Id){
                return BadRequest();
            }

            _db.Entry(new_data).State = EntityState.Modified;

            try {
                await _db.SaveChangesAsync(); 
            } catch (DbUpdateConcurrencyException){
                if(!ItemExists(book_id)){
                    return NotFound();
                } else throw;
            }

            return NoContent();
        }

        [HttpDelete("{book_id}")]
        [IdFilterAsync]
        public async Task<ActionResult> DeleteBook(long book_id){
            var book_item = await _db.BookItems.FindAsync(book_id);

            if(book_item is null){
                return NotFound();
            }

            _db.BookItems.Remove(book_item);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        private bool ItemExists(long book_id){
            return _db.BookItems.Any(e => e.Id == book_id);
        }
    } 
}