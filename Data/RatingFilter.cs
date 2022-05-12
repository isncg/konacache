using System.Collections;
using System.Linq.Expressions;

namespace Kona;

public abstract class RatingFilter
{
    protected IQueryable<Post> posts;
    public RatingFilter(IQueryable<Post> posts)
    {
        this.posts = posts;
    }
    public abstract IQueryable<Post> Process();
}

public class RatingFilterService
{
    public enum FilterType
    {
        None = 0,
        S = 1,
        Q = 2,
        E = 4,
        SQ = S | Q,
        SE = S | E,
        QE = Q | E,
        ALL = S | Q | E
    }
    public FilterType filterType = FilterType.None;
    public void SetFilter(bool s, bool q, bool e)
    {
        FilterType type = FilterType.None;
        if (s)
            type |= FilterType.S;
        if (q)
            type |= FilterType.Q;
        if (e)
            type |= FilterType.E;
        this.filterType = type;
    }
    public void GetFilter(out bool s, out bool q, out bool e)
    {
        s = (filterType & FilterType.S) > 0;
        q = (filterType & FilterType.Q) > 0;
        e = (filterType & FilterType.E) > 0;
    }

    public IQueryable<Post> Get(IQueryable<Post> posts)
    {
        return GetFilter(posts).Process();
    }

    public RatingFilter GetFilter(IQueryable<Post> posts)
    {
        switch (filterType)
        {
            case FilterType.S:
                return new RatingFilter_S(posts);
            case FilterType.Q:
                return new RatingFilter_Q(posts);
            case FilterType.E:
                return new RatingFilter_E(posts);
            case FilterType.SQ:
                return new RatingFilter_SQ(posts);
            case FilterType.SE:
                return new RatingFilter_SE(posts);
            case FilterType.QE:
                return new RatingFilter_QE(posts);
            case FilterType.ALL:
                return new RatingFilter_ALL(posts);
        }
        return new RatingFilter_S(posts);
    }

    class RatingFilter_ALL : RatingFilter
    {
        public RatingFilter_ALL(IQueryable<Post> posts) : base(posts)
        {
        }

        public override IQueryable<Post> Process()
        {
            return posts;
        }
    }

    class RatingFilter_S : RatingFilter
    {
        public RatingFilter_S(IQueryable<Post> posts) : base(posts)
        {
        }

        public override IQueryable<Post> Process()
        {
            return posts.Where(e=>e.Rating == PostRating.S);
        }
    }

    class RatingFilter_Q : RatingFilter
    {
        public RatingFilter_Q(IQueryable<Post> posts) : base(posts)
        {
        }

        public override IQueryable<Post> Process()
        {
            return posts.Where(e=>e.Rating == PostRating.Q);
        }
    }

    class RatingFilter_E : RatingFilter
    {
        public RatingFilter_E(IQueryable<Post> posts) : base(posts)
        {
        }

        public override IQueryable<Post> Process()
        {
            return posts.Where(e=>e.Rating == PostRating.E);
        }
    }

    class RatingFilter_SQ : RatingFilter
    {
        public RatingFilter_SQ(IQueryable<Post> posts) : base(posts)
        {
        }

        public override IQueryable<Post> Process()
        {
            return posts.Where(e=>e.Rating == PostRating.S || e.Rating == PostRating.Q);
        }
    }

    class RatingFilter_SE : RatingFilter
    {
        public RatingFilter_SE(IQueryable<Post> posts) : base(posts)
        {
        }

        public override IQueryable<Post> Process()
        {
            return posts.Where(e=>e.Rating == PostRating.S || e.Rating == PostRating.E);
        }
    }

    class RatingFilter_QE : RatingFilter
    {
        public RatingFilter_QE(IQueryable<Post> posts) : base(posts)
        {
        }

        public override IQueryable<Post> Process()
        {
            return posts.Where(e=>e.Rating == PostRating.Q || e.Rating == PostRating.E);
        }
    }
}

public static class FilterUtils
{
    public static IQueryable<Post> SelectFilter(this IQueryable<Post> posts, RatingFilterService filter)
    {
        return filter.Get(posts);
    }
}