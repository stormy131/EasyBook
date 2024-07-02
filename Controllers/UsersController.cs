using EasyBook.Models;
using EasyBook.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EasyBook.Identity;
using EasyBook.Services;
using Microsoft.AspNetCore.Authorization;

namespace EasyBook.Controllers{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase{
        private readonly EasyBookContext _db;

        public UsersController(EasyBookContext context) {
            _db = context;
        }

        [HttpPost("login")]
        public ActionResult<string> Authenticate(LoginData data, AuthService auth_service){
            try{
                var user_data = _db.Users.Single(
                    u => u.Email == data.Email && u.Password == data.Password
                );

                return auth_service.Create(user_data);
            } catch {
                return BadRequest("Invalid user data");
            }
        }

        [HttpGet]
        [Authorize(Policy = IdentityData.AdminUserPolicy)]
        public IQueryable<UserDTO> GetAllUsers(){
            var users = _db.Users.Select(u => (UserDTO)u);
            return users;
        }

        [HttpGet("{user_id}")]
        [IdFilterAsync<User>]
        [Authorize]
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
        [Authorize(Policy = IdentityData.AdminUserPolicy)]
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
        [Authorize]
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