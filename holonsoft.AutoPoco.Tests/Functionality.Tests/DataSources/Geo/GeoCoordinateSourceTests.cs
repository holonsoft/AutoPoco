using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Geo;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Geo;

public class GeoCoordinateSourceTests : TestBase {
   [Fact]
   public void ALatitudeStaysOnTheGlobe() {
      var source = new LatitudeSource();

      for (var i = 0; i < 300; i++)
         source.Next(null).ShouldBeInRange(-90, 90);
   }

   [Fact]
   public void ALongitudeStaysOnTheGlobe() {
      var source = new LongitudeSource();

      for (var i = 0; i < 300; i++)
         source.Next(null).ShouldBeInRange(-180, 180);
   }

   [Fact]
   public void ABoundingBoxIsRespected() {
      var latitudes = new LatitudeSource(47, 55);
      var longitudes = new LongitudeSource(5, 15);

      for (var i = 0; i < 300; i++) {
         latitudes.Next(null).ShouldBeInRange(47, 55);
         longitudes.Next(null).ShouldBeInRange(5, 15);
      }
   }

   [Fact]
   public void APinnedCoordinateIsExact() {
      new LatitudeSource(48.1371, 48.1371).Next(null).ShouldBe(48.1371);
      new LongitudeSource(11.5754, 11.5754).Next(null).ShouldBe(11.5754);
   }

   [Theory]
   [InlineData(-91, 0)]
   [InlineData(0, 91)]
   [InlineData(double.NaN, 0)]
   [InlineData(0, double.PositiveInfinity)]
   [InlineData(50, 40)]      // reversed
   public void ABrokenLatitudeRangeThrows(double min, double max)
      => Should.Throw<ArgumentOutOfRangeException>(() => new LatitudeSource(min, max));

   [Theory]
   [InlineData(-181, 0)]
   [InlineData(0, 181)]
   [InlineData(15, 5)]       // reversed
   public void ABrokenLongitudeRangeThrows(double min, double max)
      => Should.Throw<ArgumentOutOfRangeException>(() => new LongitudeSource(min, max));

   [Fact]
   public void NextReturnsStableLatitudeListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new LatitudeSource(), new double[] { 31.669814152620816D, 56.60525664808654D, 49.95233714608446D, 77.55660019739571D, -57.73951179844884D, 27.81041412257177D, -72.10171540745205D, -43.08707460818985D, -48.911967188936735D, 57.460836378945686D });

   [Fact]
   public void NextReturnsStableLongitudeListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new LongitudeSource(), new double[] { 63.33962830524163D, 113.21051329617308D, 99.90467429216892D, 155.11320039479142D, -115.47902359689768D, 55.62082824514354D, -144.2034308149041D, -86.1741492163797D, -97.82393437787347D, 114.92167275789137D });

   [Fact]
   public void ANullableSourceReturnsNullsAndOtherwiseCoordinates() {
      var source = new NullableLatitudeSource();
      source.SetNullCreationThreshold(50);
      var values = Enumerable.Range(0, 500).Select(_ => source.Next(null)).ToList();

      values.ShouldContain(v => v == null);
      values.ShouldContain(v => v != null);

      foreach (var value in values.Where(v => v is not null))
         value!.Value.ShouldBeInRange(-90, 90);
   }

   [Fact]
   public void ANonNullableSourceNeverReturnsNull()
      => Enumerable.Range(0, 200).Select(_ => new LongitudeSource().Next(null)).ShouldAllBe(v => v >= -180 && v <= 180);
}
