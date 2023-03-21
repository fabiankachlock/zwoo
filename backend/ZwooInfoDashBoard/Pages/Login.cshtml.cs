using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ZwooInfoDashBoard.Pages;


public class LoginModel : PageModel
{
    public async Task OnGet()
    {
        await HttpContext.ChallengeAsync("oidc", new AuthenticationProperties());
    }
}
