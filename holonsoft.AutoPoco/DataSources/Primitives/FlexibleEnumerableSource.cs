using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;

public class FlexibleEnumerableSource<TSource, TCollectionType, TCollectionElement>(int minCount, int maxCount, params object[] args) : IDataSource<TCollectionType>
  where TCollectionType : IEnumerable<TCollectionElement>
  where TSource : DataSourceBase<TCollectionElement>
{
    private readonly EnumerableSource<TSource, TCollectionElement> _innerSource = new(minCount, maxCount, args);

    public FlexibleEnumerableSource(int count)
      : this(count, count, Array.Empty<object>())
    {
    }

    public FlexibleEnumerableSource(int min, int max)
      : this(min, max, Array.Empty<object>())
    {
    }

    public IEnumerable<TCollectionElement> Next(IGenerationContext? context)
    {
        var propertyCollection = Activator.CreateInstance(typeof(TCollectionType)) as ICollection<TCollectionElement>;
        var collectionContents = _innerSource.Next(context);

        foreach (var item in collectionContents)
            propertyCollection?.Add(item);
        return propertyCollection ?? throw new InvalidOperationException();
    }

    object IDataSource.Next(IGenerationContext? context)
    {
        var propertyCollection = Activator.CreateInstance(typeof(TCollectionType)) as ICollection<TCollectionElement>;
        var collectionContents = _innerSource.Next(context);

        foreach (var item in collectionContents)
            propertyCollection?.Add(item);
        return propertyCollection ?? throw new InvalidOperationException();
    }
}