using CiPlatformWeb.Entities.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace CiPlatformWeb.UserAccess
{
    public class CountryCityActionFilter : IActionFilter
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CountryCityActionFilter (ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        public void OnActionExecuted (ActionExecutedContext context)
        {
            var controller = context.RouteData.Values["controller"].ToString();
            var action = context.RouteData.Values["action"].ToString();

            if (controller == "Admin" || controller == "Home" || action == "UserProfile" || action == "GetCitiesByCountry" || action == "PrivacyPolicy")
            {
                // Allow login page to load even if country and city are not set
                return;
            }

            long userId = 0;
            string authorizationHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            string token = authorizationHeader?.Substring("Bearer ".Length).Trim();
            if (token is not null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var decodedToken = tokenHandler.ReadJwtToken(token);
                var claims = decodedToken.Claims;
                var customClaimString = decodedToken.Claims.FirstOrDefault(c => c.Type == "CustomClaimForUser")?.Value;
                var customClaimValue = JsonSerializer.Deserialize<User>(customClaimString);
                userId = customClaimValue.UserId;
            }

            if (userId != 0)
            {
                User sessionUser = _db.Users.FirstOrDefault(u => u.UserId == userId);
                if (sessionUser.CountryId == null || sessionUser.CityId == null)
                {
                    if (controller != "User" || action != "UserProfile")
                    {
                        context.Result = new RedirectToRouteResult(
                           new RouteValueDictionary(new { controller = "User", action = "UserProfile" })
                       );
                    }
                }
            }
            else
            {
                if (controller != "Home" || action != "Index")
                {
                    context.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(new { controller = "Home", action = "Index" })
                    );
                }
            }
        }

        public void OnActionExecuting (ActionExecutingContext context)
        {
            //
        }
    }
}
