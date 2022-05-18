namespace Kona;

public class AutoUpdate
{
    private KonaUpdateService updateService;
    public AutoUpdate(KonaUpdateService updateService)
    {
        this.updateService = updateService;
    }
    static HttpClient? _httpClient = null;
    private int page = 1;
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
    
    Task processTask = null;

    public void Begin()
    {
        if (processTask == null)
        {
            processTask = new Task(Process);
            processTask.Start();
        }
        else
        {
            switch (processTask.Status)
            {
                case TaskStatus.Created:
                case TaskStatus.RanToCompletion:
                case TaskStatus.Canceled:
                case TaskStatus.Faulted:
                    processTask = new Task(Process);
                    processTask.Start();
                    break;
            }
        }
    }
    private void Process()
    {
        while (true)
        {
            if (updateService.IsPendingEmpty)
            {

                Console.WriteLine("Auto update start");
                var updateTask = Update();
                updateTask.Wait();
                bool result = updateTask.Result;
                if (page >= 1024)
                    page = 1;
                Console.WriteLine($"Auto update finish, result = {result}");
                if (result)
                {
                    page++;
                }
            }
            else
            {
                updateService.Begin();
            }
            Task.Delay(1000 * 30).Wait();
        }
    }

    private async Task<bool> Update()
    {
        string url = $"https://konachan.net/post.json?limit=100&page={page}";
        string result = null;
        string error;
        int tryCount = 16;
        while (tryCount-- > 0)
        {
            try
            {
                Console.WriteLine($"Loading url {url}");
                var response = httpClient.GetAsync(url);
                result = await response.Result.Content.ReadAsStringAsync();
                break;
            }
            catch (Exception e)
            {
                error = e.Message;
            }
            if (tryCount > 0)
            {
                Console.WriteLine($"Failed to load {url}, remain try count {tryCount}, error: {error}");
                await Task.Delay(2000);
            }
            else
            {
                return false;
            }
        }
        if (null == result)
            return false;
        updateService.Add(KonaUpdateService.UpdateItemType.Posts, "Auto update", result);
        updateService.Begin();

        return true;
    }
}