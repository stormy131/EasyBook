using EasyBook.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using EasyBook.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyBook.Controllers{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase{
        private readonly EasyBookContext _db;

        public UsersController(EasyBookContext context) {
            _db = context;
        }

        [HttpGet]
        public IQueryable<UserDTO> GetAllUsers(){
            var users = _db.Users.Select(u => (UserDTO)u);
            return users;
        }

        [HttpGet("{user_id}")]
        [IdFilterAsync<User>]
        [Produces(typeof(UserDTO))]
        public async Task<ActionResult<UserDTO>> GetUser(long user_id){
            return (UserDTO)(await _db.Users.FindAsync(user_id))!;
        }

        [HttpPost]
        public async Task<ActionResult> AddUser(User user_data){
            if(IsUsedEmail(user_data.Email)){
                return BadRequest("Provided email address is already taken");
            }

            _db.Users.Add(user_data);

            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(AddUser), (UserDTO)user_data);
        }

        [HttpDelete("{user_id}")]
        [IdFilterAsync<User>]
        public async Task<ActionResult> DeleteUser(long user_id){
            var user = await _db.Users.FindAsync(user_id);

            if(user is null){
                return NotFound();
            }

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{user_id}")]
        [IdFilterAsync<User>]
        public async Task<ActionResult<UserDTO>> PutUser(long user_id, User new_data){
            if(new_data.Id != user_id){
                return BadRequest();
            }

            var cahnges_email = _db.Users.AsNoTracking()
                .First(u => u.Id == user_id).Email != new_data.Email;
            if(cahnges_email && IsUsedEmail(new_data.Email)){
                return BadRequest("Provided new email address is already taken");
            }
            
            _db.Entry(new_data).State = EntityState.Modified;

            try{
                await _db.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException){
                if(!_db.Users.Any(u => u.Id == user_id)){
                    return NotFound();
                } else throw;
            }

            return NoContent();
        }

        private bool IsUsedEmail(string email){
            return _db.Users.Any(u => u.Email == email);
        }
    }
}