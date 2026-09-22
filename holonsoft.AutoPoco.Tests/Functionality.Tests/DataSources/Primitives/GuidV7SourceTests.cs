using System.Globalization;
using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

public class GuidV7SourceTests : TestBase {
   /// <summary>
   ///   The instant a version 7 UUID encodes, read from its first 48 bits.
   /// </summary>
   private static DateTime EncodedTimestamp(Guid value) {
      var hex = value.ToString("N");
      var milliseconds = long.Parse(hex[..12], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
      return DateTime.UnixEpoch.AddMilliseconds(milliseconds);
   }

   [Fact]
   public void EveryUuidCarriesTheVersionAndVariantBits() {
      var source = new GuidV7Source();

      for (var i = 0; i < 300; i++) {
         var hex = source.Next(null).ToString("N");
         hex[12].ShouldBe('7', $"'{hex}' does not carry version 7");
         "89ab".ShouldContain(hex[16], $"'{hex}' does not carry the RFC variant");
      }
   }

   [Fact]
   public void TheTimestampStaysInsideTheDefaultRange() {
      var source = new GuidV7Source();
      var min = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
      var max = new DateTime(2036, 1, 1, 0, 0, 0, DateTimeKind.Utc);

      for (var i = 0; i < 300; i++) {
         var timestamp = EncodedTimestamp(source.Next(null));
         timestamp.ShouldBeInRange(min, max);
      }
   }

   [Fact]
   public void APinnedTimestampIsEncodedExactly() {
      var instant = new DateTime(2026, 9, 22, 8, 30, 0, DateTimeKind.Utc);
      var source = new GuidV7Source(instant, instant);

      for (var i = 0; i < 20; i++)
         EncodedTimestamp(source.Next(null)).ShouldBe(instant);
   }

   [Fact]
   public void TheSameSeedGivesTheSameUuidsAndTheClockPlaysNoPart() {
      var first = new GuidV7Source();
      var second = new GuidV7Source();

      var fromFirst = Enumerable.Range(0, 20).Select(_ => first.Next(null)).ToList();
      var fromSecond = Enumerable.Range(0, 20).Select(_ => second.Next(null)).ToList();

      fromFirst.ShouldBe(fromSecond);
   }

   [Fact]
   public void ReversedBoundsThrow()
      => Should.Throw<ArgumentOutOfRangeException>(() => new GuidV7Source(
         new DateTime(2030, 1, 1, 0, 0, 0, DateTimeKind.Utc),
         new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc)));

   [Fact]
   public void ABoundBeforeTheUnixEpochThrows()
      => Should.Throw<ArgumentOutOfRangeException>(() => new GuidV7Source(
         new DateTime(1969, 12, 31, 0, 0, 0, DateTimeKind.Utc),
         new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc)));

   [Fact]
   public void NextReturnsStableUuidListInTermsOfTestability()
      => NextReturnsStableElementListInTermsOfTestability(
         new GuidV7Source(), new Guid[] { new Guid("017cba51-dadc-7f43-9473-6db153f86017"), new Guid("01842887-1fdc-7262-ace6-7a11a8307de9"), new Guid("0118f0ff-75f6-74a3-9a9a-6db2da92886e"), new Guid("013424f3-3fb6-7b6a-9e05-20d0c3dd2ec6"), new Guid("01db5c0b-34c5-750a-a05d-561089df5262"), new Guid("01d1e3b3-1e95-72fb-84ed-e31fb8d981cc"), new Guid("00fdaa34-db84-7cb8-838a-32d9fc373cc6"), new Guid("0170cc59-365b-7609-91cd-b75ab41bba83"), new Guid("01b66c1f-ad10-762e-9a7e-57aefd81bc30"), new Guid("0188ed8c-28eb-7de0-ba90-65215fdcf816") });

   [Fact]
   public void NextReturnsStableUuidListInTermsOfTestabilityAndListCanContainNull()
      => NextReturnsStableElementListInTermsOfTestability(
         new NullableGuidV7Source()!, new Guid?[] { new Guid("017cba51-dadc-7f43-9473-6db153f86017"), new Guid("01842887-1fdc-7262-ace6-7a11a8307de9"), new Guid("0118f0ff-75f6-74a3-9a9a-6db2da92886e"), new Guid("013424f3-3fb6-7b6a-9e05-20d0c3dd2ec6"), new Guid("01db5c0b-34c5-750a-a05d-561089df5262"), new Guid("01d1e3b3-1e95-72fb-84ed-e31fb8d981cc"), new Guid("00fdaa34-db84-7cb8-838a-32d9fc373cc6"), new Guid("0170cc59-365b-7609-91cd-b75ab41bba83"), new Guid("01b66c1f-ad10-762e-9a7e-57aefd81bc30"), new Guid("0188ed8c-28eb-7de0-ba90-65215fdcf816"), new Guid("01237cd7-e47f-7ff6-8c95-37b6fbbf5a7d"), new Guid("0150f3b3-0687-702a-933c-7a44a1431ba8"), new Guid("010a0e07-34b0-7bfe-ac93-0f2e7a50cc8e"), new Guid("013352b5-4ae7-71d2-bd84-9e031f2803b0"), new Guid("015ee48a-6eb1-7144-bf5c-ac9e107330bf"), new Guid("01a7db01-ac10-7098-80ae-bf0f2e67d901"), new Guid("01187fbd-e93f-7508-8c61-7b9c6da569e0"), new Guid("01e0caae-0e0b-7a2f-8607-0997b96cdd93"), new Guid("00e93f75-2beb-7b81-b26b-27c46213683b"), new Guid("016804ba-5296-770a-8260-84285fcbc08c"), new Guid("01ac3674-6257-7567-a05f-5305254d9600"), new Guid("00fc2567-64f9-7673-b917-e617c855ebaa"), new Guid("01c7df66-6ef0-7fe0-acdc-f2798328192c"), new Guid("012debba-db79-7490-902d-2bf7285157e6"), null, new Guid("0108b771-927d-7cf1-af28-f0d7acf237a9"), new Guid("019d8e3e-900a-77d9-ba2e-16ee3df8661e"), null, new Guid("0116033b-2200-798b-a71f-d7f01129d9cc"), new Guid("0123ce09-ff7f-73be-8021-5c0d6f93504d") });

   [Fact]
   public void ANullableSourceReturnsNullsAndOtherwiseVersionSevenUuids() {
      var source = new NullableGuidV7Source();
      source.SetNullCreationThreshold(50);
      var values = Enumerable.Range(0, 500).Select(_ => source.Next(null)).ToList();

      values.ShouldContain(v => v == null);
      values.ShouldContain(v => v != null);

      foreach (var value in values.Where(v => v is not null))
         value!.Value.ToString("N")[12].ShouldBe('7');
   }

   [Fact]
   public void ANonNullableSourceNeverReturnsEmpty()
      => Enumerable.Range(0, 500).Select(_ => new GuidV7Source().Next(null)).ShouldAllBe(v => v != Guid.Empty);
}
