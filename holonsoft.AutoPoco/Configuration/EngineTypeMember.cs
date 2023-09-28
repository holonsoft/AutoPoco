namespace holonsoft.AutoPoco.Configuration;

public abstract class EngineTypeMember {
   public abstract string Name { get; }

   public abstract bool IsMethod { get; }

   public abstract bool IsField { get; }

   public abstract bool IsProperty { get; }

   public static bool operator ==(EngineTypeMember? memberOne, EngineTypeMember? memberTwo) => Equals(memberOne, memberTwo);

   public static bool operator !=(EngineTypeMember? memberOne, EngineTypeMember memberTwo) => !Equals(memberOne, memberTwo);

   public override bool Equals(object? obj) {
      if (ReferenceEquals(this, obj))
         return true;
      if (obj is null)
         return false;
      throw new NotImplementedException();
   }

   public override int GetHashCode() => throw new NotImplementedException();
}