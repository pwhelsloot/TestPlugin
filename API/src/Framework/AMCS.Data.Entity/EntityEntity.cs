//-----------------------------------------------------------------------------
// <copyright file="EntityEntity.cs" company="AMCS Group">
//   Copyright © 2013 AMCS Group. All rights reserved.
// </copyright>
//
// PROJECT: P119 - Elemos
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
using AMCS.Data.Entity.Interfaces;

namespace AMCS.Data.Entity
{
  [Serializable]
  [DataContract(Namespace = "http://www.solutionworks.co.uk/wims")]
  [EntityTable("Entity", "EntityId", IdentityInsertMode = IdentityInsertMode.On)]
  public class EntityEntity : EntityObject, IRestrictableParent
  {
    #region attributes

    private int? _EntityId;

    [DataMember(Name = "EntityId")]
    public int? EntityId
    {
      get
      {
        return _EntityId;
      }

      set
      {
        if (_EntityId != value)
        {
          _EntityId = value;
          NotifyChange("EntityId");
        }
      }
    }

    private string _Description;

    [DataMember(Name = "Description")]
    public string Description
    {
      get
      {
        return _Description;
      }

      set
      {
        if (_Description != value)
        {
          _Description = value;
          NotifyChange("Description");
        }
      }
    }

    private string _ClassName;

    [DataMember(Name = "ClassName")]
    public string ClassName
    {
      get
      {
        return _ClassName;
      }

      set
      {
        if (_ClassName != value)
        {
          _ClassName = value;
          NotifyChange("ClassName");
        }
      }
    }

    private IList<EntityPropertyEntity> _EntityProperties;
    [DynamicColumn("EntityProperties")]
    [DataMember(Name = "EntityProperties")]
    public IList<EntityPropertyEntity> EntityProperties
    {
      get
      {
        return _EntityProperties;
      }

      set
      {
        if (_EntityProperties != value)
        {
          _EntityProperties = value;
          NotifyChange("EntityProperties");
        }
      }
    }

    public IList<IRestrictable> Children
    {
      get { return new List<IRestrictable>(this.EntityProperties); }
    }

    #endregion attributes

    #region overridespublic

    /// <summary>
    /// Returns the primary key value
    /// </summary>
    /// <returns>primary key value.</returns>
    public override int? GetId()
    {
      return EntityId;
    }

    #endregion overridespublic
  }
}