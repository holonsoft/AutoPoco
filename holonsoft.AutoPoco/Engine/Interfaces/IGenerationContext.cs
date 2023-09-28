﻿namespace holonsoft.AutoPoco.Engine.Interfaces;

public interface IGenerationContext : IGenerationSession {
   /// <summary>
   ///   Gets the context under which this call is being made
   /// </summary>
   IGenerationContextNode? Node { get; }

   /// <summary>
   ///   Gets access to the builders available to this context
   /// </summary>
   IGenerationConfiguration Builders { get; }

   /// <summary>
   ///   Gets the current depth of this generation context
   /// </summary>
   int Depth { get; }
}