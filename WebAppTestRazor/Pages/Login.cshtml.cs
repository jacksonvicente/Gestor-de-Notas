using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using FirebaseAdmin.Messaging;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;

namespace WebAppTestRazor.Pages
{
    public class LoginModel : PageModel
    {
        public bool IsLoggedIn { get; set; } = false;
        public string UserEmail { get; set; }
        public bool UserLoggedIn { get; set; } = false;

        public void OnGet()
        {
            
            if (User.Identity.IsAuthenticated)
            {
                IsLoggedIn = true;
                UserEmail = User.Identity.Name;
                UserLoggedIn = true;
            }
        }
        public IActionResult OnPostLogin()
        {
           
            return RedirectToPage("/Index");
        }
    }

}
