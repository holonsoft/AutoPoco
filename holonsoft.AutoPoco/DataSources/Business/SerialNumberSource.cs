using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.DataSources.Primitives;

namespace holonsoft.AutoPoco.DataSources.Business;

/// <summary>
///   Generates a serial number from a pattern. The default <c>****-****-****-****</c> gives four blocks of
///   four characters, something like <c>8HK2-P0QF-1Z7A-M4CD</c>.
/// </summary>
/// <remarks>
///   A serial number follows no standard, every manufacturer invents their own scheme, so this source makes
///   no claim of validity beyond the pattern; give your own pattern when your devices look different, see
///   <see cref="PatternSourceBase" /> for the placeholders. The IMEI of a mobile device does follow a
///   standard, use <c>ImeiSource</c> for that.
/// </remarks>
public abstract class SerialNumberSourceBase(string? pattern, int? nullCreationThreshold = null)
   : PatternSourceBase(pattern ?? DefaultPattern, nullCreationThreshold) {
   /// <summary>
   ///   The pattern used when none is given: four blocks of four characters.
   /// </summary>
   public const string DefaultPattern = "****-****-****-****";
}

/// <summary>
///   A serial number from a pattern, four blocks of four characters unless another pattern is given.
/// </summary>
public class SerialNumberSource(string? pattern) : SerialNumberSourceBase(pattern) {
   public SerialNumberSource() : this(null) { }
}

/// <summary>
///   A serial number that returns null every now and then.
/// </summary>
public class NullableSerialNumberSource(string? pattern, int nullCreationThreshold)
   : SerialNumberSourceBase(pattern, nullCreationThreshold) {
   public NullableSerialNumberSource() : this(null, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableSerialNumberSource(string? pattern) : this(pattern, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableSerialNumberSource(int nullCreationThreshold) : this(null, nullCreationThreshold) { }
}
