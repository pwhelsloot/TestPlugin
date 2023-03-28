// //-----------------------------------------------------------------------------
// // <copyright file="BslUserExceptionFactory.cs" company="AMCS Group">
// //   Copyright © 2010-12 AMCS Group. All rights reserved.
// // </copyright>
// //
// // PROJECT: P142 - Elemos
// //
// // AMCS Elemos Project
// //
// //-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Configuration.Mapping.Translate;

namespace AMCS.Data.Server
{
  /// <summary>
  /// User Exception Factory.
  /// </summary>
  /// <typeparam name="T">The type of Exception.</typeparam>
  public class BslUserExceptionFactory<T> where T : IBslUserException, new()
  {
    /// <summary>
    /// Creates the exception.
    /// </summary>
    /// <param name="businessServiceType">Type of the business service.</param>
    /// <param name="businessServiceStringsType">Type of the business service strings.</param>
    /// <param name="message">The message.</param>
    /// <returns></returns>
    public static T CreateException(Type businessServiceType, Type businessServiceStringsType, int message)
    {
      return DoCreateException(businessServiceType, businessServiceStringsType, message, null, null, null);
    }

    /// <summary>
    /// Creates the exception.
    /// </summary>
    /// <param name="businessServiceType">Type of the business service.</param>
    /// <param name="businessServiceStringsType">Type of the business service strings.</param>
    /// <param name="messageTemplate">The message template.</param>
    /// <param name="formatObjs">The format objects.</param>
    /// <returns></returns>
    public static T CreateException(Type businessServiceType, Type businessServiceStringsType, int messageTemplate, params object[] formatObjs)
    {
      return DoCreateException(businessServiceType, businessServiceStringsType, messageTemplate, null, null, formatObjs);
    }

    /// <summary>
    /// Creates the exception.
    /// </summary>
    /// <param name="businessServiceType">Type of the business service.</param>
    /// <param name="businessServiceStringsType">Type of the business service strings.</param>
    /// <param name="message">The message.</param>
    /// <param name="additional">The additional.</param>
    /// <returns></returns>
    public static T CreateException(Type businessServiceType, Type businessServiceStringsType, int message, string additional)
    {
      return DoCreateException(businessServiceType, businessServiceStringsType, message, null, additional, null);
    }

    /// <summary>
    /// Creates the exception.
    /// </summary>
    /// <param name="message">The message enum variant.</param>
    /// <returns></returns>
    public static T CreateException(Enum message)
    {
      return CreateException(message, null);
    }

    /// <summary>
    /// Creates the exception.
    /// </summary>
    /// <param name="message">The message enum variant.</param>
    /// <param name="formatObjs">The format objects.</param>
    /// <returns></returns>
    public static T CreateException(Enum message, params object[] formatObjs)
    {
      return CreateException(message.GetType(), message.GetType(), (int)(object)message, formatObjs);
    }

    /// <summary>
    /// Gets the translator.
    /// </summary>
    /// <param name="businessServiceType">Type of the business service.</param>
    /// <param name="businessStringsType">Type of the business strings.</param>
    /// <returns></returns>
    public static BusinessObjectStringTranslator GetTranslator(Type businessServiceType, Type businessStringsType)
    {
      return new BusinessObjectStringTranslator(businessServiceType.FullName, businessStringsType);
    }

    public static T CreateException(string message)
    {
      IBslUserException prefixAccessObj = new T();
      message = prefixAccessObj.MessagePrefix + message;

      return (T)Activator.CreateInstance(typeof(T), message);
    }

    /// <summary>
    /// Creates the exception.
    /// </summary>
    /// <param name="businessServiceType">Type of the business service.</param>
    /// <param name="businessServiceStringsType">Type of the business service strings.</param>
    /// <param name="message">The message.</param>
    /// <param name="errorCode">The error code associated with the error.</param>
    /// <returns></returns>
    public static T CreateCodeException(Type businessServiceType, Type businessServiceStringsType, int message, Enum errorCode)
    {
      return DoCreateException(businessServiceType, businessServiceStringsType, message, (int)(object)errorCode, null, null);
    }

    /// <summary>
    /// Creates the exception.
    /// </summary>
    /// <param name="businessServiceType">Type of the business service.</param>
    /// <param name="businessServiceStringsType">Type of the business service strings.</param>
    /// <param name="messageTemplate">The message template.</param>
    /// <param name="errorCode">The error code associated with the error.</param>
    /// <param name="formatObjs">The format objects.</param>
    /// <returns></returns>
    public static T CreateCodeException(Type businessServiceType, Type businessServiceStringsType, int messageTemplate, Enum errorCode, params object[] formatObjs)
    {
      return DoCreateException(businessServiceType, businessServiceStringsType, messageTemplate, (int)(object)errorCode, null, formatObjs);
    }

    /// <summary>
    /// Creates the exception.
    /// </summary>
    /// <param name="businessServiceType">Type of the business service.</param>
    /// <param name="businessServiceStringsType">Type of the business service strings.</param>
    /// <param name="message">The message.</param>
    /// <param name="errorCode">The error code associated with the error.</param>
    /// <param name="additional">The additional.</param>
    /// <returns></returns>
    public static T CreateCodeException(Type businessServiceType, Type businessServiceStringsType, int message, Enum errorCode, string additional)
    {
      return DoCreateException(businessServiceType, businessServiceStringsType, message, (int)(object)errorCode, additional, null);
    }

    /// <summary>
    /// Creates the exception.
    /// </summary>
    /// <param name="message">The message enum variant.</param>
    /// <param name="errorCode">The error code associated with the error.</param>
    /// <returns></returns>
    public static T CreateCodeException(Enum message, Enum errorCode)
    {
      return CreateException(message, errorCode, null);
    }

    /// <summary>
    /// Creates the exception.
    /// </summary>
    /// <param name="message">The message enum variant.</param>
    /// <param name="errorCode">The error code associated with the error.</param>
    /// <param name="formatObjs">The format objects.</param>
    /// <returns></returns>
    public static T CreateCodeException(Enum message, Enum errorCode, params object[] formatObjs)
    {
      return DoCreateException(message.GetType(), message.GetType(), (int)(object)message, (int)(object)errorCode, null, formatObjs);
    }

    public static T CreateCodeException(string message, Enum errorCode)
    {
      IBslUserException prefixAccessObj = new T();
      message = prefixAccessObj.MessagePrefix + message;

      return (T)Activator.CreateInstance(typeof(T), message, (int)(object)errorCode);
    }

    /// <summary>
    /// Don't make this public, this is being used as the master method that may change at any time, however don't want to
    /// risk braking code that calls
    /// this directly
    /// </summary>
    /// <param name="businessServiceType">Type of the business service.</param>
    /// <param name="businessServiceStringsType">Type of the business service strings.</param>
    /// <param name="message">The message.</param>
    /// <param name="errorCode">The error code associated with the error.</param>
    /// <param name="additional">The additional.</param>
    /// <param name="formatObjs">The format objects.</param>
    /// <returns></returns>
    private static T DoCreateException(Type businessServiceType, Type businessServiceStringsType, int message, int? errorCode, string additional, object[] formatObjs)
    {
      // it would have been nice to be able to to the BslService.GetTranslator(...) method but not all business classes
      // implement this.  There will be no performance hit by getting a translator from this class but it just means
      // that some code is duplicated.
      string messageString = GetTranslator(businessServiceType, businessServiceStringsType).GetLocalisedString(message);

      if (formatObjs != null)
      {
        messageString = string.Format(messageString, formatObjs);
      }

      if (!string.IsNullOrWhiteSpace(additional))
      {
        messageString += "\n" + additional;
      }

      // get the prefix for the type of exception being thrown - don't like creating an instance just for this but
      // it keeps the rest of the code cleaner (since an interface can be used) and won't cause any practical problems.
      // Would be much easier if Exception didn't have a private setter for Message!
      IBslUserException prefixAccessObj = new T();
      messageString = prefixAccessObj.MessagePrefix + messageString;

      // even if new() is provided with the generic class definition we can't create an exception without using only the default no args constructor.
      // since the Message and InnerException properties on exceptions are read-only the following needs to be done.
      if (errorCode.HasValue)
        return (T)Activator.CreateInstance(typeof(T), messageString, errorCode.Value);

      return (T)Activator.CreateInstance(typeof(T), messageString);
    }
  }
}