using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RedLeg.Coaching.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        public IActionResult OnGet(string returnUrl = "/")
        {
            return new ChallengeResult("Auth0", new Microsoft.AspNetCore.Authentication.AuthenticationProperties
            {
                RedirectUri = returnUrl
            });
        }
    }
}