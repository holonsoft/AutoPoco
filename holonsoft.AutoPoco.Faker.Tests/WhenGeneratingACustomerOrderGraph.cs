using Shouldly;
using Xunit;
using holonsoft.AutoPoco.DataSources.Base;
using holonsoft.AutoPoco.DataSources.Identifiers;
using holonsoft.AutoPoco.DataSources.Primitives;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Extensions;
using holonsoft.AutoPoco.Faker.DataSources;

namespace holonsoft.AutoPoco.Faker.Tests;

public class Product {
   public int Id { get; set; }
   public string Name { get; set; } = "";
   public string Ean { get; set; } = "";
   public string Asin { get; set; } = "";
   public decimal Price { get; set; }
   public string Currency { get; set; } = "";
}

public class Address {
   public string Street { get; set; } = "";
   public string PostalCode { get; set; } = "";
   public string City { get; set; } = "";
   public string Country { get; set; } = "";
}

public class OrderCustomer {
   public int Id { get; set; }
   public string FirstName { get; set; } = "";
   public string LastName { get; set; } = "";
   public string EmailAddress { get; set; } = "";
   public string PhoneNumber { get; set; } = "";
   public Address? ShippingAddress { get; set; }
}

public class OrderLine {
   public Product? Product { get; set; }
   public int Quantity { get; set; }
}

public class Order {
   public int Id { get; set; }
   public DateTime OrderedAt { get; set; }
   public OrderCustomer? Customer { get; set; }
   public List<OrderLine> Lines { get; set; } = [];
}

/// <summary>
///   The whole point of the two packages together, in one place: AutoPoco builds the graph and keeps it
///   repeatable, the core identifier sources give the numbers that have to survive a check digit, and Bogus
///   fills in the values a human would recognise. Doubles as the example in the README.
/// </summary>
public class WhenGeneratingACustomerOrderGraph {
   private const string _locale = "de";

   private static IGenerationSessionFactory CreateFactory(int seed)
      => AutoPocoContainer.Configure(x => {
         x.UseSeed(seed);

         x.Include<Product>()
            .Setup(p => p.Id).Use<IntegerIdSource>(1)
            .Setup(p => p.Name).Use<FakerProductNameSource>(_locale)
            .Setup(p => p.Ean).Use<Ean13Source>("40063")
            .Setup(p => p.Asin).Use<AsinSource>()
            .Setup(p => p.Price).Use<FakerPriceSource>(5m, 500m, 2, _locale)
            .Setup(p => p.Currency).Use<FakerCurrencyCodeSource>(_locale);

         x.Include<Address>()
            .Setup(a => a.Street).Use<FakerStreetAddressSource>(_locale)
            .Setup(a => a.PostalCode).Use<FakerPostalCodeSource>(_locale)
            .Setup(a => a.City).Use<FakerCitySource>(_locale)
            .Setup(a => a.Country).Use<FakerCountrySource>(_locale);

         x.Include<OrderCustomer>()
            .Setup(c => c.Id).Use<IntegerIdSource>(1)
            .Setup(c => c.FirstName).Use<FakerFirstNameSource>(_locale)
            .Setup(c => c.LastName).Use<FakerLastNameSource>(_locale)
            .Setup(c => c.EmailAddress).Use<FakerEmailAddressSource>(_locale)
            .Setup(c => c.PhoneNumber).Use<FakerPhoneNumberSource>(_locale)
            .Setup(c => c.ShippingAddress).Use<AutoSource<Address>>();

         x.Include<OrderLine>()
            .Setup(l => l.Product).Use<AutoSource<Product>>()
            .Setup(l => l.Quantity).Use<IntegerSource>(1, 5);

         x.Include<Order>()
            .Setup(o => o.Id).Use<IntegerIdSource>(1)
            .Setup(o => o.OrderedAt).Use<FakerDateTimeSource>(new DateTime(2024, 1, 1), new DateTime(2025, 12, 31), _locale)
            .Setup(o => o.Customer).Use<AutoSource<OrderCustomer>>()
            .Setup(o => o.Lines).Collection(1, 4);
      });

   private static List<Order> Orders(int seed, int count = 25)
      => CreateFactory(seed).CreateSession().Collection<Order>(count).ToList();

   [Fact]
   public void TheWholeGraphIsFilled() {
      foreach (var order in Orders(4711)) {
         order.Id.ShouldBeGreaterThan(0);
         order.OrderedAt.ShouldBeInRange(new DateTime(2024, 1, 1), new DateTime(2025, 12, 31));

         var customer = order.Customer.ShouldNotBeNull();
         customer.FirstName.ShouldNotBeNullOrWhiteSpace();
         customer.LastName.ShouldNotBeNullOrWhiteSpace();
         customer.EmailAddress.ShouldContain("@");
         customer.PhoneNumber.ShouldNotBeNullOrWhiteSpace();

         var address = customer.ShippingAddress.ShouldNotBeNull();
         address.Street.ShouldNotBeNullOrWhiteSpace();
         address.PostalCode.ShouldNotBeNullOrWhiteSpace();
         address.City.ShouldNotBeNullOrWhiteSpace();
         address.Country.ShouldNotBeNullOrWhiteSpace();

         order.Lines.Count.ShouldBeInRange(1, 4);

         foreach (var line in order.Lines) {
            line.Quantity.ShouldBeInRange(1, 5);

            var product = line.Product.ShouldNotBeNull();
            product.Name.ShouldNotBeNullOrWhiteSpace();
            product.Price.ShouldBeInRange(5m, 500m);
            product.Currency.Length.ShouldBe(3);
            product.Ean.ShouldStartWith("40063");
            IsValidGtin(product.Ean).ShouldBeTrue($"'{product.Ean}' is not a valid EAN-13");
            product.Asin.Length.ShouldBe(10);
            product.Asin[0].ShouldBe('B');
         }
      }
   }

   /// <summary>
   ///   The whole graph, identifiers and faker values together, comes back the same for the same seed. This
   ///   is what makes a shared developer database reproducible.
   /// </summary>
   [Fact]
   public void TheWholeGraphIsRepeatableForOneSeed() {
      Describe(Orders(4711)).ShouldBe(Describe(Orders(4711)));
   }

   [Fact]
   public void ADifferentSeedGivesADifferentDatabase()
      => Describe(Orders(1)).ShouldNotBe(Describe(Orders(2)));

   /// <summary>
   ///   The Faker sources are independent of each other, so an email address does not contain the name of the
   ///   customer it sits on. Where the parts have to match, build the member from the finished object at
   ///   generation time. This is the recipe the README shows, kept here so it stays true.
   /// </summary>
   [Fact]
   public void AMemberCanBeBuiltFromTheOtherMembersOfTheSameObject() {
      var customers = CreateFactory(4711).CreateSession()
         .Collection<OrderCustomer>(25, c =>
            c.Impose(x => x.EmailAddress, (_, x) => $"{x.FirstName}.{x.LastName}@example.com".ToLowerInvariant()))
         .ToList();

      customers.ShouldAllBe(c => c.EmailAddress.EndsWith("@example.com"));

      foreach (var customer in customers)
         customer.EmailAddress.ShouldBe($"{customer.FirstName}.{customer.LastName}@example.com".ToLowerInvariant());
   }

   private static List<string> Describe(IEnumerable<Order> orders)
      => [.. orders.Select(o => string.Join("|",
         o.Id,
         o.OrderedAt.ToString("O"),
         o.Customer?.FirstName,
         o.Customer?.LastName,
         o.Customer?.EmailAddress,
         o.Customer?.ShippingAddress?.City,
         string.Join(";", o.Lines.Select(l => $"{l.Product?.Ean}x{l.Quantity}@{l.Product?.Price}"))))];

   /// <summary>
   ///   The same independent GS1 check, written out again so this file stands on its own as an example.
   /// </summary>
   private static bool IsValidGtin(string value) {
      if (value.Length != 13)
         return false;

      var sum = 0;
      for (var i = 0; i < 13; i++)
         sum += (value[i] - '0') * ((13 - 1 - i) % 2 == 0 ? 1 : 3);

      return sum % 10 == 0;
   }
}
