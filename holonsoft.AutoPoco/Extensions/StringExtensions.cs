using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.DataSources.Primitives;

namespace holonsoft.AutoPoco.Extensions;

public static class StringExtensions {
   extension<TPoco>(IEngineConfigurationTypeMemberBuilder<TPoco, string> memberConfig) {
      /// <summary>
      ///   Declares that this string member should have a random length between min and max
      /// </summary>
      public IEngineConfigurationTypeBuilder<TPoco> Random(int minLength, int maxLength)
         => memberConfig.Use<RandomStringSource>(minLength, maxLength);
   }
}
