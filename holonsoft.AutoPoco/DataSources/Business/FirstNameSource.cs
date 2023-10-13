
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.DataSources.Base;

namespace holonsoft.AutoPoco.DataSources.Business;

public abstract class FirstNameSourceBase(int? nullCreationThreshold = null) : FixedArrayWithStringsSourceBase(nullCreationThreshold) {

   protected override string[] Data => _firstNames;

   private static readonly string[] _firstNames =
   {
      "Alexa", "Claire", "Lucy", "Makayla", "Violet", "Aria", "Scarlet", "Maya", "Sophie", "Ariana",
      "Michael", "David", "Christopher", "Andrew", "John", "Matthew", "Joseph", "Anthony", "Robert", "William",
      "Sophia", "Olivia", "Emma", "Ava", "Isabella", "Mia", "Charlotte", "Amelia", "Harper", "Evelyn",
      "Ethan", "Nicholas", "Daniel", "Tyler", "James", "Alexander", "Benjamin", "Samuel", "Ryan", "Johnathan",
      "Abigail", "Emily", "Elizabeth", "Sofia", "Ella", "Madison", "Grace", "Avery", "Scarlett", "Victoria",
      "Nathan", "Daniel", "Luke", "Benjamin", "Owen", "Carter", "Wyatt", "Isaac", "Landon", "Jackson",
      "Chloe", "Camila", "Penelope", "Riley", "Layla", "Lila", "Nora", "Zoey", "Mila", "Hannah",
      "Charles", "Robert", "Henry", "George", "Edward", "Thomas", "Arthur", "Matthew", "William", "James",
      "Lily", "Addison", "Ellie", "Aubrey", "Leah", "Natalie", "Samantha", "Brooklyn", "Zoe", "Stella",
      "Joseph", "David", "Daniel", "Brian", "Kevin", "Paul", "George", "Jason", "Scott", "Kenneth",
      "Eric", "Mark", "Steven", "Richard", "Timothy"
   };
}

public class FirstNameSource : FirstNameSourceBase { }

public class NullableFirstNameSource : FirstNameSourceBase {
   public NullableFirstNameSource() : base(AutoPocoGlobalSettings.NullCreationThreshold) { }

   public NullableFirstNameSource(int nullCreationThreshold) : base(nullCreationThreshold) { }
}