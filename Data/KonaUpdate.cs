using Kona;
using Newtonsoft.Json;

namespace Kona;
public class PostUpdate
{
    public enum ProcessState
    {
        Pending,
        Processing,
        Finished,
        Failed,
    }

    //private KonaContext context;

    private readonly IServiceScopeFactory scopeFactory;

    public PostUpdate(IServiceScopeFactory scopeFactory)
    {
        this.scopeFactory = scopeFactory;
        this.processTask = new Task(Process);
    }
    // public PostUpdate(IServiceProvider sp)
    // {
    //     this.context = sp.GetRequiredService<KonaContext>();
    //     this.processTask = new Task(Process);
    // }
    public class Item
    {
        public string name;
        public string json;
        public ProcessState state;
        public string message;
        public int total;
        public int progress;
        public string GetProgressStr()
        {
            return state == ProcessState.Pending ? "N/A" : string.Format("{0}/{1}", progress, total);
        }

    }

    private readonly List<Item> pendingList = new List<Item>();
    private readonly List<Item> processingList = new List<Item>();
    private readonly List<Item> finishedList = new List<Item>();

    Task processTask;
    public void Add(string name, string json)
    {
        pendingList.Add(new Item { name = name, json = json, state = ProcessState.Pending, message = string.Empty });
    }

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

    public void ClearPending()
    {
        pendingList.Clear();
    }

    public List<Item> GetStatus()
    {
        List<Item> result = new List<Item>();
        result.AddRange(pendingList);
        result.AddRange(processingList);
        result.AddRange(finishedList);
        return result;
    }


    private void Process()
    {
        var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<KonaContext>();
        processingList.Clear();
        processingList.AddRange(pendingList);
        pendingList.Clear();
        foreach (var item in processingList)
        {
            try
            {
                item.state = ProcessState.Processing;
                var rawPosts = JsonConvert.DeserializeObject<List<DataUtils.post>>(item.json);
                if (null != rawPosts)
                {
                    int count = 0;
                    item.total = rawPosts.Count;
                    item.progress = 0;
                    List<int> invalidIds = new List<int>();
                    foreach (var p in rawPosts)
                    {
                        item.progress++;
                        if (p.IsValid)
                        {
                            DataUtils.AddOrUpdatePost(p, context);
                            count++;
                        }
                        else
                        {
                            invalidIds.Add(p.id);
                        }
                    }
                    item.message = string.Format("{0} post(s) added or updated", count);
                    if (invalidIds.Count > 0)
                        item.message += string.Format(", invalid ids: [{0}]", string.Join(", ", invalidIds));
                }
                item.state = ProcessState.Finished;
            }
            catch (Exception e)
            {
                item.state = ProcessState.Failed;
                item.message = e.Message + "\n" + e.InnerException;
            }

        }
        finishedList.AddRange(processingList);
        processingList.Clear();
    }
}