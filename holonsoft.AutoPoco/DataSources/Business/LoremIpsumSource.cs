using System.Text;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Properties;

namespace holonsoft.AutoPoco.DataSources.Business;

public abstract class LoremIpsumSourceBase(int count, int? nullCreationThreshold = null) : DataSourceBase<string> {

   protected override string GetNextValue(IGenerationContext? context) {
      if (nullCreationThreshold.HasValue) {
         if (RandomNullEvaluator.ShouldNextValueReturnNull())
            return null!;
      }

      var builder = new StringBuilder(Resources.LoremIpsum);

      for (var i = 1; i < count; i++) {
         builder.AppendFormat("{0}", Resources.LoremIpsum);

         if (i < (count - 1)) {
            builder.AppendLine();
            builder.AppendLine();
         }
      }

      return builder.ToString();
   }
}

public class LoremIpsumSource(int count) : LoremIpsumSourceBase(count) {
   public LoremIpsumSource() : this(1) { }
}

public class NullableLoremIpsumSource(int count, int nullCreationThreshold) : LoremIpsumSourceBase(count, nullCreationThreshold) {
   public NullableLoremIpsumSource() : this(1, AutoPocoGlobalSettings.NullCreationThreshold) { }
}