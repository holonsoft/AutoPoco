using System.Globalization;
using System.Text.RegularExpressions;
using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Identifiers;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Identifiers;

public partial class MacAddressSourceTests : TestBase {
   [GeneratedRegex("^([0-9A-F]{2}:){5}[0-9A-F]{2}$")]
   private static partial Regex ColonFormat();

   [GeneratedRegex("^([0-9A-F]{2}-){5}[0-9A-F]{2}$")]
   private static partial Regex HyphenFormat();

   [GeneratedRegex("^[0-9A-F]{12}$")]
   private static partial Regex BareFormat();

   private static int FirstOctet(string address)
      => int.Parse(address[..2], NumberStyles.HexNumber, CultureInfo.InvariantCulture);

   [Theory]
   [InlineData(MacAddressFormat.Colons)]
   [InlineData(MacAddressFormat.Hyphens)]
   [InlineData(MacAddressFormat.Bare)]
   public void EveryAddressMatchesItsFormat(MacAddressFormat format) {
      var source = new MacAddressSource(format);
      var pattern = format switch {
         MacAddressFormat.Colons => ColonFormat(),
         MacAddressFormat.Hyphens => HyphenFormat(),
         _ => BareFormat()
      };

      for (var i = 0; i < 300; i++) {
         var value = source.Next(null);
         pattern.IsMatch(value).ShouldBeTrue($"'{value}' does not match the {format} format");
      }
   }

   /// <summary>
   ///   The plain source produces universally administered unicast addresses, so the two low bits of the
   ///   first octet are clear, the way a burned-in address of a network card looks.
   /// </summary>
   [Fact]
   public void EveryAddressIsAUniversallyAdministeredUnicast() {
      var source = new MacAddressSource();

      for (var i = 0; i < 300; i++) {
         var value = source.Next(null);
         (FirstOctet(value) & 0b0000_0011).ShouldBe(0, $"'{value}' is not a universally administered unicast");
      }
   }

   /// <summary>
   ///   The test source sets the locally administered bit and keeps the unicast bit clear, so the second
   ///   hex digit is always a 2, 6, A or E and no manufacturer OUI can own the address.
   /// </summary>
   [Fact]
   public void EveryTestAddressIsALocallyAdministeredUnicast() {
      var source = new TestMacAddressSource();

      for (var i = 0; i < 300; i++) {
         var value = source.Next(null);
         (FirstOctet(value) & 0b0000_0011).ShouldBe(0b0000_0010, $"'{value}' is not a locally administered unicast");
         "26AE".ShouldContain(value[1], $"'{value}' does not look locally administered");
      }
   }

   [Fact]
   public void TheTestAddressesDifferFromThePlainOnesOnTheSameSeed() {
      var plain = new MacAddressSource();
      var test = new TestMacAddressSource();

      var fromPlain = Enumerable.Range(0, 20).Select(_ => plain.Next(null)).ToList();
      var fromTest = Enumerable.Range(0, 20).Select(_ => test.Next(null)).ToList();

      fromTest.ShouldNotBe(fromPlain);
   }

   [Fact]
   public void AnUnknownFormatThrows()
      => Should.Throw<ArgumentOutOfRangeException>(() => new MacAddressSource((MacAddressFormat) 99));

   [Fact]
   public void NextReturnsStableAddressListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new MacAddressSource(), new string[] { "DC:EF:43:94:73:6D", "B0:53:F8:60:17:DC", "C0:62:2C:E6:7A:11", "A8:30:7D:E9:46:43", "6C:1B:F6:94:A3:5A", "98:6D:B2:DA:92:88", "6C:B6:FB:6A:9E:05", "20:D0:C3:DD:2E:C6", "C4:95:0A:20:5D:56", "10:89:DF:52:62:C5" });

   [Fact]
   public void NextReturnsStableTestAddressListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new TestMacAddressSource(), new string[] { "DE:EF:43:94:73:6D", "B2:53:F8:60:17:DC", "C2:62:2C:E6:7A:11", "AA:30:7D:E9:46:43", "6E:1B:F6:94:A3:5A", "9A:6D:B2:DA:92:88", "6E:B6:FB:6A:9E:05", "22:D0:C3:DD:2E:C6", "C6:95:0A:20:5D:56", "12:89:DF:52:62:C5" });

   [Fact]
   public void NextReturnsStableAddressListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableMacAddressSource()!, new string?[] { "DC:EF:43:94:73:6D", "B0:53:F8:60:17:DC", "C0:62:2C:E6:7A:11", "A8:30:7D:E9:46:43", "6C:1B:F6:94:A3:5A", "98:6D:B2:DA:92:88", "6C:B6:FB:6A:9E:05", "20:D0:C3:DD:2E:C6", "C4:95:0A:20:5D:56", "10:89:DF:52:62:C5", "EC:95:F2:FB:44:ED", "E0:1F:B8:D9:81:CC", "84:9C:B8:03:8A:32", "D8:FC:37:3C:C6:5B", "14:09:D1:CD:B7:5A", "B4:1B:BA:83:BD:10", "C4:2E:5A:7E:57:AE", "FC:81:BC:30:EB:FD", "E0:FA:90:65:21:5F", "DC:F8:16:C3:7F:DF", "F4:0C:95:37:B6:FB", "BC:5A:7D:87:60:2A", "90:3C:7A:44:A1:43", "18:A8:B0:1B:FE:2C", null, "90:0F:2E:7A:50:CC", "8C:D1:E7:B1:D2:BD", null, "84:9E:03:1F:28:03", "B0:1C:67:D3:B1:71" });

   [Fact]
   public void ANullableSourceReturnsNullsAndOtherwiseValidAddresses() {
      var source = new NullableTestMacAddressSource(50);
      var values = Enumerable.Range(0, 500).Select(_ => source.Next(null)).ToList();

      values.ShouldContain(v => v == null);
      values.ShouldContain(v => v != null);

      foreach (var value in values.Where(v => v is not null))
         ColonFormat().IsMatch(value!).ShouldBeTrue($"'{value}' does not match the colon format");
   }

   [Fact]
   public void ANonNullableSourceNeverReturnsNull()
      => Enumerable.Range(0, 500).Select(_ => new MacAddressSource().Next(null)).ShouldAllBe(v => v != null);
}
