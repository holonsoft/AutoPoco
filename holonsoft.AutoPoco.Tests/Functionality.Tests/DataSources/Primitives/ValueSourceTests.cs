// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueSourceTests.cs" company="">
//   
// </copyright>
// <summary>
//   The value source tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
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
      => _source.InternalNext(null).Should().Be(10);
}