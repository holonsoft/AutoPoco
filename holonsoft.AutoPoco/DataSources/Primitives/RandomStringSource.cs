using System.Text;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;

public abstract class RandomStringSourceBase<T>(int minLength, int maxLength, int? nullCreationThreshold = null, params char[] allowedChars) : DataSourceBase<T> {
   private readonly IRandomNullEvaluator _defaultRandomNullEvaluator = new DefaultRandomNullEvaluator(nullCreationThreshold ?? AutoPocoGlobalSettings.NullCreationThreshold);

   private readonly char[] _allowedChars = allowedChars;

   public RandomStringSourceBase(int minLength, int maxLength, char minChar, char maxChar, int? nullCreationThreshold = null)
      : this(minLength, maxLength, nullCreationThreshold)
      => _allowedChars = Enumerable.Range(minChar, maxChar - minChar + 1)
                            .Select(i => (char) i)
                            .ToArray();

   protected override T GetNextValue(IGenerationContext? context) {
      if (nullCreationThreshold.HasValue) {
         if (_defaultRandomNullEvaluator.ShouldNextValueReturnNull())
            return (T) (object) null!;
      }

      var result = Enumerable.Range(0, Random.Next(minLength, maxLength + 1))
            .Select(x => _allowedChars[Random.Next(0, _allowedChars.Length - 1)])
            .Aggregate(new StringBuilder(), (builder, c) => builder.Append(c))
            .ToString();

      return (T) (object) result;
   }
}

public class RandomStringSource : RandomStringSourceBase<string> {
   public RandomStringSource()
      : this(5, 10, 'A', 'z') { }

   public RandomStringSource(int minLength, int maxLength)
     : this(minLength, maxLength, (char) 65, (char) 123) { }

   public RandomStringSource(int minLength, int maxLength, char minChar, char maxChar)
      : base(minLength, maxLength, minChar, maxChar, null) { }

   public RandomStringSource(int minLength, int maxLength, char[] allowedChars)
      : base(minLength, maxLength, null, allowedChars) { }
}

public class NullableRandomStringSource : RandomStringSourceBase<string> {
   public NullableRandomStringSource()
      : this(5, 10, 'A', 'z') { }

   public NullableRandomStringSource(int minLength, int maxLength)
     : this(minLength, maxLength, (char) 65, (char) 123) { }

   public NullableRandomStringSource(int minLength, int maxLength, char minChar, char maxChar)
      : base(minLength, maxLength, minChar, maxChar, AutoPocoGlobalSettings.NullCreationThreshold) { }

   public NullableRandomStringSource(int minLength, int maxLength, char minChar, char maxChar, int? nullCreationThreshold)
      : base(minLength, maxLength, minChar, maxChar, nullCreationThreshold) { }

   public NullableRandomStringSource(int minLength, int maxLength, char[] allowedChars)
      : base(minLength, maxLength, AutoPocoGlobalSettings.NullCreationThreshold, allowedChars) { }

   public NullableRandomStringSource(int minLength, int maxLength, char[] allowedChars, int? nullCreationThreshold)
      : base(minLength, maxLength, nullCreationThreshold, allowedChars) { }
}