using System.Drawing;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class ColorSourceTests : TestBase {
   [Fact]
   public void NextReturnsStableColorListInTermsOfTestability() {
      var source = new ColorSource();

      NextReturnsStableElementListInTermsOfTestability(source, new Color[] {
         Color.FromArgb(255, 53, 30, 80),
         Color.FromArgb(255, 237, 214, 244),
         Color.FromArgb(255, 244, 49, 98),
         Color.FromArgb(255, 74, 76, 72),
         Color.FromArgb(255, 192, 12, 215),
         Color.FromArgb(255, 3, 225, 238),
         Color.FromArgb(255, 221, 1, 118),
         Color.FromArgb(255, 220, 242, 27),
         Color.FromArgb(255, 187, 117, 112),
         Color.FromArgb(255, 26, 215, 215) });
   }

   [Fact]
   public void NextReturnsStableColorListInTermsOfTestabilityAndListCanContainNull() {
      var source = new NullableColorSource();

      NextReturnsStableElementListInTermsOfTestability(source, new Color?[] {
         Color.FromArgb(255, 53, 30, 80),
         null,
         Color.FromArgb(255, 237, 214, 244),
         Color.FromArgb(255, 244, 49, 98),
         Color.FromArgb(255, 74, 76, 72),
         Color.FromArgb(255, 192, 12, 215),
         Color.FromArgb(255, 3, 225, 238),
         Color.FromArgb(255, 221, 1, 118),
         Color.FromArgb(255, 220, 242, 27),
         Color.FromArgb(255, 187, 117, 112) });
   }
}
