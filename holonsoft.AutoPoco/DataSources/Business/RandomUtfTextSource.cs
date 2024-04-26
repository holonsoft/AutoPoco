using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.Engine.Interfaces;
using holonsoft.AutoPoco.Engine;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace holonsoft.AutoPoco.DataSources.Business;

public abstract class RandomUtfTextSourceBase(int? nullCreationThreshold, int maxLengthOfText, int minParagraphCount, int maxParagraphCount, int minSentenceCount, int maxSentenceCount,
   IReadOnlySet<UnicodeCategory>? mayExcludeCategories = null) : DataSourceBase<string> {
   protected override string GetNextValue(IGenerationContext? context) {
      if (nullCreationThreshold.HasValue) {
         if (RandomNullEvaluator.ShouldNextValueReturnNull())
            return null!;
      }

      var excludeCategories = mayExcludeCategories
         ?? new HashSet<UnicodeCategory>() { UnicodeCategory.Control, UnicodeCategory.OtherNotAssigned, UnicodeCategory.Surrogate, UnicodeCategory.LineSeparator, UnicodeCategory.PrivateUse };

      StringBuilder builder = new();
      var paragraphCount = Random.Next(minParagraphCount, maxParagraphCount);

      for (var i = 0; i < paragraphCount; i++) {
         for (var j = minSentenceCount; j <= maxSentenceCount; j++) {
            var wordCount = Random.Next(6, 10);

            for (var k = 0; k < wordCount; k++) {
               builder.Append(UnicodeBlock.GetRandomString(Random, Random.Next(4, 11), excludeCategories));

               if (k < wordCount - 2)
                  builder.Append(' ');
            }

            builder.Append(". ");
         }

         builder.AppendLine();
      }

      var result = builder.ToString();

      if (result.Length > maxLengthOfText)
         return result[..(maxLengthOfText - 1)];

      return result;
   }
}

public class RandomUtfTextSource(int maxLengthOfText, int minParagraphCount, int maxParagraphCount, int minSentenceCount, int maxSentenceCount, IReadOnlySet<UnicodeCategory>? mayExcludeCategories = null)
   : RandomUtfTextSourceBase(null, maxLengthOfText, minParagraphCount, maxParagraphCount, minSentenceCount, maxSentenceCount, mayExcludeCategories) {
   public RandomUtfTextSource() : this(500, 3, 6, 3, 7) { }
}

public class NullableRandomUtfTextSource(int? nullCreationThreshold, int maxLengthOfText, int minParagraphCount, int maxParagraphCount, int minSentenceCount, int maxSentenceCount, IReadOnlySet<UnicodeCategory>? mayExcludeCategories = null)
   : RandomUtfTextSourceBase(nullCreationThreshold, maxLengthOfText, minParagraphCount, maxParagraphCount, minSentenceCount, maxSentenceCount, mayExcludeCategories) {

   public NullableRandomUtfTextSource() : this(AutoPocoGlobalSettings.NullCreationThreshold, 500, 3, 6, 3, 7) { }
}

public record UnicodeBlock(int Start, int End, int CharCount, int ExistingChars) {
   private int GetRandomCharFromBlock(Random random, IReadOnlySet<UnicodeCategory> excludeCategories) {
      UnicodeCategory category;
      int result;
      do {
         result = random.Next(Start, End + 1);
         category = CharUnicodeInfo.GetUnicodeCategory(result);
      } while (excludeCategories.Contains(category) || result == 0);

      return result;
   }

   private static int GetRandomChar(Random random, IReadOnlySet<UnicodeCategory> excludeCategories)
      => _unicodeBlocks[random.Next(0, _unicodeBlocks.Length)].GetRandomCharFromBlock(random, excludeCategories);

   public static string GetRandomString(Random random, int length, IReadOnlySet<UnicodeCategory> excludeCategories) {
      var ints = new int[length];
      for (var i = 0; i < length; ++i)
         ints[i] = GetRandomChar(random, excludeCategories);

      var bytes = MemoryMarshal.Cast<int, byte>(ints);
      return Encoding.UTF32.GetString(bytes);
   }

   private static readonly UnicodeBlock[] _unicodeBlocks = [
      new UnicodeBlock(0x0000 , 0x007F , 128 , 128 ),
         new UnicodeBlock(0x0080 , 0x00FF , 128 , 128 ),
         new UnicodeBlock(0x0100 , 0x017F , 128 , 128 ),
         new UnicodeBlock(0x0180 , 0x024F , 208 , 208 ),
         new UnicodeBlock(0x0250 , 0x02AF , 96 , 96 ),
         new UnicodeBlock(0x02B0 , 0x02FF , 80 , 80 ),
         new UnicodeBlock(0x0300 , 0x036F , 112 , 112 ),
         new UnicodeBlock(0x0370 , 0x03FF , 144 , 135 ),
         new UnicodeBlock(0x0400 , 0x04FF , 256 , 256 ),
         new UnicodeBlock(0x0500 , 0x052F , 48 , 48 ),
         new UnicodeBlock(0x0530 , 0x058F , 96 , 91 ),
         new UnicodeBlock(0x0590 , 0x05FF , 112 , 88 ),
         new UnicodeBlock(0x0600 , 0x06FF , 256 , 256 ),
         new UnicodeBlock(0x0700 , 0x074F , 80 , 77 ),
         new UnicodeBlock(0x0750 , 0x077F , 48 , 48 ),
         new UnicodeBlock(0x0780 , 0x07BF , 64 , 50 ),
         new UnicodeBlock(0x07C0 , 0x07FF , 64 , 62 ),
         new UnicodeBlock(0x0800 , 0x083F , 64 , 61 ),
         new UnicodeBlock(0x0840 , 0x085F , 32 , 29 ),
         new UnicodeBlock(0x0860 , 0x086F , 16 , 11 ),
         new UnicodeBlock(0x0870 , 0x089F , 48 , 41 ),
         new UnicodeBlock(0x08A0 , 0x08FF , 96 , 96 ),
         new UnicodeBlock(0x0900 , 0x097F , 128 , 128 ),
         new UnicodeBlock(0x0980 , 0x09FF , 128 , 96 ),
         new UnicodeBlock(0x0A00 , 0x0A7F , 128 , 80 ),
         new UnicodeBlock(0x0A80 , 0x0AFF , 128 , 91 ),
         new UnicodeBlock(0x0B00 , 0x0B7F , 128 , 91 ),
         new UnicodeBlock(0x0B80 , 0x0BFF , 128 , 72 ),
         new UnicodeBlock(0x0C00 , 0x0C7F , 128 , 100 ),
         new UnicodeBlock(0x0C80 , 0x0CFF , 128 , 91 ),
         new UnicodeBlock(0x0D00 , 0x0D7F , 128 , 118 ),
         new UnicodeBlock(0x0D80 , 0x0DFF , 128 , 91 ),
         new UnicodeBlock(0x0E00 , 0x0E7F , 128 , 87 ),
         new UnicodeBlock(0x0E80 , 0x0EFF , 128 , 83 ),
         new UnicodeBlock(0x0F00 , 0x0FFF , 256 , 211 ),
         new UnicodeBlock(0x1000 , 0x109F , 160 , 160 ),
         new UnicodeBlock(0x10A0 , 0x10FF , 96 , 88 ),
         new UnicodeBlock(0x1100 , 0x11FF , 256 , 256 ),
         new UnicodeBlock(0x1200 , 0x137F , 384 , 358 ),
         new UnicodeBlock(0x1380 , 0x139F , 32 , 26 ),
         new UnicodeBlock(0x13A0 , 0x13FF , 96 , 92 ),
         new UnicodeBlock(0x1400 , 0x167F , 640 , 640 ),
         new UnicodeBlock(0x1680 , 0x169F , 32 , 29 ),
         new UnicodeBlock(0x16A0 , 0x16FF , 96 , 89 ),
         new UnicodeBlock(0x1700 , 0x171F , 32 , 23 ),
         new UnicodeBlock(0x1720 , 0x173F , 32 , 23 ),
         new UnicodeBlock(0x1740 , 0x175F , 32 , 20 ),
         new UnicodeBlock(0x1760 , 0x177F , 32 , 18 ),
         new UnicodeBlock(0x1780 , 0x17FF , 128 , 114 ),
         new UnicodeBlock(0x1800 , 0x18AF , 176 , 158 ),
         new UnicodeBlock(0x18B0 , 0x18FF , 80 , 70 ),
         new UnicodeBlock(0x1900 , 0x194F , 80 , 68 ),
         new UnicodeBlock(0x1950 , 0x197F , 48 , 35 ),
         new UnicodeBlock(0x1980 , 0x19DF , 96 , 83 ),
         new UnicodeBlock(0x19E0 , 0x19FF , 32 , 32 ),
         new UnicodeBlock(0x1A00 , 0x1A1F , 32 , 30 ),
         new UnicodeBlock(0x1A20 , 0x1AAF , 144 , 127 ),
         new UnicodeBlock(0x1AB0 , 0x1AFF , 80 , 31 ),
         new UnicodeBlock(0x1B00 , 0x1B7F , 128 , 124 ),
         new UnicodeBlock(0x1B80 , 0x1BBF , 64 , 64 ),
         new UnicodeBlock(0x1BC0 , 0x1BFF , 64 , 56 ),
         new UnicodeBlock(0x1C00 , 0x1C4F , 80 , 74 ),
         new UnicodeBlock(0x1C50 , 0x1C7F , 48 , 48 ),
         new UnicodeBlock(0x1C80 , 0x1C8F , 16 , 9 ),
         new UnicodeBlock(0x1C90 , 0x1CBF , 48 , 46 ),
         new UnicodeBlock(0x1CC0 , 0x1CCF , 16 , 8 ),
         new UnicodeBlock(0x1CD0 , 0x1CFF , 48 , 43 ),
         new UnicodeBlock(0x1D00 , 0x1D7F , 128 , 128 ),
         new UnicodeBlock(0x1D80 , 0x1DBF , 64 , 64 ),
         new UnicodeBlock(0x1DC0 , 0x1DFF , 64 , 64 ),
         new UnicodeBlock(0x1E00 , 0x1EFF , 256 , 256 ),
         new UnicodeBlock(0x1F00 , 0x1FFF , 256 , 233 ),
         new UnicodeBlock(0x2000 , 0x206F , 112 , 111 ),
         new UnicodeBlock(0x2070 , 0x209F , 48 , 42 ),
         new UnicodeBlock(0x20A0 , 0x20CF , 48 , 33 ),
         new UnicodeBlock(0x20D0 , 0x20FF , 48 , 33 ),
         new UnicodeBlock(0x2100 , 0x214F , 80 , 80 ),
         new UnicodeBlock(0x2150 , 0x218F , 64 , 60 ),
         new UnicodeBlock(0x2200 , 0x22FF , 256 , 256 ),
         new UnicodeBlock(0x2300 , 0x23FF , 256 , 256 ),
         new UnicodeBlock(0x2400 , 0x243F , 64 , 39 ),
         new UnicodeBlock(0x2440 , 0x245F , 32 , 11 ),
         new UnicodeBlock(0x2460 , 0x24FF , 160 , 160 ),
         new UnicodeBlock(0x2600 , 0x26FF , 256 , 256 ),
         new UnicodeBlock(0x27C0 , 0x27EF , 48 , 48 ),
         new UnicodeBlock(0x2980 , 0x29FF , 128 , 128 ),
         new UnicodeBlock(0x2A00 , 0x2AFF , 256 , 256 ),
         new UnicodeBlock(0x2C00 , 0x2C5F , 96 , 96 ),
         new UnicodeBlock(0x2C60 , 0x2C7F , 32 , 32 ),
         new UnicodeBlock(0x2C80 , 0x2CFF , 128 , 123 ),
         new UnicodeBlock(0x2D00 , 0x2D2F , 48 , 40 ),
         new UnicodeBlock(0x2D30 , 0x2D7F , 80 , 59 ),
         new UnicodeBlock(0x2D80 , 0x2DDF , 96 , 79 ),
         new UnicodeBlock(0x2DE0 , 0x2DFF , 32 , 32 ),
         new UnicodeBlock(0x2E00 , 0x2E7F , 128 , 94 ),
         new UnicodeBlock(0x2F00 , 0x2FDF , 224 , 214 ),
         new UnicodeBlock(0x2FF0 , 0x2FFF , 16 , 12 ),
         new UnicodeBlock(0x3000 , 0x303F , 64 , 64 ),
         new UnicodeBlock(0x3040 , 0x309F , 96 , 93 ),
         new UnicodeBlock(0x30A0 , 0x30FF , 96 , 96 ),
         new UnicodeBlock(0x3100 , 0x312F , 48 , 43 ),
         new UnicodeBlock(0x3130 , 0x318F , 96 , 94 ),
         new UnicodeBlock(0x3190 , 0x319F , 16 , 16 ),
         new UnicodeBlock(0x31A0 , 0x31BF , 32 , 32 ),
         new UnicodeBlock(0x31C0 , 0x31EF , 48 , 36 ),
         new UnicodeBlock(0x31F0 , 0x31FF , 16 , 16 ),
         new UnicodeBlock(0x3200 , 0x32FF , 256 , 255 ),
         new UnicodeBlock(0x3300 , 0x33FF , 256 , 256 ),
         new UnicodeBlock(0x3400 , 0x4DBF , 6592 , 6592 ),
         new UnicodeBlock(0x4DC0 , 0x4DFF , 64 , 64 ),
         new UnicodeBlock(0x4E00 , 0x9FFF , 20992 , 20992 ),
         new UnicodeBlock(0xA000 , 0xA48F , 1168 , 1165 ),
         new UnicodeBlock(0xA490 , 0xA4CF , 64 , 55 ),
         new UnicodeBlock(0xA4D0 , 0xA4FF , 48 , 48 ),
         new UnicodeBlock(0xA500 , 0xA63F , 320 , 300 ),
         new UnicodeBlock(0xA640 , 0xA69F , 96 , 96 ),
         new UnicodeBlock(0xA6A0 , 0xA6FF , 96 , 88 ),
         new UnicodeBlock(0xA700 , 0xA71F , 32 , 32 ),
         new UnicodeBlock(0xA720 , 0xA7FF , 224 , 193 ),
         new UnicodeBlock(0xA800 , 0xA82F , 48 , 45 ),
         new UnicodeBlock(0xA830 , 0xA83F , 16 , 10 ),
         new UnicodeBlock(0xA840 , 0xA87F , 64 , 56 ),
         new UnicodeBlock(0xA880 , 0xA8DF , 96 , 82 ),
         new UnicodeBlock(0xA8E0 , 0xA8FF , 32 , 32 ),
         new UnicodeBlock(0xA900 , 0xA92F , 48 , 48 ),
         new UnicodeBlock(0xA930 , 0xA95F , 48 , 37 ),
         new UnicodeBlock(0xA960 , 0xA97F , 32 , 29 ),
         new UnicodeBlock(0xA980 , 0xA9DF , 96 , 91 ),
         new UnicodeBlock(0xA9E0 , 0xA9FF , 32 , 31 ),
         new UnicodeBlock(0xAA00 , 0xAA5F , 96 , 83 ),
         new UnicodeBlock(0xAA60 , 0xAA7F , 32 , 32 ),
         new UnicodeBlock(0xAA80 , 0xAADF , 96 , 72 ),
         new UnicodeBlock(0xAAE0 , 0xAAFF , 32 , 23 ),
         new UnicodeBlock(0xAB00 , 0xAB2F , 48 , 32 ),
         new UnicodeBlock(0xAB30 , 0xAB6F , 64 , 60 ),
         new UnicodeBlock(0xAB70 , 0xABBF , 80 , 80 ),
         new UnicodeBlock(0xABC0 , 0xABFF , 64 , 56 ),
         new UnicodeBlock(0xAC00 , 0xD7AF , 11184 , 11172 ),
         new UnicodeBlock(0xD7B0 , 0xD7FF , 80 , 72 ),
         new UnicodeBlock(0xF900 , 0xFAFF , 512 , 472 ),
         new UnicodeBlock(0xFB00 , 0xFB4F , 80 , 58 ),
         new UnicodeBlock(0xFB50 , 0xFDFF , 688 , 631 ),
         new UnicodeBlock(0xFE00 , 0xFE0F , 16 , 16 ),
         new UnicodeBlock(0xFE10 , 0xFE1F , 16 , 10 ),
         new UnicodeBlock(0xFE20 , 0xFE2F , 16 , 16 ),
         new UnicodeBlock(0xFE30 , 0xFE4F , 32 , 32 ),
         new UnicodeBlock(0xFE50 , 0xFE6F , 32 , 26 ),
         new UnicodeBlock(0xFE70 , 0xFEFF , 144 , 141 ),
         new UnicodeBlock(0xFF00 , 0xFFEF , 240 , 225 ),
         new UnicodeBlock(0xFFF0 , 0xFFFD , 14 , 5 ),
         new UnicodeBlock(0x10000 , 0x1007F , 128 , 88 ),
         new UnicodeBlock(0x10080 , 0x100FF , 128 , 123 ),
         new UnicodeBlock(0x10100 , 0x1013F , 64 , 57 ),
         new UnicodeBlock(0x10140 , 0x1018F , 80 , 79 ),
         new UnicodeBlock(0x10190 , 0x101CF , 64 , 14 ),
         new UnicodeBlock(0x101D0 , 0x101FF , 48 , 46 ),
         new UnicodeBlock(0x10280 , 0x1029F , 32 , 29 ),
         new UnicodeBlock(0x102A0 , 0x102DF , 64 , 49 ),
         new UnicodeBlock(0x102E0 , 0x102FF , 32 , 28 ),
         new UnicodeBlock(0x10300 , 0x1032F , 48 , 39 ),
         new UnicodeBlock(0x10330 , 0x1034F , 32 , 27 ),
         new UnicodeBlock(0x10350 , 0x1037F , 48 , 43 ),
         new UnicodeBlock(0x10380 , 0x1039F , 32 , 31 ),
         new UnicodeBlock(0x103A0 , 0x103DF , 64 , 50 ),
         new UnicodeBlock(0x10400 , 0x1044F , 80 , 80 ),
         new UnicodeBlock(0x10450 , 0x1047F , 48 , 48 ),
         new UnicodeBlock(0x10480 , 0x104AF , 48 , 40 ),
         new UnicodeBlock(0x104B0 , 0x104FF , 80 , 72 ),
         new UnicodeBlock(0x10500 , 0x1052F , 48 , 40 ),
         new UnicodeBlock(0x10530 , 0x1056F , 64 , 53 ),
         new UnicodeBlock(0x10570 , 0x105BF , 80 , 70 ),
         new UnicodeBlock(0x10600 , 0x1077F , 384 , 341 ),
         new UnicodeBlock(0x10780 , 0x107BF , 64 , 57 ),
         new UnicodeBlock(0x10800 , 0x1083F , 64 , 55 ),
         new UnicodeBlock(0x10840 , 0x1085F , 32 , 31 ),
         new UnicodeBlock(0x10860 , 0x1087F , 32 , 32 ),
         new UnicodeBlock(0x10880 , 0x108AF , 48 , 40 ),
         new UnicodeBlock(0x108E0 , 0x108FF , 32 , 26 ),
         new UnicodeBlock(0x10900 , 0x1091F , 32 , 29 ),
         new UnicodeBlock(0x10920 , 0x1093F , 32 , 27 ),
         new UnicodeBlock(0x10980 , 0x1099F , 32 , 32 ),
         new UnicodeBlock(0x109A0 , 0x109FF , 96 , 90 ),
         new UnicodeBlock(0x10A00 , 0x10A5F , 96 , 68 ),
         new UnicodeBlock(0x10A60 , 0x10A7F , 32 , 32 ),
         new UnicodeBlock(0x10A80 , 0x10A9F , 32 , 32 ),
         new UnicodeBlock(0x10AC0 , 0x10AFF , 64 , 51 ),
         new UnicodeBlock(0x10B00 , 0x10B3F , 64 , 61 ),
         new UnicodeBlock(0x10B40 , 0x10B5F , 32 , 30 ),
         new UnicodeBlock(0x10B60 , 0x10B7F , 32 , 27 ),
         new UnicodeBlock(0x10B80 , 0x10BAF , 48 , 29 ),
         new UnicodeBlock(0x10C00 , 0x10C4F , 80 , 73 ),
         new UnicodeBlock(0x10C80 , 0x10CFF , 128 , 108 ),
         new UnicodeBlock(0x10D00 , 0x10D3F , 64 , 50 ),
         new UnicodeBlock(0x10E60 , 0x10E7F , 32 , 31 ),
         new UnicodeBlock(0x10E80 , 0x10EBF , 48 , 47 ),
         new UnicodeBlock(0x10EC0 , 0x10EFF , 64 , 3 ),
         new UnicodeBlock(0x10F00 , 0x10F2F , 48 , 40 ),
         new UnicodeBlock(0x10F30 , 0x10F6F , 64 , 42 ),
         new UnicodeBlock(0x10F70 , 0x10FAF , 64 , 26 ),
         new UnicodeBlock(0x10FB0 , 0x10FDF , 48 , 28 ),
         new UnicodeBlock(0x10FE0 , 0x10FFF , 32 , 23 ),
         new UnicodeBlock(0x11000 , 0x1107F , 128 , 115 ),
         new UnicodeBlock(0x11080 , 0x110CF , 80 , 68 ),
         new UnicodeBlock(0x110D0 , 0x110FF , 48 , 35 ),
         new UnicodeBlock(0x11100 , 0x1114F , 80 , 71 ),
         new UnicodeBlock(0x11150 , 0x1117F , 48 , 39 ),
         new UnicodeBlock(0x11180 , 0x111DF , 96 , 96 ),
         new UnicodeBlock(0x111E0 , 0x111FF , 32 , 20 ),
         new UnicodeBlock(0x11200 , 0x1124F , 80 , 65 ),
         new UnicodeBlock(0x11280 , 0x112AF , 48 , 38 ),
         new UnicodeBlock(0x112B0 , 0x112FF , 80 , 69 ),
         new UnicodeBlock(0x11300 , 0x1137F , 128 , 86 ),
         new UnicodeBlock(0x11400 , 0x1147F , 128 , 97 ),
         new UnicodeBlock(0x11480 , 0x114DF , 96 , 82 ),
         new UnicodeBlock(0x11580 , 0x115FF , 128 , 92 ),
         new UnicodeBlock(0x11600 , 0x1165F , 96 , 79 ),
         new UnicodeBlock(0x11660 , 0x1167F , 32 , 13 ),
         new UnicodeBlock(0x11680 , 0x116CF , 80 , 68 ),
         new UnicodeBlock(0x11700 , 0x1174F , 80 , 65 ),
         new UnicodeBlock(0x11800 , 0x1184F , 80 , 60 ),
         new UnicodeBlock(0x118A0 , 0x118FF , 96 , 84 ),
         new UnicodeBlock(0x11900 , 0x1195F , 96 , 72 ),
         new UnicodeBlock(0x119A0 , 0x119FF , 96 , 65 ),
         new UnicodeBlock(0x11A00 , 0x11A4F , 96 , 72 ),
         new UnicodeBlock(0x11A50 , 0x11AAF , 96 , 83 ),
         new UnicodeBlock(0x11AB0 , 0x11ABF , 16 , 16 ),
         new UnicodeBlock(0x11AC0 , 0x11AFF , 64 , 57 ),
         new UnicodeBlock(0x11B00 , 0x11B5F , 80 , 10 ),
         new UnicodeBlock(0x11C00 , 0x11C6F , 112 , 97 ),
         new UnicodeBlock(0x11C70 , 0x11CBF , 80 , 68 ),
         new UnicodeBlock(0x11D00 , 0x11D5F , 96 , 75 ),
         new UnicodeBlock(0x11D60 , 0x11DAF , 80 , 63 ),
         new UnicodeBlock(0x11EE0 , 0x11EFF , 32 , 25 ),
         new UnicodeBlock(0x11F00 , 0x11F5F , 96 , 86 ),
         new UnicodeBlock(0x11FB0 , 0x11FBF , 16 , 1 ),
         new UnicodeBlock(0x11FC0 , 0x11FFF , 64 , 51 ),
         new UnicodeBlock(0x12000 , 0x123FF , 1024 , 922 ),
         new UnicodeBlock(0x12400 , 0x1247F , 128 , 116 ),
         new UnicodeBlock(0x12480 , 0x1254F , 208 , 196 ),
         new UnicodeBlock(0x12F90 , 0x12FFF , 112 , 99 ),
         new UnicodeBlock(0x13000 , 0x1342F , 1072 , 1072 ),
         new UnicodeBlock(0x13430 , 0x1343F , 16 , 38 ),
         new UnicodeBlock(0x14400 , 0x1467F , 640 , 583 ),
         new UnicodeBlock(0x16800 , 0x16A3F , 576 , 569 ),
         new UnicodeBlock(0x16A40 , 0x16A6F , 48 , 43 ),
         new UnicodeBlock(0x16A70 , 0x16ACF , 96 , 89 ),
         new UnicodeBlock(0x16AD0 , 0x16AFF , 48 , 36 ),
         new UnicodeBlock(0x16B00 , 0x16B8F , 144 , 127 ),
         new UnicodeBlock(0x16E40 , 0x16E9F , 96 , 91 ),
         new UnicodeBlock(0x16F00 , 0x16F9F , 160 , 149 ),
         new UnicodeBlock(0x16FE0 , 0x16FFF , 32 , 7 ),
         new UnicodeBlock(0x17000 , 0x187FF , 6144 , 6136 ),
         new UnicodeBlock(0x18800 , 0x18AFF , 768 , 768 ),
         new UnicodeBlock(0x18B00 , 0x18CFF , 512 , 470 ),
         new UnicodeBlock(0x18D00 , 0x18D7F , 128 , 9 ),
         new UnicodeBlock(0x1AFF0 , 0x1AFFF , 16 , 13 ),
         new UnicodeBlock(0x1B000 , 0x1B0FF , 256 , 256 ),
         new UnicodeBlock(0x1B100 , 0x1B12F , 48 , 35 ),
         new UnicodeBlock(0x1B130 , 0x1B16F , 64 , 9 ),
         new UnicodeBlock(0x1B170 , 0x1B2FF , 400 , 396 ),
         new UnicodeBlock(0x1BC00 , 0x1BC9F , 160 , 143 ),
         new UnicodeBlock(0x1BCA0 , 0x1BCAF , 16 , 4 ),
         new UnicodeBlock(0x1CF00 , 0x1CFCF , 208 , 185 ),
         new UnicodeBlock(0x1D000 , 0x1D0FF , 256 , 246 ),
         new UnicodeBlock(0x1D100 , 0x1D1FF , 256 , 233 ),
         new UnicodeBlock(0x1D200 , 0x1D24F , 80 , 70 ),
         new UnicodeBlock(0x1D2E0 , 0x1D2FF , 32 , 20 ),
         new UnicodeBlock(0x1D300 , 0x1D35F , 96 , 87 ),
         new UnicodeBlock(0x1D360 , 0x1D37F , 32 , 25 ),
         new UnicodeBlock(0x1D400 , 0x1D7FF , 1024 , 996 ),
         new UnicodeBlock(0x1D800 , 0x1DAAF , 688 , 672 ),
         new UnicodeBlock(0x1DF00 , 0x1DFFF , 256 , 37 ),
         new UnicodeBlock(0x1E000 , 0x1E02F , 48 , 38 ),
         new UnicodeBlock(0x1E100 , 0x1E14F , 80 , 71 ),
         new UnicodeBlock(0x1E270 , 0x1E28F , 32 , 0 ),
         new UnicodeBlock(0x1E290 , 0x1E2BF , 48 , 31 ),
         new UnicodeBlock(0x1E2C0 , 0x1E2FF , 64 , 59 ),
         new UnicodeBlock(0x1E4D0 , 0x1E4FF , 48 , 42 ),
         new UnicodeBlock(0x1E7E0 , 0x1E7FF , 32 , 28 ),
         new UnicodeBlock(0x1E800 , 0x1E8DF , 224 , 213 ),
         new UnicodeBlock(0x1E900 , 0x1E95F , 96 , 88 ),
         new UnicodeBlock(0x1EC70 , 0x1ECBF , 80 , 68 ),
         new UnicodeBlock(0x1ED00 , 0x1ED4F , 80 , 61 ),
         new UnicodeBlock(0x1EE00 , 0x1EEFF , 256 , 143 ),
         new UnicodeBlock(0x1F000 , 0x1F02F , 48 , 44 ),
         new UnicodeBlock(0x1F030 , 0x1F09F , 112 , 100 ),
         new UnicodeBlock(0x1F0A0 , 0x1F0FF , 96 , 82 ),
         new UnicodeBlock(0x1F100 , 0x1F1FF , 256 , 200 ),
         new UnicodeBlock(0x1F200 , 0x1F2FF , 256 , 64 ),
         new UnicodeBlock(0x1F300 , 0x1F5FF , 768 , 768 ),
         new UnicodeBlock(0x1F600 , 0x1F64F , 80 , 80 ),
         new UnicodeBlock(0x1F650 , 0x1F67F , 48 , 48 ),
         new UnicodeBlock(0x1F680 , 0x1F6FF , 128 , 118 ),
         new UnicodeBlock(0x1F700 , 0x1F77F , 128 , 124 ),
         new UnicodeBlock(0x1F780 , 0x1F7FF , 128 , 103 ),
         new UnicodeBlock(0x1F800 , 0x1F8FF , 256 , 150 ),
         new UnicodeBlock(0x1F900 , 0x1F9FF , 256 , 256 ),
         new UnicodeBlock(0x1FA00 , 0x1FA6F , 112 , 98 ),
         new UnicodeBlock(0x1FB00 , 0x1FBFF , 256 , 212 ),
         new UnicodeBlock(0x20000 , 0x2A6DF , 42720 , 42720 ),
         new UnicodeBlock(0x2A700 , 0x2B73F , 4160 , 4154 ),
         new UnicodeBlock(0x2B740 , 0x2B81F , 224 , 222 ),
         new UnicodeBlock(0x2B820 , 0x2CEAF , 5776 , 5762 ),
         new UnicodeBlock(0x2CEB0 , 0x2EBEF , 7488 , 7473 ),
         new UnicodeBlock(0x2F800 , 0x2FA1F , 544 , 542 ),
         new UnicodeBlock(0x30000 , 0x3134F , 4944 , 4939 ),
         new UnicodeBlock(0x31350 , 0x323AF , 4192 , 4192 ),
         new UnicodeBlock(0xE0000 , 0xE007F , 128 , 97 ),
         new UnicodeBlock(0xE0100 , 0xE01EF , 240 , 240 )];
}