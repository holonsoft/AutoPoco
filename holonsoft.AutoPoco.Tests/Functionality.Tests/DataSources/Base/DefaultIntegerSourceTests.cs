﻿using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources.Base;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.DataSources.Base;

public class DefaultIntegerSourceTests {
   [Fact]
   public void NextReturnsZero() {
      var source = new DefaultIntegerSource();
      var value = source.Next(null);
      value.Should().Be(0);
   }
}