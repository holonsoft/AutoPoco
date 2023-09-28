﻿using System.Linq.Expressions;

namespace holonsoft.AutoPoco.Engine.Interfaces;

public interface IObjectGenerator<TPoco> {
   /// <summary>
   ///   Creates an instance of an object conforming to the specified rules
   /// </summary>
   /// <returns></returns>
   TPoco Get();

   /// <summary>
   ///   Imposes a value on the created object that overrides any rules speciifed in configuration
   /// </summary>
   /// <typeparam name="TMember"></typeparam>
   /// <param name="propertyExpr"></param>
   /// <param name="value"></param>
   /// <returns></returns>
   IObjectGenerator<TPoco> Impose<TMember>(Expression<Func<TPoco, TMember>> propertyExpr, TMember value);

   /// <summary>
   ///   Invokes a method on all the items in the current selection
   /// </summary>
   /// <param name="methodExpr"></param>
   /// <returns></returns>
   IObjectGenerator<TPoco> Invoke(Expression<Action<TPoco>> methodExpr);

   /// <summary>
   ///   Overrides the data source for this particular generation scope
   /// </summary>
   /// <returns></returns>
   IObjectGenerator<TPoco> Source<TMember>(Expression<Func<TPoco, TMember>> propertyExpr, IDataSource dataSource);

   /// <summary>
   ///   Invokes a method on all the items in the current selection
   /// </summary>
   /// <param name="methodExpr"></param>
   /// <returns></returns>
   IObjectGenerator<TPoco> Invoke<TMember>(Expression<Func<TPoco, TMember>> methodExpr);
}