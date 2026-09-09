using Shouldly;
using Xunit;
using holonsoft.AutoPoco.Tests.Common;
using holonsoft.AutoPoco.Util;

namespace holonsoft.AutoPoco.Tests.Functionality.Tests.Util;

public class SeedDerivationTests {
   [Fact]
   public void Fnv1aIsTheReferenceImplementation() {
      // reference values of 32 bit FNV-1a, as signed int
      SeedDerivation.Fnv1a("").ShouldBe(unchecked((int) 0x811c9dc5));
      SeedDerivation.Fnv1a("a").ShouldBe(unchecked((int) 0xe40c292c));
      SeedDerivation.Fnv1a("foobar").ShouldBe(unchecked((int) 0xbf9cf968));
   }

   [Fact]
   public void DerivationIsDeterministic() {
      SeedDerivation.ForMember(1337, typeof(SimpleUser), "FirstName").ShouldBe(SeedDerivation.ForMember(1337, typeof(SimpleUser), "FirstName"));
      SeedDerivation.ForType(1337, typeof(SimpleUser)).ShouldBe(SeedDerivation.ForType(1337, typeof(SimpleUser)));
   }

   [Fact]
   public void DifferentPlacesGetDifferentSeeds() {
      var firstName = SeedDerivation.ForMember(1337, typeof(SimpleUser), "FirstName");

      firstName.ShouldNotBe(SeedDerivation.ForMember(1337, typeof(SimpleUser), "LastName"));
      firstName.ShouldNotBe(SeedDerivation.ForMember(1337, typeof(SimpleUserRecord), "FirstName"));
      firstName.ShouldNotBe(SeedDerivation.ForMember(1338, typeof(SimpleUser), "FirstName"));
      firstName.ShouldNotBe(SeedDerivation.ForNullEvaluator(1337, typeof(SimpleUser), "FirstName"));
      firstName.ShouldNotBe(SeedDerivation.ForType(1337, typeof(SimpleUser)));
      SeedDerivation.ForMethodArgument(1337, typeof(SimpleUser), "SetPassword", 0)
         .ShouldNotBe(SeedDerivation.ForMethodArgument(1337, typeof(SimpleUser), "SetPassword", 1));
   }

   [Fact]
   public void Fnv1aRejectsNull()
      => Should.Throw<ArgumentNullException>(() => SeedDerivation.Fnv1a(null!));
}
