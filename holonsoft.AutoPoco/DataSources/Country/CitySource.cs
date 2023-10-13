using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.DataSources.Base;

namespace holonsoft.AutoPoco.DataSources.Country;
public abstract class CitySourceBase(int? nullCreationThreshold = null) : FixedArrayWithStringsSourceBase(nullCreationThreshold) {

   protected override string[] Data => _cities;

   private static readonly string[] _cities = {
    "Shanghai",
    "Beijing",
    "Karachi",
    "Istanbul",
    "Dhaka",
    "Tokyo",
    "Moscow",
    "Manila",
    "Tianjin",
    "Mumbai",
    "Lahore",
    "Shenzhen",
    "Bangalore",
    "Seoul",
    "Jakarta",
    "Kinshasa",
    "Cairo",
    "Mexico City",
    "Lima",
    "New York City",
    "London",
    "Bogota",
    "Bangkok",
    "Ho Chi Minh City",
    "Kuala Lumpur",
    "Chennai",
    "Taipei",
    "Rio de Janeiro",
    "Hyderabad",
    "Sao Paulo",
    "Baghdad",
    "Santiago",
    "Wuhan",
    "Nairobi",
    "Guangzhou",
    "Los Angeles",
    "Kolkata",
    "Buenos Aires",
    "Shijiazhuang",
    "Nanchang",
    "Xi'an",
    "San Francisco",
    "Riyadh",
    "Singapore",
    "Berlin",
    "Sydney",
    "Toronto",
    "New Delhi",
    "Alexandria",
    "Abidjan",
    "Rome",
    "Dar es Salaam",
    "Algiers",
    "Yangon",
    "Jinan",
    "Hangzhou",
    "Kabul",
    "Casablanca",
    "Beirut",
    "Ankara",
    "Accra",
    "Birmingham",
    "Vienna",
    "Lusaka",
    "Luxembourg City",
    "Bishkek",
    "Tripoli",
    "Freetown",
    "Bamako",
    "Addis Ababa",
    "Monrovia",
    "Brazzaville",
    "Antananarivo",
    "Mogadishu",
    "Ulaanbaatar",
    "Nouakchott",
    "Hargeisa",
    "Asmara",
    "Podgorica",
    "Andorra la Vella",
    "Sao Tome",
    "Monaco",
    "Vatican City",
    "Kingstown",
    "Basseterre",
    "Saint John's",
    "Valletta",
    "Majuro"
   };
}

/// <summary>
///   The 100 biggest cities (per 2021)
/// </summary>
public class CitySource : CitySourceBase {
   public CitySource() : base() { }
}

/// <summary>
///  The 100 biggest cities (per 2021)
///  Result can be NULL, too 
/// </summary>
public class NullableCitySource : CitySourceBase {
   public NullableCitySource() : base(AutoPocoGlobalSettings.NullCreationThreshold) { }

   public NullableCitySource(int nullCreationThreshold) : base(nullCreationThreshold) { }
}
