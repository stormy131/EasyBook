using EasyBook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EasyBook.Filters{

    // Custonm filter to verify the existance of a instance woth provided ID.
    // Should be applied to the endpoints, that operate with specific entity instances.
    public class ExistanceFilterAsync<TEntity> : Attribute, IAsyncActionFilter where TEntity : class {
        private readonly Dictionary<Type, string> route_paramter = new Dictionary<Type, string>{
            {typeof(User), "user_id"},
            {typeof(BookItem), "book_id"},
            {typeof(ReviewItem), "review_id"},
            {typeof(Order), "order_id"}
        };

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var scope = context.HttpContext.RequestServices.CreateAsyncScope();
            var _db = scope.ServiceProvider.GetService<EasyBookContext>()!;

            var item_id = Convert.ToInt64(context.RouteData.Values[route_paramter[typeof(TEntity)]]);

            var record = await _db.Set<TEntity>().FindAsync(item_id);
            if(record is null){
                context.Result = new StatusCodeResult(StatusCodes.Status404NotFound);
            } else {
                await next();
            }
        }
    } 
}