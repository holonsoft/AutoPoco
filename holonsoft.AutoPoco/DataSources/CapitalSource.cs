using holonsoft.AutoPoco.DataSources.Base;

namespace holonsoft.AutoPoco.DataSources;

public class CapitalSource : FixedStringArraySourceBase {
   protected override string[] Data => _capitals;

   private static readonly string[] _capitals = {
       "Berlin",           // Germany
       "Paris",            // France
       "London",           // United Kingdom
       "Madrid",           // Spain
       "Rome",             // Italy
       "Lisbon",           // Portugal
       "Amsterdam",        // Netherlands
       "Brussels",         // Belgium
       "Vienna",           // Austria
       "Bern",             // Switzerland
       "Warsaw",           // Poland
       "Prague",           // Czech Republic
       "Budapest",         // Hungary
       "Copenhagen",       // Denmark
       "Oslo",             // Norway
       "Stockholm",        // Sweden
       "Helsinki",         // Finland
       "Reykjavik",        // Iceland
       "Dublin",           // Ireland
       "Athens",           // Greece
       "Ankara",           // Turkey
       "Moscow",           // Russia
       "Kyiv",             // Ukraine
       "Lisbon",           // Portugal
       "Bucharest",        // Romania
       "Sofia",            // Bulgaria
       "Athens",           // Greece
       "Ankara",           // Turkey
       "Cairo",            // Egypt
       "Rabat",            // Morocco
       "Tunis",            // Tunisia
       "Algiers",          // Algeria
       "Nairobi",          // Kenya
       "Addis Ababa",      // Ethiopia
       "Khartoum",         // Sudan
       "Cape Town",        // South Africa
       "Pretoria",         // South Africa
       "Accra",            // Ghana
       "Lagos",            // Nigeria
       "Nairobi",          // Kenya
       "Riyadh",           // Saudi Arabia
       "Doha",             // Qatar
       "Kuwait City",      // Kuwait
       "Manama",           // Bahrain
       "Muscat",           // Oman
       "Abu Dhabi",        // United Arab Emirates
       "Baghdad",          // Iraq
       "Tehran",           // Iran
       "Jerusalem",        // Israel
       "Amman",            // Jordan
       "Beirut",           // Lebanon
       "Damascus",         // Syria
       "Ankara",           // Turkey
       "Baku",             // Azerbaijan
       "Tbilisi",          // Georgia
       "Yerevan",          // Armenia
       "Bishkek",          // Kyrgyzstan
       "Tashkent",         // Uzbekistan
       "Ashgabat",         // Turkmenistan
       "Dushanbe",         // Tajikistan
       "Nur-Sultan",       // Kazakhstan
       "Astana",           // Kazakhstan
       "Ulaanbaatar",      // Mongolia
       "Beijing",          // China
       "Shanghai",         // China
       "Tokyo",            // Japan
       "Seoul",            // South Korea
       "Pyongyang",        // North Korea
       "Taipei",           // Taiwan
       "Hanoi",            // Vietnam
       "Bangkok",          // Thailand
       "Vientiane",        // Laos
       "Phnom Penh",       // Cambodia
       "Yangon",           // Myanmar
       "Kuala Lumpur",     // Malaysia
       "Singapore",        // Singapore
       "Jakarta",          // Indonesia
       "Manila",           // Philippines
       "Hanoi",            // Vietnam
       "Naypyidaw",        // Myanmar
       "Islamabad",        // Pakistan
       "New Delhi",        // India
       "Kathmandu",        // Nepal
       "Thimphu",          // Bhutan
       "Colombo",          // Sri Lanka
       "Dhaka",            // Bangladesh
       "Male",             // Maldives
       "Sri Jayawardenepura Kotte", // Sri Lanka
       "Kabul",            // Afghanistan
       "Tashkent",         // Uzbekistan
       "Ashgabat",         // Turkmenistan
       "Dushanbe",         // Tajikistan
       "Nur-Sultan",       // Kazakhstan
       "Astana",           // Kazakhstan
       "Ulaanbaatar",      // Mongolia
       "Beijing",          // China
       "Shanghai",         // China
       "Tokyo",            // Japan
       "Seoul",            // South Korea
       "Pyongyang",        // North Korea
       "Taipei",           // Taiwan
       "Hanoi",            // Vietnam
       "Bangkok",          // Thailand
       "Vientiane",        // Laos
       "Phnom Penh",       // Cambodia
       "Yangon",           // Myanmar
       "Kuala Lumpur",     // Malaysia
       "Singapore",        // Singapore
       "Jakarta",          // Indonesia
       "Manila",           // Philippines
       "Beirut",           // Lebanon
       "Damascus",         // Syria
       "Amman",            // Jordan
       "Jerusalem",        // Israel
       "Baghdad",          // Iraq
       "Kuwait City",      // Kuwait
       "Manama",           // Bahrain
       "Muscat",           // Oman
       "Abu Dhabi",        // United Arab Emirates
       "Riyadh",           // Saudi Arabia
       "Doha",             // Qatar
       "Cairo",            // Egypt
       "Tripoli",          // Libya
       "Tunis",            // Tunisia
       "Algiers",          // Algeria
       "Rabat",            // Morocco
       "Casablanca",       // Morocco
       "Rabat",            // Morocco
       "Nouakchott",       // Mauritania
       "Dakar",            // Senegal
       "Banjul",           // Gambia
       "Bissau",           // Guinea-Bissau
       "Conakry",          // Guinea
       "Freetown"          // Sierra Leone
   };
}
