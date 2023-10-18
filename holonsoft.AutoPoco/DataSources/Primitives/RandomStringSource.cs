using System.Text;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;

public abstract class RandomStringSourceBase<T> : DataSourceBase<T> {
   private char[] _allowedChars;
   private int _minLength;
   private int _maxLength;
   private readonly int? _nullCreationThreshold;

   protected RandomStringSourceBase(int minLength, int maxLength, int? nullCreationThreshold = null, params char[] allowedChars) {
      _minLength = minLength;
      _maxLength = maxLength;
      _allowedChars = allowedChars;
      _nullCreationThreshold = nullCreationThreshold;

      if (_nullCreationThreshold.HasValue)
         RandomNullEvaluator = new DefaultRandomNullEvaluator(_nullCreationThreshold.Value);
   }

   protected RandomStringSourceBase(int minLength, int maxLength, char minChar, char maxChar, int? nullCreationThreshold = null)
      : this(minLength, maxLength, nullCreationThreshold)
      => _allowedChars = Enumerable.Range(minChar, maxChar - minChar + 1)
         .Select(i => (char) i)
         .ToArray();

   public RandomStringSourceBase<T> SetMinMaxLength(int minLength, int maxLength) {
      _minLength = minLength;
      _maxLength = maxLength;
      return this;
   }

   public RandomStringSourceBase<T> SetAllowedChars(params char[] allowedChars) {
      _allowedChars = allowedChars;
      return this;
   }

   

   protected override T GetNextValue(IGenerationContext? context) {
      if (_nullCreationThreshold.HasValue) {
         if (RandomNullEvaluator.ShouldNextValueReturnNull())
            return (T) (object) null!;
      }

      var result = Enumerable.Range(0, Random.Next(_minLength, _maxLength + 1))
            .Select(x => _allowedChars[Random.Next(0, _allowedChars.Length - 1)])
            .Aggregate(new StringBuilder(), (builder, c) => builder.Append(c))
            .ToString();

      return (T) (object) result;
   }
}

/// <summary>
/// Create a random string source. 
/// Default: char set between 65 and 123, min length 5 and max length 10
/// </summary>
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

/// <summary>
/// Create a nullable random string source. 
/// Default: char set between 65 and 123, min length 5 and max length 10
/// </summary>
/// <seealso cref="AutoPocoGlobalSettings"/>
public class NullableRandomStringSource : RandomStringSourceBase<string?> {
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