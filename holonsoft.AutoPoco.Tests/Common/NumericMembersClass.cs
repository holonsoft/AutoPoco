namespace holonsoft.AutoPoco.Tests.Common;

public class NumericMembersClass {
   public byte Rating { get; set; }
   public decimal Amount { get; set; }
   public Int128 BigId { get; set; }
   public Half Ratio { get; set; }
   public ushort? OptionalCount { get; set; }
   public long Field;
}
