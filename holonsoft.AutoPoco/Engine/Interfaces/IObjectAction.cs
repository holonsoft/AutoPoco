namespace holonsoft.AutoPoco.Engine.Interfaces;

/// <summary>
///   The base interface for any action to be enacted on an object post-creation
/// </summary>
public interface IObjectAction {
   /// <summary>
   ///   Enacts this action on the target object
   /// </summary>
   /// <param name="context"></param>
   /// <param name="target"></param>
   void Enact(IGenerationContext? context, object target);
}