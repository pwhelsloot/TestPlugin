//-----------------------------------------------------------------------------
// <copyright file="TranslationMappingOverrideEntity.cs" company="AMCS Group">
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
  public class TranslationMappingOverrideEntity : EntityObject
  {
    #region properties

    private int? _TranslationMappingOverrideId;

    [DataMember(Name = "TranslationMappingOverrideId")]
    public int? TranslationMappingOverrideId
    {
      get { return _TranslationMappingOverrideId; }
      set { if (_TranslationMappingOverrideId != value) { _TranslationMappingOverrideId = value; NotifyChange(() => TranslationMappingOverrideId); } }
    }

    private string _ProjectIdentifier;

    [DataMember(Name = "ProjectIdentifier")]
    public string ProjectIdentifier
    {
      get { return _ProjectIdentifier; }
      set { if (_ProjectIdentifier != value) { _ProjectIdentifier = value; NotifyChange(() => ProjectIdentifier); } }
    }

    private string _MappingId;

    [DataMember(Name = "MappingId")]
    public string MappingId
    {
      get { return _MappingId; }
      set { if (_MappingId != value) { _MappingId = value; NotifyChange(() => MappingId); } }
    }

    private string _Mapping;

    [DataMember(Name = "Mapping")]
    public string Mapping
    {
      get { return _Mapping; }
      set { if (_Mapping != value) { _Mapping = value; NotifyChange(() => Mapping); } }
    }

    #endregion properties

    #region tablenames

    /// <summary>
    /// Returns table name used to generate update/insert
    /// </summary>
    /// <returns>table name.</returns>
    public override string GetTableName()
    {
      return "TranslationMappingOverride";
    }

    /// <summary>
    /// Returns primary key name used to generate update/insert
    /// </summary>
    /// <returns>primary key  name.</returns>
    public override string GetKeyName()
    {
      return "TranslationMappingOverrideId";
    }

    /// <summary>
    /// Returns the primary key value
    /// </summary>
    /// <returns>primary key value.</returns>
    public override int? GetId()
    {
      return TranslationMappingOverrideId;
    }

    #endregion tablenames
  }
}