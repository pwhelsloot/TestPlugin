//-----------------------------------------------------------------------------
// <copyright file="GeographyPoint.cs" company="AMCS Group">
//   Copyright © 2010-12 AMCS Group. All rights reserved.
// </copyright>
//
// PROJECT: P142 - Elemos
//
// AMCS Elemos Project
//
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity
{
  [Serializable]
  [DataContract(Namespace = "http://www.amcsgroup.com/elemos")]
  public class GeographyPoint : EntityObject
  {

    public GeographyPoint()
      : base()
    {
    }

    public GeographyPoint(object initialiseWith)
      : base(initialiseWith)
    {
    }

    public GeographyPoint(object initialiseWith, Type commonInterface)
      : base(initialiseWith, commonInterface)
    {
    }

    private double? _Lat;

    [DataMember(Name = "Lat")]
    public double? Lat
    {
      get
      {
        return _Lat;
      }

      set
      {
        if (this._Lat != value)
        {
          if (value != null && value.HasValue && value >= -90 && value <= 90)
          {
            this._Lat = value;
            this.IsValidPoint = true;
          }
          else
          {
            this._Lat = null;
            this.IsValidPoint = false;
          }
          this.NotifyChange("Lat");
        }
      }
    }

    private double? _Long;

    [DataMember(Name = "Long")]
    public double? Long
    {
      get
      {
        return _Long;
      }

      set
      {
        if (this._Long != value)
        {
          if (value != null && value.HasValue && value >= -180 && value <= 180)
          {
            this._Long = value;
            this.IsValidPoint = true;
          }
          else
          {
            this._Long = null;
            this.IsValidPoint = false;
          }
          this.NotifyChange("Long");
        }
      }
    }

    public bool HasCoordinates
    {
      get
      {
        return this.Lat.HasValue && this.Long.HasValue && this.IsValidPoint;
      }
    }

    public bool IsValidPoint { get; set; }

    #region overridespublic

    /// <summary>
    /// Returns table name used to generate update/insert
    /// </summary>
    /// <returns>table name.</returns>
    public override string GetTableName()
    {
      return "NotImplemented";
    }

    /// <summary>
    /// Returns primary key name used to generate update/insert
    /// </summary>
    /// <returns>primary key  name.</returns>
    public override string GetKeyName()
    {
      return "NotImplemented";
    }

    /// <summary>
    /// Returns the primary key value
    /// </summary>
    /// <returns>primary key value.</returns>
    public override int? GetId()
    {
      return null;
    }

    #endregion overridespublic
  }
}