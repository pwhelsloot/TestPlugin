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
  public class GeographyPolygon : EntityObject
  {

    public GeographyPolygon()
      : base()
    {
    }

    public GeographyPolygon(object initialiseWith)
      : base(initialiseWith)
    {
    }

    public GeographyPolygon(object initialiseWith, Type commonInterface)
      : base(initialiseWith, commonInterface)
    {
    }

    private IList<GeographyPoint> _Positions;
    [DataMember(Name = "Positions")]
    public IList<GeographyPoint> Positions
    {
      get { return _Positions; }
      set { if (_Positions != value) { _Positions = value; NotifyChange(() => Positions); } }
    }

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