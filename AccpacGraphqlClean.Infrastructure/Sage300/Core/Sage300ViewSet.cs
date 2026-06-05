namespace AccpacGraphqlClean.Infrastructure;

public sealed class Sage300ViewSet
{
    private readonly Dictionary<string, object> _views = new(StringComparer.OrdinalIgnoreCase);

    public Sage300ViewSet(Sage300Session session, string viewIdsCsv, bool compose)
    {
        var viewIds = viewIdsCsv.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        foreach (var viewId in viewIds)
        {
            _views[viewId] = session.OpenView(viewId);
        }

        if (compose)
        {
            ComposeViews();
        }
    }

    public dynamic ViewById(string viewId) => _views[viewId];

    private void ComposeViews()
    {
        foreach (var view in _views.Values)
        {
            var compositeNames = TryGetCompositeNames((dynamic)view);
            if (compositeNames is null)
            {
                continue;
            }

            var count = TryGetCount(compositeNames);
            if (count <= 0)
            {
                continue;
            }

            var list = new List<object>(count);
            for (var i = 0; i < count; i++)
            {
                var name = Convert.ToString(compositeNames.Item(i));
                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                if (_views.TryGetValue(name, out object child))
                {
                    list.Add(child);
                }
            }

            if (list.Count == 0)
            {
                continue;
            }

            object objViews = list.ToArray();
            ((dynamic)view).Compose(ref objViews);
        }
    }

    private static dynamic? TryGetCompositeNames(dynamic view)
    {
        try
        {
            return view.CompositeNames;
        }
        catch
        {
            return null;
        }
    }

    private static int TryGetCount(dynamic collection)
    {
        try
        {
            return (int)collection.Count;
        }
        catch
        {
            return 0;
        }
    }
}
