using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.Engine;

public abstract class DataSourceBase<T> : IDataSource<T> {

   protected Random Random { get; private set; } = new(1337);

   public virtual void SetSeedToRandomValue()
      => Random = new(Guid.NewGuid().GetHashCode());

   public virtual void SetSeedToRandomValue(int seed)
      => Random = new(seed);

   object IDataSource.Next(IGenerationContext? context)
      => Next(context)!;

   /// <summary>
   ///   Gets the next object from this data source
   /// </summary>
   /// <returns></returns>
   public abstract T Next(IGenerationContext? context);
}