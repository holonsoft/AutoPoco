// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmailAddressSource.cs" company="AutoPoco">
//   Microsoft Public License (Ms-PL)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources;

/// <summary>
///   The email address source.
/// </summary>
public class EmailAddressSource(string namePartPrefix, string domain) : DataSourceBase<string> {
   /// <summary>
   ///   The index.
   /// </summary>
   private int _index;

   public EmailAddressSource()
      : this("eg", "example.com") { }

   /// <summary>
   ///   The next.
   /// </summary>
   /// <param name="context">
   ///   The context.
   /// </param>
   /// <returns>
   ///   The <see cref="string" />.
   /// </returns>
   public override string Next(IGenerationContext? context) =>
      // TODO: See if first name/last name has been used in this context
      $"{namePartPrefix}{_index++}@{domain}";
}