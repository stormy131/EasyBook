using EasyBook.Models;
using EasyBook.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using EasyBook.Identity;

namespace EasyBook.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase {
        private readonly EasyBookContext _db;

        public BooksController(EasyBookContext context){
            _db = context;
        }

        [HttpGet]
        [Authorize]
        public IQueryable<BookItemDTO> GetAllBooks(){
            return _db.BookItems.Select(b => (BookItemDTO) b);
        }

        [HttpGet("{book_id}")]
        [ExistanceFilterAsync<BookItem>]
        [Authorize]
        public ActionResult<BookItem> GetBook(long book_id){
            var book = _db.BookItems.Include("Reviews").First(b => b.Id == book_id);
            return book;
        }

        [HttpPost]
        [Authorize( Policy = IdentityData.AdminUserPolicy )]
        public async Task<ActionResult> AddBook(BookItemDTO book_data){
            _db.BookItems.Add(book_data);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(AddBook), book_data);
        }

        [HttpPut("{book_id}")]
        [ExistanceFilterAsync<BookItem>]
        [Authorize(Policy = IdentityData.AdminUserPolicy)]
        public async Task<ActionResult<BookItem>> PutBook(long book_id, BookItemDTO new_data){
            if(book_id != new_data.Id){
                return BadRequest();
            }

            _db.Entry((BookItem) new_data).State = EntityState.Modified;

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
        [ExistanceFilterAsync<BookItem>]
        [Authorize(Policy = IdentityData.AdminUserPolicy)]
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
        [ExistanceFilterAsync<BookItem>]
        [Authorize]
        public async Task<ActionResult> PostReview(long book_id, ReviewDTO review_data){
            var issuer_id = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id")!;
            if(review_data.UserId != Convert.ToInt64(issuer_id.Value)){
                return BadRequest("Mismatch in payload data");
            }

            var review = (ReviewItem) review_data;
            review.BookItemId = book_id;

            _db.Reviews.Add(review);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(PostReview), review_data);           
        }

        [HttpDelete("Review/{review_id}")]
        [ExistanceFilterAsync<ReviewItem>, OwnershipFilterAsync<ReviewItem>]
        [Authorize]
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