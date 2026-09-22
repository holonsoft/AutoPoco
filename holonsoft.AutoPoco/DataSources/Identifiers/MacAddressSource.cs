using System.Text;
using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Identifiers;

/// <summary>
///   How a MAC address is written.
/// </summary>
public enum MacAddressFormat {
   /// <summary>
   ///   Six hex pairs separated by colons, e.g. <c>00:1A:2B:3C:4D:5E</c>. The most common form.
   /// </summary>
   Colons = 0,

   /// <summary>
   ///   Six hex pairs separated by hyphens, e.g. <c>00-1A-2B-3C-4D-5E</c>. The Windows form.
   /// </summary>
   Hyphens = 1,

   /// <summary>
   ///   Twelve hex digits without separators, e.g. <c>001A2B3C4D5E</c>.
   /// </summary>
   Bare = 2
}

/// <summary>
///   Generates a MAC address as upper case hex, a unicast address in the universally administered space.
/// </summary>
/// <remarks>
///   The two low bits of the first octet are cleared: bit zero says unicast, bit one says universally
///   administered, so the address looks like the burned-in address of a network card. The leading three
///   octets are drawn, so they can land on an OUI a real manufacturer owns; where that matters, use
///   <see cref="TestMacAddressSource" />, whose addresses sit in the locally administered space that the
///   IEEE keeps free of manufacturer assignments.
/// </remarks>
public abstract class MacAddressSourceBase : DataSourceBase<string> {
   private readonly MacAddressFormat _format;

   /// <summary>
   ///   Creates the source.
   /// </summary>
   /// <param name="format">How the address is written, colons unless another form is given.</param>
   /// <param name="nullCreationThreshold">Optional null creation threshold in percent.</param>
   /// <exception cref="ArgumentOutOfRangeException"><paramref name="format" /> is not a known form.</exception>
   protected MacAddressSourceBase(MacAddressFormat format = MacAddressFormat.Colons, int? nullCreationThreshold = null)
      : base(nullCreationThreshold) {
      if (!Enum.IsDefined(format))
         throw new ArgumentOutOfRangeException(nameof(format), format, "Unknown MAC address format.");

      _format = format;
   }

   protected override string GetNextValue(IGenerationContext? context) {
      if (NullCreationThreshold.HasValue) {
         if (RandomNullEvaluator.ShouldNextValueReturnNull())
            return null!;
      }

      Span<byte> octets = stackalloc byte[6];

      for (var i = 0; i < 6; i++)
         octets[i] = (byte) Random.Next(0, 256);

      octets[0] = AdjustFirstOctet(octets[0]);

      var separator = _format switch {
         MacAddressFormat.Colons => ":",
         MacAddressFormat.Hyphens => "-",
         _ => ""
      };

      var address = new StringBuilder(17);

      for (var i = 0; i < 6; i++) {
         if (i > 0)
            address.Append(separator);

         address.Append(octets[i].ToString("X2", System.Globalization.CultureInfo.InvariantCulture));
      }

      return address.ToString();
   }

   /// <summary>
   ///   Puts the drawn first octet into the address space of this source. The base class clears the
   ///   multicast and the locally administered bit, so the address is a universally administered unicast.
   /// </summary>
   private protected virtual byte AdjustFirstOctet(byte octet)
      => (byte) (octet & 0b1111_1100);
}

/// <summary>
///   A unicast MAC address in the universally administered space.
/// </summary>
public class MacAddressSource(MacAddressFormat format) : MacAddressSourceBase(format) {
   public MacAddressSource() : this(MacAddressFormat.Colons) { }
}

/// <summary>
///   A unicast MAC address that returns null every now and then.
/// </summary>
public class NullableMacAddressSource(MacAddressFormat format, int nullCreationThreshold)
   : MacAddressSourceBase(format, nullCreationThreshold) {
   public NullableMacAddressSource() : this(MacAddressFormat.Colons, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableMacAddressSource(MacAddressFormat format) : this(format, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableMacAddressSource(int nullCreationThreshold) : this(MacAddressFormat.Colons, nullCreationThreshold) { }
}

/// <summary>
///   A unicast MAC address in the locally administered space, recognisable as not belonging to any
///   manufacturer's network card.
/// </summary>
/// <remarks>
///   The second bit of the first octet is set, which IEEE 802 reserves for locally administered addresses:
///   no manufacturer OUI is ever assigned there, so the address provably names no vendor hardware. The
///   second hex digit of such an address is always a 2, 6, A or E. This is the same convention that
///   randomised Wi-Fi addresses and virtual machines use.
/// </remarks>
public abstract class TestMacAddressSourceBase(MacAddressFormat format = MacAddressFormat.Colons, int? nullCreationThreshold = null)
   : MacAddressSourceBase(format, nullCreationThreshold) {
   private protected override byte AdjustFirstOctet(byte octet)
      => (byte) ((octet & 0b1111_1100) | 0b0000_0010);
}

/// <summary>
///   A locally administered unicast MAC address, recognisable as test data.
/// </summary>
public class TestMacAddressSource(MacAddressFormat format) : TestMacAddressSourceBase(format) {
   public TestMacAddressSource() : this(MacAddressFormat.Colons) { }
}

/// <summary>
///   A locally administered unicast MAC address that returns null every now and then.
/// </summary>
public class NullableTestMacAddressSource(MacAddressFormat format, int nullCreationThreshold)
   : TestMacAddressSourceBase(format, nullCreationThreshold) {
   public NullableTestMacAddressSource() : this(MacAddressFormat.Colons, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableTestMacAddressSource(MacAddressFormat format) : this(format, AutoPocoDefaults.NullCreationThreshold) { }

   public NullableTestMacAddressSource(int nullCreationThreshold) : this(MacAddressFormat.Colons, nullCreationThreshold) { }
}
