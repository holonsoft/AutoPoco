using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Extensions;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Integration.Tests.Complete;

public class WhenUsingNumberSources {
   private static IGenerationSessionFactory CreateFactory()
      => AutoPocoContainer.Configure(x => {
         x.Include<NumericMembersClass>()
            // constructor arguments
            .Setup(c => c.Rating).Use<NumberSource<byte>>((byte) 1, (byte) 5)
            .Setup(c => c.Amount).Use<NumberSource<decimal>>(1m, 100m)
            // lambda configuration
            .Setup(c => c.BigId).Use<NumberSource<Int128>>(s => s.SetMinMax(long.MaxValue, Int128.MaxValue))
            .Setup(c => c.Ratio).Use<NumberSource<Half>>(s => s.SetMin(Half.Zero).SetMax(Half.One))
            // nullable member with an explicit threshold
            .Setup(c => c.OptionalCount).Use<NullableNumberSource<ushort>>((ushort) 10, (ushort) 20, 50)
            // a field
            .Setup(c => c.Field).Use<NumberSource<long>>(-10L, 10L);
      });

   private static List<NumericMembersClass> Generate(int count)
      => CreateFactory().CreateSession().Collection<NumericMembersClass>(count).ToList();

   [Fact]
   public void EveryMemberStaysInItsConfiguredRange() {
      var items = Generate(300);

      items.ShouldAllBe(x => x.Rating >= 1 && x.Rating <= 5);
      items.ShouldAllBe(x => x.Amount >= 1m && x.Amount <= 100m);
      items.ShouldAllBe(x => x.BigId >= long.MaxValue);
      items.ShouldAllBe(x => x.Ratio >= Half.Zero && x.Ratio <= Half.One);
      items.ShouldAllBe(x => x.OptionalCount == null || (x.OptionalCount >= 10 && x.OptionalCount <= 20));
      items.ShouldAllBe(x => x.Field >= -10 && x.Field <= 10);
   }

   [Fact]
   public void IntegerMembersReachBothEndsOfTheirRange() {
      var items = Generate(300);

      items.ShouldContain(x => x.Rating == 1);
      items.ShouldContain(x => x.Rating == 5);
      items.ShouldContain(x => x.Field == -10);
      items.ShouldContain(x => x.Field == 10);
      items.ShouldContain(x => x.BigId > ulong.MaxValue);
   }

   [Fact]
   public void NullableMemberIsNullAccordingToItsThreshold() {
      var nullCount = Generate(1000).Count(x => x.OptionalCount == null);

      nullCount.ShouldBeInRange(400, 600);
   }

   [Fact]
   public void GenerationTimeOverrideWorksWithANumberSource() {
      var items = CreateFactory().CreateSession()
         .List<NumericMembersClass>(100)
         .Source(x => x.Amount, new NumberSource<decimal>(1000m, 2000m))
         .Get()
         .ToList();

      items.ShouldAllBe(x => x.Amount >= 1000m && x.Amount <= 2000m);
   }
}
