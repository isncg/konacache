using Kona;
using Newtonsoft.Json;

namespace Kona;
public class KonaUpdateService
{
    public enum UpdateItemProcessState
    {
        Pending,
        Processing,
        Finished,
        Failed,
    }

    public enum UpdateItemType
    {
        None = 0,
        Posts,
        Tags,
    }

    private readonly IServiceScopeFactory scopeFactory;

    public KonaUpdateService(IServiceScopeFactory scopeFactory)
    {
        this.scopeFactory = scopeFactory;
        this.processTask = new Task(Process);
    }

    public class UpdateItem
    {
        public UpdateItemType type;
        public string name;
        public string json;
        public UpdateItemProcessState state;
        public string message;
        public int total;
        public int progress;
        public string GetProgressStr()
        {
            switch (state)
            {
                case UpdateItemProcessState.Pending:
                    return "N/A";
                case UpdateItemProcessState.Processing:
                    return total == 0 ? "Calculating..." : $"{progress}/{total}";
            }
            return $"{progress}/{total}";
        }
    }


    private readonly List<UpdateItem> pendingList = new List<UpdateItem>();
    private readonly List<UpdateItem> processingList = new List<UpdateItem>();
    private readonly List<UpdateItem> finishedList = new List<UpdateItem>();

    Task processTask;
    public void Add(UpdateItemType type, string name, string json)
    {
        pendingList.Add(new UpdateItem { type = type, name = name, json = json, state = UpdateItemProcessState.Pending, message = string.Empty });
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

    public bool IsPendingEmpty => pendingList.Count == 0;

    public List<UpdateItem> GetStatus()
    {
        List<UpdateItem> result = new List<UpdateItem>();
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
                item.state = UpdateItemProcessState.Processing;
                switch (item.type)
                {
                    case UpdateItemType.Posts:
                        {
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
                                item.message = string.Format("{0} posts added or updated", count);
                                if (invalidIds.Count > 0)
                                    item.message += string.Format(", invalid ids: [{0}]", string.Join(", ", invalidIds));
                            }

                        }
                        break;
                    case UpdateItemType.Tags:
                        {
                            var tags = JsonConvert.DeserializeObject<List<DataUtils.tag>>(item.json);
                            if (null != tags)
                            {
                                item.total = tags.Count;
                                item.progress = 0;
                                item.message = $"{tags.Count} tags are adding or updating in one batch";
                                DataUtils.AddOrUpdateTags(tags, context);
                                item.progress = item.total;
                                item.message = $"{tags.Count} tags added or updated in one batch";
                            }
                            else
                            {
                                item.message = "Invalid data, skip";
                            }
                        }
                        break;
                }
                item.state = UpdateItemProcessState.Finished;

            }
            catch (Exception e)
            {
                item.state = UpdateItemProcessState.Failed;
                item.message = e.Message + "\n" + e.InnerException;
            }

        }
        finishedList.AddRange(processingList);
        processingList.Clear();
    }
}