using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.VisualBasic;

namespace WebAppTestRazor.Pages
{
    public class IndexModel : PageModel
    {

        private readonly ILogger<IndexModel> _logger;
        private readonly HttpClient _httpClient;
       
        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)] 
        public DateTime? EndDate { get; set; }
        [BindProperty(SupportsGet = true)]
        public int Tipo0Count { get; set; }
        [BindProperty(SupportsGet = true)]
        public int Tipo9Count { get; set; }
        [BindProperty(SupportsGet = true)]
        public Dictionary<string, double> TotalPorProdutoTipo0 { get; set; }
        [BindProperty(SupportsGet = true)]
        public Dictionary<string, double> TotalPorProdutoTipo9 { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool enviar {  get; set; }

//----------------------------------------------------------------------//
        [BindProperty(SupportsGet = true)]
        public string volumeGCSNF {  get; set; }
        [BindProperty(SupportsGet = true)]
        public string volumeGASNF { get; set; }
        [BindProperty(SupportsGet = true)]
        public string volumeECSNF { get; set; }
        [BindProperty(SupportsGet = true)]
        public string volumeDS10SNF { get; set; }
        [BindProperty(SupportsGet = true)]
        public string volumeDS500SNF { get; set; }
        //------------------------------------------//
        [BindProperty(SupportsGet = true)]
        public string volumeGCCNF { get; set; }
        [BindProperty(SupportsGet = true)]
        public string volumeGACNF { get; set; }
        [BindProperty(SupportsGet = true)]
        public string volumeECCNF { get; set; }
        [BindProperty(SupportsGet = true)]
        public string volumeDS10CNF { get; set; }
        [BindProperty(SupportsGet = true)]
        public string volumeDS500CNF { get; set; }
        [BindProperty(SupportsGet = true)]
        public string volumeTSNF { get; set; }
        [BindProperty(SupportsGet = true)]
        public string volumeTCNF { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory)
        {            
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("DadosController"); // Nome do cliente HttpClient
        }

        public async Task OnGet()
        {
            
            if (this.Tipo0Count == 0||this.Tipo9Count == 0) { this.Tipo0Count = 1; this.Tipo9Count = 1; }
            Page();
            if (StartDate.HasValue || EndDate.HasValue)
            {
                bool valida = false;
                DateTime sDate;
                DateTime dDate;
                if (StartDate==null) {
                 
                    sDate = DateTime.Now;
                    valida = true;
                }
                else
                {
                    sDate = StartDate.Value;
                    
                }
                if (EndDate == null)
                {
                    dDate = DateTime.Now;
                    valida = true;
                }
                else { dDate = EndDate.Value;  }

                if (dDate < sDate)
                {
                    dDate = sDate;
                    valida = true;
                }
                if (valida==false)
                {
                    OnGetFilteredDates();
                }
                /*else
                {
                    StartDate = sDate;
                    EndDate = dDate;
                    OnGetFilteredDates();
                }*/
                
                await ObterDados(sDate.ToString("yyyy-MM-dd"), dDate.ToString("yyyy-MM-dd"));
            }
            else
            {
                StartDate = DateTime.Now;
                EndDate = DateTime.Now;
                OnGetFilteredDates();
                await ObterDados(StartDate.Value.ToString("yyyy-MM-dd"), EndDate.Value.ToString("yyyy-MM-dd"));
            }

            
        }

        

        private async Task ObterDados(string startDate,string endDate)
        {
            
            string link = "https://0232-187-108-133-151.ngrok-free.app/WSEAIDataExchange?tipoinformacao=101&emitentes=45181198000195&datainicial=" + startDate + "&datafinal=" + endDate;
            try
            {
                var responseString = await _httpClient.GetStringAsync(link);
                var dados = JsonConvert.DeserializeObject<List<SeuModelo>>(responseString);
                // Variáveis para armazenar as estatísticas
                int tipo0Count = 0;
                int tipo9Count = 0;
                double totalVolumeSNF = 0.0;
                double totalVolumeCNF = 0.0;
                Dictionary<string, double> totalPorProdutoTipo0 = new Dictionary<string, double>();
                Dictionary<string, double> totalPorProdutoTipo9 = new Dictionary<string, double>();

                foreach (var item in dados)
                {
                    
                    foreach (var produto in item.itens)
                    {
                       
                        if (item.tipopreatendimentoid == 0)
                        {
                            tipo0Count++;
                            if (totalPorProdutoTipo0.ContainsKey(produto.produtodescricao))
                            {
                                totalPorProdutoTipo0[produto.produtodescricao] += produto.quantidade;
                            }
                            else
                            {
                                totalPorProdutoTipo0[produto.produtodescricao] = produto.quantidade;
                            }
                        }
                        else if (item.tipopreatendimentoid == 9)
                        {
                            tipo9Count++;
                            if (totalPorProdutoTipo9.ContainsKey(produto.produtodescricao))
                            {
                                totalPorProdutoTipo9[produto.produtodescricao] += produto.quantidade;
                            }
                            else
                            {
                                totalPorProdutoTipo9[produto.produtodescricao] = produto.quantidade;
                            }
                        }
                    }

                   
                }
               
                this.Tipo0Count = tipo0Count;
                this.Tipo9Count = tipo9Count;   
                this.TotalPorProdutoTipo0 = totalPorProdutoTipo0;
                this.TotalPorProdutoTipo9 = totalPorProdutoTipo9;
             
            
                
                if (totalPorProdutoTipo9.ContainsKey("GASOLINA COMUM"))
                {
                    double qdt = totalPorProdutoTipo9["GASOLINA COMUM"];
                    totalVolumeSNF += qdt;
                    this.volumeGCSNF = $"{qdt:F3}";
                    
                }
                if (totalPorProdutoTipo9.ContainsKey("ETANOL"))
                {
                    double qdt = totalPorProdutoTipo9["ETANOL"];
                    totalVolumeSNF += qdt;
                    this.volumeECSNF = $"{qdt:F3}";

                }
                if (totalPorProdutoTipo9.ContainsKey("GASOLINA ADITIVADA"))
                {
                    double qdt = totalPorProdutoTipo9["GASOLINA ADITIVADA"];
                    totalVolumeSNF += qdt;
                    this.volumeGASNF = $"{qdt:F3}";

                }
                if (totalPorProdutoTipo9.ContainsKey("DIESEL S 10"))
                {
                    double qdt = totalPorProdutoTipo9["DIESEL S 10"];
                    totalVolumeSNF += qdt;
                    this.volumeDS10SNF = $"{qdt:F3}";

                }
                if (totalPorProdutoTipo9.ContainsKey("DIESEL S 500"))
                {
                    double qdt = totalPorProdutoTipo9["DIESEL S 500"];
                    totalVolumeSNF += qdt;
                    this.volumeDS500SNF = $"{qdt:F3}";

                }
                //---------------------------------------------------------------------//
                if (totalPorProdutoTipo0.ContainsKey("GASOLINA COMUM"))
                {
                    double qdt = totalPorProdutoTipo0["GASOLINA COMUM"];
                    totalVolumeCNF += qdt;
                    this.volumeGCCNF = $"{qdt:F3}";

                }
                if (totalPorProdutoTipo0.ContainsKey("ETANOL"))
                {
                    double qdt = totalPorProdutoTipo0["ETANOL"];
                    totalVolumeCNF += qdt;
                    this.volumeECCNF = $"{qdt:F3}";

                }
                if (totalPorProdutoTipo0.ContainsKey("GASOLINA ADITIVADA"))
                {
                    double qdt = totalPorProdutoTipo0["GASOLINA ADITIVADA"];
                    totalVolumeCNF += qdt;
                    this.volumeGACNF = $"{qdt:F3}";

                }
                if (totalPorProdutoTipo0.ContainsKey("DIESEL S 10"))
                {
                    double qdt = totalPorProdutoTipo0["DIESEL S 10"];
                    totalVolumeCNF += qdt;
                    this.volumeDS10CNF = $"{qdt:F3}";

                }
                if (totalPorProdutoTipo0.ContainsKey("DIESEL S 500"))
                {
                    double qdt = totalPorProdutoTipo0["DIESEL S 500"];
                    totalVolumeCNF += qdt;
                    this.volumeDS500CNF = $"{qdt:F3}";

                }
                this.volumeTCNF = $"{totalVolumeCNF:F3}";
                this.volumeTSNF = $"{totalVolumeSNF:F3}";
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("Erro na solicitação HTTP: " + e.Message);
            }

        }
       
        class SeuModelo
        {
            public int tipopreatendimentoid { get; set; }
            public int numero { get; set; }
            public decimal valortotalnota { get; set; }
            public List<Item> itens { get; set; }
            public string dataemissao { get; set; }
        }
        class Item
        {
            public string produtodescricao { get; set; }

            public double quantidade { get; set; }
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
            else
            {
                // Crie uma instância de DateFilterModel com os valores
                var filter = new DateFilterModel
                {
                    StartDate = this.StartDate.Value,
                    EndDate = this.EndDate.Value
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

            return RedirectToPage("/Index");
        }

        public class DateFilterModel
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }

    }
    public class DadosController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public DadosController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

      

    }


}