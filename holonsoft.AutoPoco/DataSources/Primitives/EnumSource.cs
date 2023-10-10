using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;
public class EnumSource<T> : DataSourceBase<T> where T : struct, IConvertible
{
    public override T Next(IGenerationContext? context)
    {
        if (!typeof(T).IsEnum)
            throw new ArgumentException("T must be an enumerated type");

        var values = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
        return values[Random.Next(values.Length)];
    }
}
