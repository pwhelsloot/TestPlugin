//-----------------------------------------------------------------------------
// <copyright file="EntityPropertyEntity.cs" company="AMCS Group">
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
  [EntityTable("EntityProperty", "EntityPropertyId", IdentityInsertMode = IdentityInsertMode.On)]
  public class EntityPropertyEntity : EntityObject, IRestrictableChild
  {
    #region attributes

    private int? _EntityPropertyId;

    [DataMember(Name = "EntityPropertyId")]
    public int? EntityPropertyId
    {
      get
      {
        return _EntityPropertyId;
      }

      set
      {
        if (_EntityPropertyId != value)
        {
          _EntityPropertyId = value;
          NotifyChange("EntityPropertyId");
        }
      }
    }

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

    private string _PropertyName;

    [DataMember(Name = "PropertyName")]
    public string PropertyName
    {
      get
      {
        return _PropertyName;
      }

      set
      {
        if (_PropertyName != value)
        {
          _PropertyName = value;
          NotifyChange("PropertyName");
        }
      }
    }

    private string _DefaultValue;

    [DataMember(Name = "DefaultValue")]
    public string DefaultValue
    {
      get
      {
        return _DefaultValue;
      }

      set
      {
        if (_DefaultValue != value)
        {
          _DefaultValue = value;
          NotifyChange("DefaultValue");
        }
      }
    }

    private IRestrictableParent _Parent;

    public IRestrictableParent Parent
    {
      get
      {
        return _Parent;
      }

      set
      {
        if (_Parent != value)
        {
          _Parent = value;
          NotifyChange("Parent");
        }
      }
    }

    #endregion attributes

    #region overridespublic

    /// <summary>
    /// Returns the primary key value
    /// </summary>
    /// <returns>primary key value.</returns>
    public override int? GetId()
    {
      return EntityPropertyId;
    }

    #endregion overridespublic
  }
}