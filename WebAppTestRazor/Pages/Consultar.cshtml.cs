using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAppTestRazor.Pages
{
    public class ConsultarModel : PageModel
    {

        public string Startdate = "2023-10-25";
        public IActionResult OnGet()
        {
         
            OnGetFilteredDates();

            return RedirectToPage("/Index");
        }

        public IActionResult OnGetFilteredDates()
        {
            // Obtenha os valores dos parâmetros da URL
            if (Request.Query.TryGetValue("startDate", out var startDate) &&
                Request.Query.TryGetValue("endDate", out var endDate))
            {
                // Converta os valores para DateTime
                if (DateTime.TryParse(startDate, out var startDateValue) &&
                    DateTime.TryParse(endDate, out var endDateValue))
                {
                    // Crie uma instância de DateFilterModel com os valores
                    var filter = new DateFilterModel
                    {
                        StartDate = startDateValue,
                        EndDate = endDateValue
                    };

                    var cookieOptions = new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(1) // Define a data de expiração para 1 dia a partir de agora
                    };
                    // Faça o que for necessário com o filtro
                    TempData["StartDate"] = filter.StartDate;
                    TempData["EndDate"] = filter.EndDate;

                    // Configure os cookies se necessário
                    string formattedStartDate = filter.StartDate.ToString("yyyy-MM-dd");
                    string formattedEndDate = filter.EndDate.ToString("yyyy-MM-dd");
                    
                    Response.Cookies.Append("StartDate", formattedStartDate);
                    Response.Cookies.Append("EndDate", formattedEndDate);
                }
            }

            return RedirectToPage("/Index");
        }

        public class DateFilterModel
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }
    

    }
	

}
