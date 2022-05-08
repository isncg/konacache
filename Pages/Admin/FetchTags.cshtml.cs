using Kona;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace kona.Pages;
class FetchTags: PageModel
{
    static HttpClient _httpClient = null;
    private HttpClient httpClient
    {
        get
        {
            if (null == _httpClient)
            {
                var httpClientHandler = new HttpClientHandler
                {
                    SslProtocols = System.Security.Authentication.SslProtocols.Tls
                };
                _httpClient = new HttpClient(httpClientHandler);
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
            }
            return _httpClient;
        }
    }
    [BindProperty(SupportsGet = true)]
    public string tag { get; set; }
    [BindProperty(SupportsGet = true)]
    public string source { get; set; }
    public string result {get; private set;}
    public string resultFormatted {get; private set;}
    public bool fetchSuccess {get; private set;}
    public string error {get; private set;}

    [BindProperty]
    public string submitMessage { get; set; }
    [BindProperty]
    public string submitContent { get; set; }
    public async Task<IActionResult> OnGetAsync()
    {
        if (string.IsNullOrWhiteSpace(tag) && string.IsNullOrWhiteSpace(source))
            return Page();

        try
        {
            var response = httpClient.GetAsync("https://konachan.net/tag.json?order=count&limit=10000&name=" + tag);
            result = await response.Result.Content.ReadAsStringAsync();
            resultFormatted = DataUtils.FormatJson(result);
            fetchSuccess = true;
        }
        catch (Exception e)
        {
            fetchSuccess = false;
            error = e.Message + "\n" + e.InnerException;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        return RedirectToPage("/Index");
    }
}