// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueSourceTests.cs" company="">
//   
// </copyright>
// <summary>
//   The value source tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Primitives;

/// <summary>
///   The value source tests.
/// </summary>

public class ValueSourceTests {
   /// <summary>
   ///   The setup.
   /// </summary>
   public ValueSourceTests() => _source = new ValueSource<int>(10);

   /// <summary>
   ///   The source.
   /// </summary>
   private readonly ValueSource<int> _source;

   /// <summary>
   ///   The next_ returns value.
   /// </summary>
   [Fact]
   public void NextReturnsValue()
      => _source.InternalNext(null).ShouldBe(10);

   [Fact]
   public void NextReturnsTheSameValueEveryTime()
      => Enumerable.Range(0, 5).Select(_ => _source.InternalNext(null)).ShouldAllBe(x => (int) x == 10);

   [Fact]
   public void ReferenceValuesAreReturnedAsIs() {
      var value = new object();
      new ValueSource<object>(value).InternalNext(null).ShouldBeSameAs(value);
   }

   [Fact]
   public void NullIsRejectedAtConstructionTime() {
      Action act = () => new ValueSource<string>(null!);
      Should.Throw<ArgumentNullException>(act).ParamName.ShouldBe("value");
   }
}