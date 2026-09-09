using System.Drawing;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class ColorSourceTests : TestBase {
   [Fact]
   public void NextReturnsStableColorListInTermsOfTestability() {
      var source = new ColorSource();

      NextReturnsStableElementListInTermsOfTestability(source, new Color[] { Color.FromArgb(255, 220, 239, 67), Color.FromArgb(255, 148, 115, 109), Color.FromArgb(255, 177, 83, 248), Color.FromArgb(255, 96, 23, 220), Color.FromArgb(255, 194, 98, 44), Color.FromArgb(255, 230, 122, 17), Color.FromArgb(255, 168, 48, 125), Color.FromArgb(255, 233, 70, 67), Color.FromArgb(255, 110, 27, 246), Color.FromArgb(255, 148, 163, 90) });
   }

   [Fact]
   public void NextReturnsStableColorListInTermsOfTestabilityAndListCanContainNull() {
      var source = new NullableColorSource();

      NextReturnsStableElementListInTermsOfTestability(source, new Color?[] { Color.FromArgb(255, 220, 239, 67), Color.FromArgb(255, 148, 115, 109), Color.FromArgb(255, 177, 83, 248), Color.FromArgb(255, 96, 23, 220), Color.FromArgb(255, 194, 98, 44), Color.FromArgb(255, 230, 122, 17), Color.FromArgb(255, 168, 48, 125), Color.FromArgb(255, 233, 70, 67), Color.FromArgb(255, 110, 27, 246), Color.FromArgb(255, 148, 163, 90) });
   }
}
