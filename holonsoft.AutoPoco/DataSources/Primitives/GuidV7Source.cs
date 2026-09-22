using holonsoft.AutoPoco.Engine;
using holonsoft.AutoPoco.Engine.Interfaces;

namespace holonsoft.AutoPoco.DataSources.Primitives;

/// <summary>
///   Generates a version 7 UUID per RFC 9562: 48 bits of millisecond timestamp, the version and variant
///   bits, and 74 random bits, all drawn from the AutoPoco stream instead of the clock.
/// </summary>
/// <remarks>
///   The timestamp is drawn between the two bounds, the last day of 2035 as the default upper one, so the
///   same seed gives the same UUIDs on every run and no value depends on when the test runs. That is a
///   deliberate difference from <c>Guid.CreateVersion7()</c>: these UUIDs decode to a plausible
///   instant and carry correct version and variant bits, but they are not monotonic and do not encode the
///   current time. Use <see cref="GuidSource" /> when any random Guid does the job and the version bits do
///   not matter.
/// </remarks>
public abstract class GuidV7SourceBase<T> : DataSourceBase<T> {
   private static readonly DateTime _defaultMinTimestamp = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
   private static readonly DateTime _defaultMaxTimestamp = new(2035, 12, 31, 23, 59, 59, DateTimeKind.Utc);

   private readonly long _minMilliseconds;
   private readonly long _maxMilliseconds;

   /// <summary>
   ///   Creates the source.
   /// </summary>
   /// <param name="minTimestamp">Lower bound of the encoded instant, inclusive, treated as UTC.</param>
   /// <param name="maxTimestamp">Upper bound of the encoded instant, inclusive, treated as UTC.</param>
   /// <exception cref="ArgumentOutOfRangeException">
   ///   The bounds are reversed, or lie outside what 48 bits of Unix milliseconds can hold.
   /// </exception>
   protected GuidV7SourceBase(DateTime? minTimestamp = null, DateTime? maxTimestamp = null) {
      var min = minTimestamp ?? _defaultMinTimestamp;
      var max = maxTimestamp ?? _defaultMaxTimestamp;

      if (min < DateTime.UnixEpoch)
         throw new ArgumentOutOfRangeException(nameof(minTimestamp), min, "A version 7 UUID timestamp starts at the Unix epoch.");

      if (max < min)
         throw new ArgumentOutOfRangeException(nameof(maxTimestamp), max, $"The maximum timestamp must not be before the minimum timestamp ({min:O}).");

      _minMilliseconds = (min.Ticks - DateTime.UnixEpoch.Ticks) / TimeSpan.TicksPerMillisecond;
      _maxMilliseconds = (max.Ticks - DateTime.UnixEpoch.Ticks) / TimeSpan.TicksPerMillisecond;

      // 48 bits of milliseconds reach the year 10889, a DateTime cannot exceed them
   }

   protected override T GetNextValue(IGenerationContext? context) {
      var milliseconds = _minMilliseconds + Random.NextInt64(0, _maxMilliseconds - _minMilliseconds + 1);

      Span<byte> bytes = stackalloc byte[16];

      for (var i = 5; i >= 0; i--) {
         bytes[i] = (byte) (milliseconds & 0xFF);
         milliseconds >>= 8;
      }

      bytes[6] = (byte) (0x70 | Random.Next(0, 16));
      bytes[7] = (byte) Random.Next(0, 256);
      bytes[8] = (byte) (0x80 | Random.Next(0, 64));

      for (var i = 9; i < 16; i++)
         bytes[i] = (byte) Random.Next(0, 256);

      return (T) (object) new Guid(bytes, bigEndian: true);
   }
}

/// <summary>
///   A version 7 UUID whose timestamp is drawn from the seeded range instead of the clock.
/// </summary>
public class GuidV7Source(DateTime? minTimestamp, DateTime? maxTimestamp) : GuidV7SourceBase<Guid>(minTimestamp, maxTimestamp) {
   public GuidV7Source() : this(null, null) { }
}

/// <summary>
///   A version 7 UUID source that returns null every now and then.
/// </summary>
/// <seealso cref="holonsoft.AutoPoco.Configuration.AutoPocoDefaults" />
public class NullableGuidV7Source(DateTime? minTimestamp, DateTime? maxTimestamp) : GuidV7SourceBase<Guid?>(minTimestamp, maxTimestamp) {
   public NullableGuidV7Source() : this(null, null) { }
}
