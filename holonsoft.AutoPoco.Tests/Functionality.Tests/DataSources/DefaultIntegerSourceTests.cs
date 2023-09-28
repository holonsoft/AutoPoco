﻿using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources;

public class DefaultIntegerSourceTests {
   [Fact]
   public void NextReturnsZero() {
      var source = new DefaultIntegerSource();
      var value = source.Next(null);
      value.Should().Be(0);
   }
}