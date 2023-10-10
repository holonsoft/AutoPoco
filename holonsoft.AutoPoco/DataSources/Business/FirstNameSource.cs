// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FirstNameSource.cs" company="AutoPoco">
//   Microsoft Public License (Ms-PL)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FirstNameSource.cs" company="AutoPoco">
//   Microsoft Public License (Ms-PL)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using holonsoft.AutoPoco.DataSources.Base;

namespace holonsoft.AutoPoco.DataSources.Business;

/// <summary>
///   The first name source.
/// </summary>
public class FirstNameSource : FixedStringArraySourceBase
{

    protected override string[] Data => _firstNames;

    private static readonly string[] _firstNames =
    {
    "Jack", "Thomas", "Oliver", "Joshua", "Harry", "Charlie", "Daniel", "William", "James", "Alfie", "Samuel", "George", "Megan",
    "Joseph", "Benjamin", "Ethan", "Lewis", "Mohammed", "Jake", "Dylan", "Jacob", "Ruby", "Olivia", "Grace", "Emily", "Jessica",
    "Chloe", "Lily", "Mia", "Lucy", "Amelia", "Evie", "Ella", "Katie", "Ellie", "Charlotte", "Summer", "Mohammed", "Hannah", "Ava",
    "Isabella", "Sophia", "Noah", "Liam", "Jackson", "Lucas", "Lukas", "Luna", "Aiden", "Elijah", "Harper", "Evelyn", "Evelin", "David",
    "Logan", "Sophie"
  };
}