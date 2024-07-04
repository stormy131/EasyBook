using EasyBook.Models;
using EasyBook.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace EasyBook.Controllers{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase{
        private readonly EasyBookContext _db;

        public OrdersController(EasyBookContext context){
            _db = context;
        }

        [HttpGet("{user_id}")]
        [IdFilterAsync<User>]
        public IQueryable<OrderDTO> GetUserOrders(long user_id){
            return _db.Orders.Include("OrderedItems").Where(o => o.UserId == user_id).Select(o => (OrderDTO) o);
        }

        [HttpPost]
        public async Task<ActionResult<OrderDTO>> PostOrder(OrderDTO order_data){
            _db.Orders.Add(order_data);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(PostOrder), order_data);
        }

        [HttpDelete("{user_id}/{order_id}")]
        [IdFilterAsync<User>, AuthorityFilterAsync<Order>]
        public async Task<ActionResult> DeleteOrder(long order_id){
            var order = await _db.Orders.FindAsync(order_id);

            if (order is null) {
                return NotFound();
            }

            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();

            return NoContent();
        }

    }
}