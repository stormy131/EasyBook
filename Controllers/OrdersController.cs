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

        [HttpGet]
        public IQueryable<OrderDTO> GetUserOrders(){
            var issuer_id = HttpContext.User.Claims.FirstOrDefault(
                c => c.Type == "id"
            );

            return _db.Orders.Include("OrderedItems")
                .Where(o => o.UserId == Convert.ToInt32(issuer_id.Value))
                .Select(o => (OrderDTO) o);
        }

        [HttpPost]
        public async Task<ActionResult<OrderDTO>> PostOrder(OrderDTO order_data){
            _db.Orders.Add(order_data);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(PostOrder), order_data);
        }

        [HttpDelete("{order_id}")]
        [OwnershipFilterAsync<Order>]
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