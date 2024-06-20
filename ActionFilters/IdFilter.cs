using EasyBook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EasyBook.Filters{
    public class IdFilterAsync : Attribute, IAsyncActionFilter{
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var scope = context.HttpContext.RequestServices.CreateAsyncScope();
            var _db = scope.ServiceProvider.GetService<EasyBookContext>()!;
            var item_id = Convert.ToInt64(context.RouteData.Values["book_id"]);

            var record = await _db.BookItems.FindAsync(item_id);
            if(record is null){
                context.Result = new StatusCodeResult(StatusCodes.Status404NotFound);
            } else {
                await next();
            }   
        }
    } 
}