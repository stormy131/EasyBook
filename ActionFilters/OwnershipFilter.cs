using EasyBook.Identity;
using EasyBook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EasyBook.Filters {
    
    // Helper interface to denote entities, that were created by user
    public interface ICreatedByUser {
        public long UserId { get; set; }
    }

    // Custom filter for verifying user access to the target endpoint.
    // Should be applied to the endpoints, that operate with specific entity instances.
    public class OwnershipFilterAsync<TEntity> : Attribute, IAsyncActionFilter where TEntity : class, ICreatedByUser {
        private readonly Dictionary<Type, string> route_parameter = new Dictionary<Type, string>{
            {typeof(Order), "order_id"},
            {typeof(ReviewItem), "review_id"}
        };

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next){
            var issuer_claims = context.HttpContext.User.Claims;
            var is_admin = issuer_claims.FirstOrDefault(c => c.Type == IdentityData.AdminUserClaim)!;

            if(Convert.ToBoolean(is_admin.Value)){
                await next();
                return;
            }

            var scope = context.HttpContext.RequestServices.CreateAsyncScope();
            var _db = scope.ServiceProvider.GetService<EasyBookContext>()!;

            var item_id = Convert.ToInt64(
                context.RouteData.Values[route_parameter[typeof(TEntity)]]
            );

            var item_record = (await _db.Set<TEntity>().FindAsync(item_id))!;
            var issuer_id = Convert.ToInt64(
                issuer_claims.FirstOrDefault(x => x.Type == "id")!.Value
            );
            
            if(item_record.UserId != issuer_id){
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
            } else {
                await next();
            }
        }
    }
}