using Kona;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace Kona.Pages;

public class FetchPostsByTag : PageModel
{
    public int pageIndex { get; set; }
    public string tags { get; set; }
    public string url { get; set; }
    public string result { get; set; }
    public string resultFormatted { get; set; }
    public string error { get; set; }
    public bool fetchSuccess { get; set; }

    //public string submitMessage { get; set; }
    //public string postContent { get; set; }
    private string emptyTags { get; set; }
    static HttpClient? _httpClient = null;
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

    private KonaUpdateService updateService;
    public FetchPostsByTag(KonaUpdateService updateService)
    {
        this.updateService = updateService;
    }

    public async Task<IActionResult> OnGetAsync(string tags, string sortingTags, string pageIndex, string fetchNow)
    {
        return await Show(tags, sortingTags, pageIndex, fetchNow);
    }

    public async Task<IActionResult> Show(string tags, string sortingTags, string pageIndex, string? fetchNow)
    {
        this.tags = tags;
        if (int.TryParse(pageIndex, out var _page))
            this.pageIndex = _page;
        else
            this.pageIndex = 1;
        this.pageIndex = Math.Max(1, this.pageIndex);
        emptyTags = string.IsNullOrWhiteSpace(tags) ? "latest posts" : string.Empty;
        string combinedTags = string.Format("{0} {1}", tags, sortingTags).Trim();
        result = string.Empty;
        if (!string.IsNullOrWhiteSpace(combinedTags) && !string.IsNullOrWhiteSpace(fetchNow))
        {
            try
            {
                url = $"https://konachan.net/post.json?limit=100&page={this.pageIndex}&tags={combinedTags}";
                var response = httpClient.GetAsync(url);
                result = await response.Result.Content.ReadAsStringAsync();
                resultFormatted = DataUtils.FormatJson(result);
                fetchSuccess = true;
            }
            catch (Exception e)
            {
                fetchSuccess = false;
                error = e.Message + "\n" + e.InnerException;
            }
        }
        //submitMessage = GetDefaultSubmitMessage();
        return Page();
    }

    public async Task<IActionResult> OnPost(string submitMessage, string submitContent, string tags, string sortingTags, string pageIndex)
    {
        if (!string.IsNullOrWhiteSpace(submitContent))
        {
            updateService.Add(KonaUpdateService.UpdateItemType.Posts, string.IsNullOrWhiteSpace(submitMessage) ? GetDefaultSubmitMessage() : submitMessage, submitContent);
            updateService.Begin();
        }
        return await Show(tags, sortingTags, pageIndex, null);
    }

    public string GetDefaultSubmitMessage()
    {
        if (string.IsNullOrWhiteSpace(tags))
            return string.Format("Fetched {0} on page {1}", emptyTags, pageIndex);
        else
            return string.Format("Fetched posts of '{0}' on page {1}", tags, pageIndex);
    }
}