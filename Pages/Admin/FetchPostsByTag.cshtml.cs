using Kona;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace kona.Pages;

public class FetchPostsByTag : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string tags { get; set; }
    [BindProperty(SupportsGet = true)]
    public int page { get; set; }
    public string result { get; set; }
    public string resultFormatted { get; set; }
    public string error { get; set; }
    public bool fetchSuccess { get; set; }

    [BindProperty]
    public string submitMessage { get; set; }
    [BindProperty]
    public string postContent { get; set; }
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

    private PostUpdate postUpdate;
    public FetchPostsByTag(PostUpdate postUpdate)
    {
        this.postUpdate = postUpdate;
    }
    private static string FormatJson(string json)
    {
        dynamic parsedJson = JsonConvert.DeserializeObject(json);
        return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
    }
    public async Task<IActionResult> OnGetAsync()
    {
        if (!string.IsNullOrWhiteSpace(tags))
        {
            try
            {
                var response = httpClient.GetAsync("https://konachan.net/post.json?limit=100&tags=" + tags);
                result = await response.Result.Content.ReadAsStringAsync();
                resultFormatted = FormatJson(result);
                fetchSuccess = true;
            }
            catch (Exception e)
            {
                fetchSuccess = false;
                error = e.Message + "\n" + e.InnerException;
            }
        }
        return Page();
    }

    public IActionResult OnPost()
    {
        if (!string.IsNullOrWhiteSpace(postContent))
        {
            postUpdate.Add(string.IsNullOrWhiteSpace(submitMessage) ? string.Format("Fatched posts with tag {0}", tags) : submitMessage, postContent);
            return RedirectToPage("./UpdatePostsProgress");
        }
        return RedirectToPage("/Index");
    }
}