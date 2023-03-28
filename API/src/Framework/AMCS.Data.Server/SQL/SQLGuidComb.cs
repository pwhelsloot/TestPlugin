using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SQL
{
  public static class SQLGuidComb
  {
    private static readonly long BaseDateTicks = new DateTime(1900, 1, 1).Ticks;

    /// <summary>
    /// Generate a semi sequential GUID to prevent index fragmentation.
    /// </summary>
    /// <remarks>
    /// Originally taken from https://github.com/nhibernate/nhibernate-core/blob/master/src/NHibernate/Id/GuidCombGenerator.cs.
    /// </remarks>
    /// <returns>Semi sequential GUID.</returns>
    public static Guid Generate()
    {
      var guidArray = Guid.NewGuid().ToByteArray();

      var now = DateTime.UtcNow;

      // Get the days and milliseconds which will be used to build the byte string 
      var days = new TimeSpan(now.Ticks - BaseDateTicks);
      var msecs = now.TimeOfDay;

      // Convert to a byte array 
      // Note that SQL Server is accurate to 1/300th of a millisecond so we divide by 3.333333 
      var daysArray = BitConverter.GetBytes(days.Days);
      var msecsArray = BitConverter.GetBytes((long)(msecs.TotalMilliseconds / 3.333333));

      // Reverse the bytes to match SQL Servers ordering 
      Array.Reverse(daysArray);
      Array.Reverse(msecsArray);

      // Copy the bytes into the guid 
      Array.Copy(daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, 2);
      Array.Copy(msecsArray, msecsArray.Length - 4, guidArray, guidArray.Length - 4, 4);

      return new Guid(guidArray);
    }
  }
}
