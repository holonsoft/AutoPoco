#if NET11_0_OR_GREATER
using System.Numerics;
using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Extensions;

namespace holonsoft.AutoPoco.Tests.Integration.Tests.Complete;

public class IeeeDecimalMembersClass {
   public Decimal32 Rate { get; set; }
   public Decimal64 Price { get; set; }
   public Decimal128 Balance { get; set; }
   public Decimal64? Discount { get; set; }
}

public class WhenUsingIeeeDecimalNumberSources {
   private static readonly Decimal32 MaxRate = Decimal32.One;
   private static readonly Decimal64 MinPrice = Decimal64.CreateChecked(1);
   private static readonly Decimal64 MaxPrice = Decimal64.CreateChecked(100);
   private static readonly Decimal128 MinBalance = Decimal128.CreateChecked(-1_000_000);
   private static readonly Decimal128 MaxBalance = Decimal128.CreateChecked(1_000_000);

   private static IGenerationSessionFactory CreateFactory()
      => AutoPocoContainer.Configure(x => {
         x.Include<IeeeDecimalMembersClass>()
            .Setup(c => c.Rate).Use<NumberSource<Decimal32>>(Decimal32.Zero, MaxRate)
            .Setup(c => c.Price).Use<NumberSource<Decimal64>>(s => s.SetMinMax(MinPrice, MaxPrice))
            .Setup(c => c.Balance).Use<NumberSource<Decimal128>>(MinBalance, MaxBalance)
            .Setup(c => c.Discount).Use<NullableNumberSource<Decimal64>>(Decimal64.Zero, Decimal64.One, 50);
      });

   private static List<IeeeDecimalMembersClass> Generate(int count)
      => CreateFactory().CreateSession().Collection<IeeeDecimalMembersClass>(count).ToList();

   [Fact]
   public void EveryMemberStaysInItsConfiguredRange() {
      var items = Generate(300);

      items.ShouldAllBe(x => x.Rate >= Decimal32.Zero && x.Rate <= MaxRate);
      items.ShouldAllBe(x => x.Price >= MinPrice && x.Price <= MaxPrice);
      items.ShouldAllBe(x => x.Balance >= MinBalance && x.Balance <= MaxBalance);
      items.ShouldAllBe(x => x.Discount == null || (x.Discount >= Decimal64.Zero && x.Discount <= Decimal64.One));
   }

   [Fact]
   public void NullableMemberIsNullAccordingToItsThreshold()
      => Generate(1000).Count(x => x.Discount == null).ShouldBeInRange(400, 600);

   [Fact]
   public void TheSameFactoryGivesTheSameValuesInEverySession() {
      var first = Generate(50);
      var second = Generate(50);

      first.Select(x => (x.Rate, x.Price, x.Balance, x.Discount))
         .ShouldBe(second.Select(x => (x.Rate, x.Price, x.Balance, x.Discount)));
   }
}
#endif
