//-----------------------------------------------------------------------------
// <copyright file="EntityObjectRequest.cs" company="AMCS Group">
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
  [DataContract(Namespace = "http://www.solutionworks.co.uk/wims")]
  public struct EntityObjectRequest
  {
    public enum WebRequestType { GetById, GetAll, GetAllById, GetAllByParentId }

    [DataMember]
    public WebRequestType RequestType { get; private set; }

    [DataMember]
    public string EntityTypeFullName { get; private set; }

    [DataMember]
    public int Id { get; private set; }

    [DataMember]
    public bool IncludeDeleted { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityObjectRequest"/> class.
    /// </summary>
    /// <param name="requestType">Type of the request.</param>
    /// <param name="entityObjectType">Type of the entity object.</param>
    /// <param name="includeDeleted">if set to <c>true</c> [include deleted].</param>
    public EntityObjectRequest(WebRequestType requestType, Type entityObjectType, bool includeDeleted = false)
      : this()
    {
      if (requestType != WebRequestType.GetAll)
        throw new ArgumentException("Attempt to construct invalid EntityObjectRequest.  An Id must be supplied for all WebRequestTypes other than GetAll");
      RequestType = requestType;
      EntityTypeFullName = entityObjectType.FullName;
      IncludeDeleted = includeDeleted;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityObjectRequest"/> class.
    /// </summary>
    /// <param name="requestType">Type of the request.</param>
    /// <param name="entityObjectType">Type of the entity object.</param>
    /// <param name="id">The id.</param>
    /// <param name="includeDeleted">if set to <c>true</c> [include deleted].</param>
    public EntityObjectRequest(WebRequestType requestType, Type entityObjectType, int id, bool includeDeleted = false)
      : this()
    {
      RequestType = requestType;
      EntityTypeFullName = entityObjectType.FullName;
      Id = id;
      IncludeDeleted = includeDeleted;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityObjectRequest"/> class.
    /// </summary>
    /// <param name="requestType">Type of the request.</param>
    /// <param name="entityObjectType">Type of the entity object.</param>
    /// <param name="id">The id.</param>
    public EntityObjectRequest(WebRequestType requestType, Type entityObjectType, int id)
      : this(requestType, entityObjectType, id, false)
    {
    }
  }
}