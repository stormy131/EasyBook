using EasyBook.Models;
using EasyBook.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace EasyBook.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase {
        private readonly EasyBookContext _db;

        public BooksController(EasyBookContext context){
            _db = context;
        }

        [HttpGet]
        public IQueryable<BookItemDTO> GetAllBooks(){
            return _db.BookItems.Select(b => (BookItemDTO)b);
        }

        [HttpGet("{book_id}")]
        [IdFilterAsync<BookItem>]
        public ActionResult<BookItem> GetBook(long book_id){
            var book = _db.BookItems.Include("Reviews").First(b => b.Id == book_id);

            if (book is null){
                return NotFound();
            }

            return book;
        }

        [HttpPost]
        public async Task<ActionResult> AddBook(BookItemDTO book_data){
            _db.BookItems.Add(book_data);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(AddBook), book_data);
        }

        [IdFilterAsync<BookItem>]
        [HttpPut("{book_id}")]
        public async Task<ActionResult<BookItem>> PutBook(long book_id, BookItemDTO new_data){
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
        [IdFilterAsync<BookItem>]
        public async Task<ActionResult> DeleteBook(long book_id){
            var book_item = await _db.BookItems.FindAsync(book_id);

            if(book_item is null){
                return NotFound();
            }

            _db.BookItems.Remove(book_item);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("Review/{book_id}")]
        public async Task<ActionResult> PostReview(long book_id, ReviewDTO review_data){
            if(review_data.BookItemId != book_id){
                return BadRequest("Target book item IDs do not correspond to each other");
            }

            _db.Reviews.Add(review_data);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(PostReview), review_data);           
        }

        [HttpDelete("Review/{review_id}")]
        [IdFilterAsync<ReviewItem>]
        public async Task<ActionResult> DeleteReview(long review_id){
            var review_item = (await _db.Reviews.FindAsync(review_id))!;
            _db.Remove(review_item);
            
            await _db.SaveChangesAsync();
            return NoContent();
        }

        private bool ItemExists(long book_id){
            return _db.BookItems.Any(e => e.Id == book_id);
        }
    } 
}