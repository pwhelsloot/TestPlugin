//-----------------------------------------------------------------------------
// <copyright file="EntityObject.cs" company="AMCS Group">
//   Copyright © 2010-11 AMCS Group. All rights reserved.
// </copyright>
//
// PROJECT: P142 - Elemos
//
// TSW Elemos Project
//
//-----------------------------------------------------------------------------

namespace AMCS.Data.Entity
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.ComponentModel;
  using System.Diagnostics;
  using System.IO;
  using System.Linq.Expressions;
  using System.Reflection;
  using System.Runtime.Serialization;
  using System.Runtime.Serialization.Formatters.Binary;
  using System.Text;
  using System.Xml.Serialization;
  using AMCS.Data.Entity.Validation;
  using Util.Extension;
  using CsvHelper.Configuration.Attributes;
  using Newtonsoft.Json;

  [Serializable]
  [DataContract(Namespace = "http://www.solutionworks.co.uk/wims")]
  [JsonObject(MemberSerialization.OptOut)]
  // [KnownType("GetKnownTypes")]
  public class EntityObject : ICloneable, INotifyPropertyChanged, IDataErrorInfo, ILoadedPropertyValues
  {
    #region const

    public static readonly string DontTranslatePrefix = "!£$%NoTran:";

    #endregion const

    #region constructor

    /// <summary>
    /// Constructor
    /// </summary>
    public EntityObject()
    {
      SetDefaults();
    }

    /// <summary>
    /// Constructs an object populating all properties with the values in "initialiseWith".
    /// N.B. The newly constructed object is not a clone of "initialiseWith" any "By Ref" properties will, in both objects, point to the same constructs.
    /// </summary>
    /// <param name="initialiseWith"></param>
    public EntityObject(object initialiseWith)
    {
      EntityObjectInitializer.ForTypes(initialiseWith.GetType(), GetType()).Initialize(initialiseWith, this);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityObject"/> class.
    /// </summary>
    /// <param name="initialiseWith">The initialise with.</param>
    /// <param name="commonInterface">The common interface.</param>
    public EntityObject(object initialiseWith, Type commonInterface)
    {
      // Not sure if this is correct. This should still give the source and
      // target type, but use the interface to get the list of properties. However,
      // how this should work is that this type and the initialiseWith should
      // implement the provided interface, which would ensure that the below
      // call will just work.
      EntityObjectInitializer.ForTypes(commonInterface, commonInterface).Initialize(initialiseWith, this);
    }

    #endregion constructor

    #region INotifyPropertyChanged

    [field: NonSerialized]
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Attempts to raise the PropertyChanged event, and
    /// invokes the virtual AfterPropertyChanged method.
    /// </summary>
    /// <param name="propertyName">
    /// The property which was changed.
    /// </param>
    public void NotifyChange(string propertyName)
    {
      this.VerifyProperty(propertyName);
      if (this.PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      this.AfterPropertyChanged(propertyName);
    }

    /// <summary>
    /// Attempts to raise the PropertyChanged event, and
    /// invokes the virtual AfterPropertyChanged method.
    /// </summary>
    /// <typeparam name="T">Class type - no need to provide any more (see example)</typeparam>
    /// <param name="propertyPointer">Property expression, for example (() => CallTypeRestrictionId)</param>
    public void NotifyChange<T>(Expression<Func<T>> property)
    {
      string propertyName = property.GetPropertyName<T>();
      NotifyChange(propertyName);
    }

    /// <summary>
    /// Derived classes can override this method to execute logic after EntityObject property is set. The base implementation does nothing.
    /// </summary>
    /// <param name="propertyName">
    /// The property which was changed.
    /// </param>
    protected virtual void AfterPropertyChanged(string propertyName)
    {
    }

    #region debug

    /// <summary>
    /// Debug only to verify name of property on notify change events.
    /// </summary>
    [Conditional("DEBUG")]
    private void VerifyProperty(string propertyName)
    {
      Type type = this.GetType();

      // Look for EntityObject public property with the specified name.
      PropertyInfo propInfo = type.GetProperty(propertyName);

      if (propInfo == null)
      {
        // The property could not be found,
        // so alert the developer of the problem.

        string msg = string.Format(
            "{0} is not EntityObject public property of {1}",
            propertyName,
            type.FullName);

        Debug.Fail(msg);
      }
    }

    #endregion debug

    #endregion INotifyPropertyChanged

    #region ICloneable

    /// <summary>
    /// Supports cloning, which creates EntityObject new instance of EntityObject class with the same value as an existing instance.
    /// </summary>
    public object Clone()
    {
      return EntityObjectAccessor.ForType(GetType()).CreateClone(this);
    }

    public object CloneWithoutKeys()
    {
      string keyName = this.GetKeyName();

      if (keyName == null)
      {
        keyName = string.Empty;
      }

      string GuidName = "GUID";

      string[] keyArray = new string[] { keyName, GuidName };
      return this.Clone(keyArray);
    }

    public object Clone(params string[] suppressedProperties)
    {
      object clone = this.Clone();

      foreach (string suppressedProperty in suppressedProperties)
      {
        var property = clone.GetType().GetProperty(suppressedProperty);
        if (property != null)
        {
          // will set reference types to null and value types to default value
          property.SetValue(clone, null, null);
        }
      }

      return clone;
    }

    #endregion ICloneable

    #region IDataErrorInfo

    /// <summary>
    /// ValidatingProperty is fired before validation kicks in for a particular property on an EntityObject.
    /// If you wish to perform some validation check that is not provided in some EntityObject then register for this event.
    /// Within your event handler perform whatever validation you wish.  Update the ValidationEventArgs "Error" parameter with any error message
    /// and set the "Handled" property to "true" to signify that validation has already been performed and should not be performed by the EntityObject.
    /// </summary>
    [NonSerialized]
    [XmlIgnore]
    public ValidationEventHandler ValidatingProperty;

    [NonSerialized]
    [XmlIgnore]
    public ValidationEventHandler RevalidateProperty;

    /// <summary>
    /// If you have conditional validation. This method can be used to update the UI.
    /// Just applies when the Entity is opened in an editor
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    public void ValidateProperty(string propertyName)
    {
      this.VerifyProperty(propertyName);
      if (this.RevalidateProperty != null)
      {
        RevalidateProperty(this, new ValidationEventArgs { PropertyName = propertyName, Entity = this });
      }
    }

    /// <summary>
    /// If you have conditional validation. This method can be used to update the UI.
    /// Just applies when the Entity is opened in an editor
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="property">The property.</param>
    public void ValidateProperty<T>(Expression<Func<T>> property)
    {
      string propertyName = property.GetPropertyName<T>();
      ValidateProperty(propertyName);
    }

    public virtual bool IsPropertyValid(string propertyName)
    {
      return string.IsNullOrEmpty(GetValidationError(propertyName));
    }

    protected virtual string GetValidationError(string propertyName)
    {
      var entityObjectValidator = DataServices.Resolve<IEntityObjectValidator>();

      if (this is IValidatesInContext contextValidator && contextValidator.ValidationContext?.ValidationContextId != null)
      {
        int validationContextId = contextValidator.ValidationContext.ValidationContextId.Value;
        
        if (entityObjectValidator.IsTypeValidated(this.GetType(), validationContextId))
          return entityObjectValidator.GetPropertyValidationError(this, validationContextId, propertyName);
      }
      else
      {
        if (entityObjectValidator.IsTypeValidated(this.GetType()))
          return entityObjectValidator.GetPropertyValidationError(this, propertyName);
      }
      
      return null;
    }

    protected virtual string GetValidationErrors()
    {
      StringBuilder sb = new StringBuilder();
      string validationError;

      foreach (string property in GetValidatedProperties() ?? new string[0])
      {
        validationError = GetValidationError(property);
        if (validationError != null)
          sb.Append(validationError + Environment.NewLine);
      }

      if (sb.Length > 0)
        return sb.ToString();
      else
        return null;
    }

    public virtual string[] GetValidatedProperties()
    {
      var entityObjectValidator = DataServices.Resolve<IEntityObjectValidator>();

      if (this is IValidatesInContext contextValidator && contextValidator.ValidationContext?.ValidationContextId != null)
      {
        int validationContextId = contextValidator.ValidationContext.ValidationContextId.Value;

        if (entityObjectValidator.IsTypeValidated(this.GetType(), validationContextId))
          return entityObjectValidator.GetValidatedProperties(this.GetType(), validationContextId);
      }
      else
      {
        if (entityObjectValidator.IsTypeValidated(this.GetType()))
          return entityObjectValidator.GetValidatedProperties(this.GetType());
      }

      return null;
    }

    private string[] GetAllValidatedProperties()
    {
      string[] standardProps = GetValidatedProperties();
      var entityObjectValidator = DataServices.Resolve<IEntityObjectValidator>();

      if (this is IValidatesInContext contextValidator && contextValidator.ValidationContext?.ValidationContextId != null)
      {
        int validationContextId = contextValidator.ValidationContext.ValidationContextId.Value;
        if (entityObjectValidator.IsTypeValidated(this.GetType(), validationContextId))
        {
          string[] additionalProps = entityObjectValidator.GetValidatedProperties(this.GetType(), validationContextId);
          if (additionalProps != null && additionalProps.Length > 0)
          {
            List<string> allValidatedProps = new List<string>(standardProps);
            foreach (string additionalProp in additionalProps)
            {
              if (!allValidatedProps.Contains(additionalProp))
                allValidatedProps.Add(additionalProp);
            }
            return allValidatedProps.ToArray();
          }
        }
      }
      else
      {
        if (entityObjectValidator.IsTypeValidated(this.GetType()))
        {
          string[] additionalProps = entityObjectValidator.GetValidatedProperties(this.GetType());
          if (additionalProps != null && additionalProps.Length > 0)
          {
            List<string> allValidatedProps = new List<string>(standardProps);
            foreach (string additionalProp in additionalProps)
            {
              if (!allValidatedProps.Contains(additionalProp))
                allValidatedProps.Add(additionalProp);
            }
            return allValidatedProps.ToArray();
          }
        }
      }

      return standardProps;
    }

    string IDataErrorInfo.Error { get { return GetValidationErrors(); } }

    string IDataErrorInfo.this[string propertyName]
    {
      get
      {
        this.VerifyProperty(propertyName);

        ValidationEventArgs e = null;
        if (ValidatingProperty != null)
        {
          e = new ValidationEventArgs();
          e.Entity = this;
          e.PropertyName = propertyName;
          ValidatingProperty(this, e);
        }
        if (e != null && e.Handled)
          return !string.IsNullOrEmpty(e.Error) ? DontTranslatePrefix + e.Error : null;

        var entityObjectValidator = DataServices.Resolve<IEntityObjectValidator>();

        if (this is IValidatesInContext contextValidator && contextValidator.ValidationContext?.ValidationContextId != null)
        {
          int validationContextId = contextValidator.ValidationContext.ValidationContextId.Value;
          if (entityObjectValidator.IsTypePropertyValidated(this.GetType(), validationContextId, propertyName))
            return entityObjectValidator.GetPropertyValidationError(this, validationContextId, propertyName);
        }
        else
        {
          if (entityObjectValidator.IsTypePropertyValidated(this.GetType(), propertyName))
            return entityObjectValidator.GetPropertyValidationError(this, propertyName);
        }

        return this.GetValidationError(propertyName);
      }
    }

    /// <summary>
    /// Returns true if this object has no validation errors.
    /// </summary>
    [Ignore]
    [JsonIgnore]
    public bool IsValid
    {
      get
      {
        string[] vp = GetAllValidatedProperties();
        if (vp == null)
          return true;
        foreach (string property in vp)
          if (((IDataErrorInfo)this)[property] != null)
            return false;

        return true;
      }
    }

    #endregion IDataErrorInfo

    #region public methods

    /// <summary>
    /// Returns EntityObject name of underlying table related to this object
    /// </summary>
    /// <returns>Table name or empty string if not applicable.</returns>
    public virtual string GetTableName()
    {
      return EntityMetadataReader.GetTableName(GetType());
    }

    /// <summary>
    /// Returns the schema of the underlying table related to this object.
    /// </summary>
    /// <returns>Schema name. Default implementation returns "dbo".</returns>
    public virtual string GetSchemaName()
    {
      return EntityMetadataReader.GetSchemaName(GetType());
    }

    /// <summary>
    /// Convenience method to get table name qualified with schema name.
    /// </summary>
    /// <returns></returns>
    public string GetTableNameWithSchema()
    {
      return GetSchemaName() + "." + GetTableName();
    }

    /// <summary>
    /// Returns whether inserts are tracked in entity history
    /// </summary>
    /// <returns></returns>
    public virtual bool GetTrackInserts()
    {
      return EntityMetadataReader.GetTrackInserts(GetType());
    }

    /// <summary>
    /// Returns whether updates are tracked in entity history
    /// </summary>
    /// <returns></returns>
    public virtual bool GetTrackUpdates()
    {
      return EntityMetadataReader.GetTrackUpdates(GetType());
    }

    /// <summary>
    /// Returns whether deletes are tracked in entity history
    /// </summary>
    /// <returns></returns>
    public virtual bool GetTrackDeletes()
    {
      return EntityMetadataReader.GetTrackDeletes(GetType());
    }

    /// <summary>
    /// Returns EntityObject name of underlying table primary key name related to this object
    /// </summary>
    /// <returns>primary key name or empty string if not applicable.</returns>
    public virtual string GetKeyName()
    {
      return EntityMetadataReader.GetKeyField(GetType());
    }

    public virtual string GetNiceName()
    {
      string niceName = "";
      string className = this.GetType().Name;
      int entityStringIndex = className.IndexOf("Entity");
      className = entityStringIndex < 0 ? className : className.Substring(0, entityStringIndex);

      for (int i = 0; i < className.Length; i++)
      {
        if (i > 0 && char.IsUpper(className[i]))
          niceName += ' ';
        niceName += className[i];
      }

      return niceName;
    }

    /// <summary>
    /// Returns EntityObject the primary key id of the object, or null
    /// </summary>
    /// <returns>the if or null if not applicable.</returns>
    public virtual int? GetId()
    {
      throw new NotSupportedException("GetId()");
    }

    public virtual void SetId(int? id)
    {
      throw new NotSupportedException("SetId()");
    }

    /// <summary>
    /// Override to set defaults when creating new instance of
    /// </summary>
    public virtual void SetDefaults()
    {
    }

    /// <summary>
    /// Checks this instance of entity for any validation errors.
    /// </summary>
    /// <param name="errors">out- a dictionary of errors found</param>
    /// <returns>true if validation is OK false if validation fails caller then uses errors for messages.</returns>
    public virtual bool RequiredFieldsEntered(out Dictionary<string, string> errors)
    {
      errors = new Dictionary<string, string>();

      string[] validatedProperties = GetAllValidatedProperties();
      if (validatedProperties == null || validatedProperties.Length == 0)
        return true;
      foreach (string property in validatedProperties)
      {
        string validationError = ((IDataErrorInfo)this)[property];
        if (validationError != null)
          errors.Add(property, validationError);
      }

      if (errors.Count > 0)
        return false;

      return true;
    }

    public void OverwriteWith(EntityObject entity)
    {
      if (!this.GetType().Equals(entity.GetType()))
        throw new Exception("The arguments type (" + entity.GetType().FullName + ") must be the same as the containing type (" + this.GetType().FullName + ")");

      foreach (PropertyInfo prop in this.GetType().GetProperties())
      {
        if (prop.GetSetMethod() != null)
        {
          object overwriteValue = prop.GetValue(entity, null);
          prop.SetValue(this, overwriteValue, null);
        }
      }
    }

    /// <summary>
    /// This is very inefficient and not guaranteed to work with all naming conventions, it is only to be used for during testing.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private bool DoEqualsIgnoreCollectionsAndPrimaryKey(object obj, ObservableCollection<string> ignoreFields, out string fieldName)
    {
      fieldName = "";

      if (!obj.GetType().Equals(this.GetType()))
        return false;
      EntityObject that = (EntityObject)obj;

      FieldInfo[] thisFields = this.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
      FieldInfo[] thatFields = that.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

      for (int i = 0; i < thisFields.Length; i++)
      {
        FieldInfo thisField = thisFields[i];
        FieldInfo thatField = thatFields[i];

        // check to see if this field is a primary key...not totally simple but since we're looking at fields this
        // is the only way to check.
        bool isPrimaryKeyField = false;
        string keyName = GetKeyName();
        PropertyInfo[] props = this.GetType().GetProperties();
        foreach (PropertyInfo prop in props)
        {
          if (prop.Name.Replace("_", "").ToLower().Equals(thisField.Name.Replace("_", "").ToLower()))
          {
            if (EntityMetadataReader.GetMemberColumnName(prop) == keyName)
            {
              isPrimaryKeyField = true;
              break;
            }
          }
        }

        // if we're looking at the primary key then just move to the next field
        if (isPrimaryKeyField)
          continue;

        fieldName = thatField.Name;

        if ((ignoreFields != null) && (ignoreFields.Contains(fieldName.ToUpper().Replace("_", ""))))
          continue;

        Object thisFieldValue = thisField.GetValue(this);
        Object thatFieldValue = thatField.GetValue(that);

        if (thisFieldValue == null && thatFieldValue == null)
          continue;

        if (thisFieldValue == null && thatFieldValue != null)
        {
          return false;
        }

        Type dictionaryType = thisField.FieldType.GetInterface("ICollection");
        // if this type is a collection then skip it
        if (dictionaryType == null)
        {
          if (thisFieldValue is DateTime)
          {
            if (!(thisFieldValue.ToString() == thatFieldValue.ToString()))
              return false;
          }
          else if (!thisFieldValue.Equals(thatFieldValue))
            return false;
        }
      }
      fieldName = "";
      return true;
    }

    /// <summary>
    /// This is very inefficient and not guaranteed to work with all naming conventions, it is only to be used for during testing.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public virtual bool EqualsIgnoreCollectionsAndPrimaryKey(object obj)
    {
      string fieldName = "";
      return DoEqualsIgnoreCollectionsAndPrimaryKey2(obj, null, out fieldName);
    }

    /// <summary>
    /// This is very inefficient and not guaranteed to work with all naming conventions, it is only to be used for during testing.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public virtual bool EqualsIgnoreCollectionsAndPrimaryKey(object obj, out string fieldName)
    {
      return DoEqualsIgnoreCollectionsAndPrimaryKey2(obj, null, out fieldName);
    }

    /// <summary>
    /// This is very inefficient and not guaranteed to work with all naming conventions, it is only to be used for during testing.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public virtual bool EqualsIgnoreCollectionsAndPrimaryKey(object obj, ObservableCollection<string> ignoreFields, out string fieldName)
    {
      return DoEqualsIgnoreCollectionsAndPrimaryKey2(obj, ignoreFields, out fieldName);
    }

    /// <summary>
    /// This is very inefficient and not guaranteed to work with all naming conventions, it is only to be used for during testing.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private bool DoEqualsIgnoreCollectionsAndPrimaryKey2(object obj, ObservableCollection<string> ignoreFields, out string fieldName)
    {
      fieldName = "";
      if (!obj.GetType().Equals(this.GetType()))
        return false;

      EntityObject thatObj = (EntityObject)obj;

      bool result = true;
      PropertyInfo[] props = this.GetType().GetProperties();
      for (int i = 0; i < props.Length; i++)
      {
        PropertyInfo prop = props[i];

        // if this property is dynamic column ignore
        if (EntityMetadataReader.IsMemberDynamic(prop))
        {
          continue;
        }

        if (this.GetKeyName().Equals(prop.Name) || prop.Name.Equals("Id") || prop.Name.Equals("Id32"))
        {
          continue;
        }

        if ((ignoreFields != null) && (ignoreFields.Contains(prop.Name.ToUpper().Replace("_", ""))))
          continue;

        fieldName = prop.Name;
        object thispropertyValue = prop.GetValue(this, null);
        object thatpropertyValue = prop.GetValue(thatObj, null);

        if (thispropertyValue == null && thatpropertyValue == null)
        {
          continue;
        }

        if (thispropertyValue == null && thatpropertyValue != null)
        {
          result = false;
          break;
        }

        if (thispropertyValue != null && thatpropertyValue == null)
        {
          result = false;
          break;
        }

        if ((thispropertyValue is DateTime) && (thispropertyValue.ToString() != thatpropertyValue.ToString()))
        {
          result = false;
          break;
        }
        if (!(thispropertyValue is DateTime) && (!thispropertyValue.Equals(thatpropertyValue)))
        {
          result = false;
          break;
        }
      }
      return result;
    }

    /// <summary>
    /// Return the first line of the given string for display in gridviews etc.
    /// Optionally return a substring of the first line.
    /// </summary>
    public string GetFirstLine(string text, int? substringLength = null)
    {
      string firstLine = text ?? "";
      var multiLine = !string.IsNullOrEmpty(text) && text.Contains("\r\n");

      if (multiLine)
      {
        var lines = text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length > 0)
        {
          firstLine = lines[0];
        }
      }

      bool truncated = false;
      if (substringLength != null && firstLine.Length > substringLength.Value)
      {
        firstLine = firstLine.Substring(0, substringLength.Value);
        truncated = true;
      }

      if (truncated || multiLine)
      {
        firstLine = firstLine + " ...";
      }

      return firstLine;
    }

    #endregion public methods

    #region properties

    /// <summary>
    /// The primary key id value.
    /// </summary>
    [CsvHelper.Configuration.Attributes.Ignore]
    [JsonIgnore]
    public int? Id { get { return GetId(); } }

    [CsvHelper.Configuration.Attributes.Ignore]
    [JsonIgnore]
    public int Id32 { get { return Id.GetValueOrDefault(); } }

    private Guid? _GUID;

    [DataMember(Name = "GUID")]
    [CsvHelper.Configuration.Attributes.Ignore]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public virtual Guid? GUID
    {
      get { return _GUID; }
      set { if (_GUID != value) { _GUID = value; NotifyChange(() => GUID); } }
    }

    [DynamicColumn]
    [CsvHelper.Configuration.Attributes.Ignore]
    [JsonIgnore]
    public WriteActionType WriteActionType { get; set; }

    private int? _LastChangeReasonId;

    [CsvHelper.Configuration.Attributes.Ignore]
    [DataMember(Name = "LastChangeReasonId")]
    public virtual int? LastChangeReasonId
    {
      get
      {
        return _LastChangeReasonId;
      }
      set
      {
        if (_LastChangeReasonId != value)
        {
          _LastChangeReasonId = value;
          NotifyChange("LastChangeReasonId");
        }
      }
    }
    
    // can be used just for general operations where a temporary identifier needs to be set.
    [DataMember]
    [CsvHelper.Configuration.Attributes.Ignore]
    [JsonIgnore]
    public string TempIdentifier { get; set; }

    public void SetGuidIfUnset(Guid value)
    {
      if (GUID == null || GUID == Guid.Empty)
        GUID = value;
    }

    public static Type GetEntityType(string typeName)
    {
      return Type.GetType(typeName);
    }

    [DataMember(EmitDefaultValue = false)]
    private List<EntityBlobData> pendingBlobs;

    internal void SetPendingBlob(EntityBlob blob, byte[] data)
    {
      if (pendingBlobs == null)
      {
        pendingBlobs = new List<EntityBlobData>
        {
          new EntityBlobData
          {
            BlobMemberName = blob.Metadata.BlobMemberName,
            Data = data
          }
        };
      }
      else
      {
        foreach (var blobData in pendingBlobs)
        {
          if (string.Equals(blobData.BlobMemberName, blob.Metadata.BlobMemberName, StringComparison.OrdinalIgnoreCase))
          {
            blobData.Data = data;
            return;
          }
        }

        pendingBlobs.Add(new EntityBlobData
        {
          BlobMemberName = blob.Metadata.BlobMemberName,
          Data = data
        });
      }
    }

    internal bool TryGetPendingBlob(EntityBlob blob, out byte[] data, bool remove = false)
    {
      if (pendingBlobs != null)
      {
        for (var i = 0; i < pendingBlobs.Count; i++)
        {
          var blobData = pendingBlobs[i];

          if (string.Equals(blobData.BlobMemberName, blob.Metadata.BlobMemberName, StringComparison.OrdinalIgnoreCase))
          {
            data = blobData.Data;

            if (remove)
            {
              if (pendingBlobs.Count == 1)
                pendingBlobs = null;
              else
                pendingBlobs.RemoveAt(i); 
            }

            return true;
          }
        }
      }

      data = null;
      return false;
    }

    #endregion properties

    #region Change Tracking

    [DataMember]
    [XmlIgnore]
    [JsonIgnore]
    private object[] loadedValues;

    public object[] GetLoadedPropertyValues()
    {
      return (object[])loadedValues?.Clone();
    }

    void ILoadedPropertyValues.SetLoadedPropertyValues(object[] loadedPropertyValues)
    {
      loadedValues = loadedPropertyValues;
    }

    public bool HasPropertyChanged(string propertyName)
    {
      return DataServices.Resolve<IEntityLoadedValuesService>().HasPropertyChanged(this, loadedValues, propertyName);
    }
    
    public bool HasAnyPropertyChanged()
    {
      return DataServices.Resolve<IEntityLoadedValuesService>().HasAnyPropertyChanged(this, loadedValues);
    }

    public bool SupportsTracking()
    {
      return GetTrackInserts() || GetTrackUpdates() || GetTrackDeletes();
    }


    #endregion Change Tracking
  }
}