﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LastNameSource.cs" company="AutoPoco">
//   Microsoft Public License (Ms-PL)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.DataSources.Base;

namespace holonsoft.AutoPoco.DataSources.Business;

/// <summary>
///   The last name source.
/// </summary>
public abstract class LastNameSourceBase(int? nullCreationThreshold = null) : FixedArrayWithStringsSourceBase(nullCreationThreshold) {

   protected override string[] Data => _lastNames;

   private static readonly string[] _lastNames =
   {
    "Smith",
    "Johnson",
    "Brown",
    "Davis",
    "Miller",
    "Wilson",
    "Moore",
    "Taylor",
    "Anderson",
    "Harris",
    "Clark",
    "Young",
    "Martin",
    "Walker",
    "White",
    "Lewis",
    "Hall",
    "Allen",
    "Adams",
    "Scott",
    "Baker",
    "Carter",
    "Turner",
    "Thomas",
    "Parker",
    "Roberts",
    "Johnson",
    "Wright",
    "King",
    "Hill",
    "Green",
    "Evans",
    "Lee",
    "Murphy",
    "Campbell",
    "Garcia",
    "Rodriguez",
    "Martinez",
    "Hernandez",
    "Lopez",
    "Gonzalez",
    "Perez",
    "Sanchez",
    "Ramirez",
    "Torres",
    "Flores",
    "Rivera",
    "Mendoza",
    "Gomez",
    "Diaz",
    "Reyes",
    "Stewart",
    "Morris",
    "Morgan",
    "Rogers",
    "Peterson",
    "Cooper",
    "Reed",
    "Bailey",
    "Bell",
    "Murphy",
    "Bennett",
    "Cox",
    "James",
    "Hughes",
    "Price",
    "Long",
    "Harrison",
    "Howard",
    "Richardson",
    "Butler",
    "Foster",
    "Sanders",
    "Perry",
    "Simmons",
    "Stevens",
    "Coleman",
    "Powell",
    "Patterson",
    "Jordan",
    "Watkins",
    "Lynch",
    "Mason",
    "Harper",
    "George",
    "Mills",
    "Woods",
    "Dixon",
    "Fox",
    "Rose",
    "Wells",
    "Carpenter",
    "Knight",
    "Reyes",
    "Warren",
    "Ferguson",
    "Dean",
    "Edwards",
    "Gordon",
    "Pierce",
    "Harrison",
    "Hunt",
    "Scott",
    "Sullivan",
    "Russell",
    "Ortiz",
    "Jenkins",
  };
}

public class LastNameSource : LastNameSourceBase { }

public class NullableLastNameSource : LastNameSourceBase {
   public NullableLastNameSource() : base(AutoPocoGlobalSettings.NullCreationThreshold) { }

   public NullableLastNameSource(int nullCreationThreshold) : base(nullCreationThreshold) { }
}