using EasyBook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EasyBook.Filters {
    public interface ICreatedByUser {
        public long UserId { get; set; }
    }

    public class AuthorityFilterAsync<TEntity> : Attribute, IAsyncActionFilter where TEntity : class, ICreatedByUser {
        private readonly Dictionary<Type, string> route_parameter = new Dictionary<Type, string>{
            {typeof(Order), "order_id"},
            {typeof(ReviewItem), "review_id"}
        };

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next){
            var scope = context.HttpContext.RequestServices.CreateAsyncScope();
            var _db = scope.ServiceProvider.GetService<EasyBookContext>()!;

            var item_id = Convert.ToInt64(
                context.RouteData.Values[route_parameter[typeof(TEntity)]]
            );

            var item_record = (await _db.Set<TEntity>().FindAsync(item_id))!;
            var issuer_id = Convert.ToInt64(
                context.HttpContext.User
                    .Claims.Single(x => x.Type == "id")
                    .Value
            );
            
            if(item_record.UserId != issuer_id){
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
            } else {
                await next();
            }
        }
    }
}