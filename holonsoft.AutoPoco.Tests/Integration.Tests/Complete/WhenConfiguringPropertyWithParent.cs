// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WhenConfiguringPropertyWithParent.cs" company="">
//   
// </copyright>
// <summary>
//   The when configuring property with parent.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Xunit;
using holonsoft.AutoPoco.DataSources.Base;
using holonsoft.AutoPoco.Extensions;
using holonsoft.AutoPoco.Tests.Common;

namespace holonsoft.AutoPoco.Tests.Integration.Tests.Complete;

/// <summary>
///   The when configuring property with parent.
/// </summary>

public class WhenConfiguringPropertyWithParent {
   /// <summary>
   ///   The property_ is_ set_ with_ null_ value_ if_ no_ parent_ exists.
   /// </summary>
   [Fact]
   public void PropertyIsSetWithNullValueIfNoParentExists() {
      var session = AutoPocoContainer.Configure(
        x => { x.Include<SimpleNode>().Setup(y => y.Children).Collection(1, 1); }).CreateSession();

      var node = session.Next<SimpleNode>();

      node.Parent.Should().BeNull();
   }

   /// <summary>
   ///   The property_ is_ set_ with_ parent_ value_ if_ parent_ exists.
   /// </summary>
   [Fact]
   public void PropertyIsSetWithParentValueIfParentExists() {
      // TODO: This test fails. Fix it --Praneeth
      var session = AutoPocoContainer.Configure(
        x => x.Include<SimpleNode>()
          .Setup(y => y.Children).Collection(1, 1)
          .Setup(y => y.Parent).Use<ParentSource<SimpleNode>>()).CreateSession();

      var node = session.Next<SimpleNode>();
      node.Children.First().Parent.Should().Be(node);
   }
}