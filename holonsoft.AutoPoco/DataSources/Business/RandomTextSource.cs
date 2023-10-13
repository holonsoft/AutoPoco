// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoremIpsumSource.cs" company="AutoPoco">
//   Microsoft Public License (Ms-PL)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Business;

public abstract class RandomTextSourceBase(int? nullCreationThreshold, int maxLengthOfText, int minParagraphCount, int maxParagraphCount, int minSentenceCount, int maxSentenceCount, params char[] allowedCharacters) : DataSourceBase<string> {
   protected override string GetNextValue(IGenerationContext? context) {
      if (nullCreationThreshold.HasValue) {
         if (RandomNullEvaluator.ShouldNextValueReturnNull())
            return null!;
      }

      StringBuilder builder = new();

      var paragraphCount = Random.Next(minParagraphCount, maxParagraphCount);

      for (var i = 0; i < paragraphCount; i++) {
         for (var j = minSentenceCount; j <= maxSentenceCount; j++) {
            var wordCount = Random.Next(6, 10);

            for (var k = 0; k < wordCount; k++) {
               var word = new string(Enumerable.Repeat(allowedCharacters, Random.Next(4, 11))
                           .Select(s => s[Random.Next(s.Length)]).ToArray());

               if (k == 0)
                  word = char.ToUpper(word[0]) + word[1..(word.Length - 2)];

               builder.Append(word);

               if (k < wordCount - 2)
                  builder.Append(' ');
            }

            builder.Append(". ");
         }

         builder.AppendLine();
      }

      var result = builder.ToString();

      if (result.Length > maxLengthOfText)
         return result[..(maxLengthOfText - 1)];

      return result;
   }
}

public class RandomTextSource(int maxLengthOfText, int minParagraphCount, int maxParagraphCount, int minSentenceCount, int maxSentenceCount, params char[] allowedCharacters)
   : RandomTextSourceBase(null, maxLengthOfText, minParagraphCount, maxParagraphCount, minSentenceCount, maxSentenceCount, allowedCharacters) {
   public RandomTextSource() : this(500, 3, 6, 3, 7, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray()) { }
}

public class NullableRandomTextSource(int? nullCreationThreshold, int maxLengthOfText, int minParagraphCount, int maxParagraphCount, int minSentenceCount, int maxSentenceCount, params char[] allowedCharacters)
   : RandomTextSourceBase(nullCreationThreshold, maxLengthOfText, minParagraphCount, maxParagraphCount, minSentenceCount, maxSentenceCount, allowedCharacters) {

   public NullableRandomTextSource() : this(AutoPocoGlobalSettings.NullCreationThreshold, 500, 3, 6, 3, 7, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray()) { }
}