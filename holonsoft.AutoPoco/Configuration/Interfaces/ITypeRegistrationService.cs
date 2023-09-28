﻿namespace holonsoft.AutoPoco.Configuration.Interfaces;

public interface ITypeRegistrationService {
   /// <summary>
   ///   Registers a type with the underlying engine
   /// </summary>
   /// <param name="t"></param>
   void RegisterType(Type t);

   /// <summary>
   ///   Registers a collection of types with the underlying engine
   /// </summary>
   /// <param name="t"></param>
   void RegisterTypes(IEnumerable<Type> t);
}