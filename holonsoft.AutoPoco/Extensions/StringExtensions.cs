using holonsoft.AutoPoco.Configuration.Interfaces;
using holonsoft.AutoPoco.DataSources;

namespace holonsoft.AutoPoco.Extensions;

public static class StringExtensions {
   /// <summary>
   ///   Declares that this string member should have a random length between min and max
   /// </summary>
   public static IEngineConfigurationTypeBuilder<TPoco> Random<TPoco>(
     this IEngineConfigurationTypeMemberBuilder<TPoco, string> memberConfig, int minLength, int maxLength) 
      => memberConfig.Use<RandomStringSource>(minLength, maxLength);
}