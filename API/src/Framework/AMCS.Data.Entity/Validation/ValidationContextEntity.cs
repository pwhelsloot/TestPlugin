namespace AMCS.Data.Entity.Validation
{
  using System;
  using System.Collections.ObjectModel;
  using System.Runtime.Serialization;
  using Util.Extension;

  [Serializable]
  [DataContract(Namespace = "http://www.solutionworks.co.uk/elemos")]
  [EntityTable("ValidationContext", "ValidationContextId", IdentityInsertMode = IdentityInsertMode.On)]
  public class ValidationContextEntity : EntityObject
  {
    #region Attributes

    private int? validationContextId;
    [DataMember(Name = "ValidationContextId")]
    public int? ValidationContextId
    {
      get { return validationContextId; }
      set
      {
        if (validationContextId != value)
        {
          validationContextId = value;
          NotifyChange(() => ValidationContextId);
        }
      }
    }

    private int? entityId;
    [DataMember(Name = "EntityId")]
    public int? EntityId
    {
      get { return entityId; }
      set
      {
        if (entityId != value)
        {
          entityId = value;
          NotifyChange(() => EntityId);
        }
      }
    }

    private bool _IsDeleted;
    [DataMember(Name = "IsDeleted")]
    public bool IsDeleted
    {
      get { return _IsDeleted; }
      set { if (_IsDeleted != value) { _IsDeleted = value; NotifyChange(() => IsDeleted); } }
    }

    private string description;
    [DataMember(Name = "Description")]
    public string Description
    {
      get { return description; }
      set
      {
        if (description != value)
        {
          description = value;
          NotifyChange(() => Description);
        }
      }
    }

    private string className;
    [DataMember(Name = "ClassName")]
    [DynamicColumn]
    public string ClassName
    {
      get { return className; }
      set
      {
        if (className != value)
        {
          className = value;
          NotifyChange(() => ClassName);
        }
      }
    }

    private ObservableCollection<EntityPropertyValidationEntity> validations;
    [DynamicColumn]
    [DataMember(Name = "Validations")]
    public ObservableCollection<EntityPropertyValidationEntity> Validations
    {
      get { return validations; }
      set
      {
        if (validations != value)
        {
          validations = value;
          NotifyChange(() => Validations);
        }
      }
    }

    #endregion

    #region validation

    public enum ValidationStrings
    {
      [StringValue("Description: Must be no more than 255 characters long.")]
      Description
    }

    private static readonly string[] ValidatedProperties =
    {
      "Description",
    };

    /// <summary>
    /// Returns an array of properties to validated
    /// </summary>
    /// <returns>array of property names to validated</returns>
    public override string[] GetValidatedProperties()
    {
      return ValidatedProperties;
    }

    /// <summary>
    /// Returns an array of properties to validated
    /// </summary>
    /// <param name="propertyName">Name of property to be validated.</param>
    /// <returns>error string null if no error</returns>
    protected override string GetValidationError(string propertyName)
    {
      string error = null;

      switch (propertyName)
      {
        case "Description":
          if ((string.IsNullOrWhiteSpace(Description)) || (Description.Length > 255))
            error = ValidationStrings.Description.GetStringValue();
          break;

        default:
          break;
      }

      return error;
    }

    #endregion validation

    #region tableNames

    public override int? GetId()
    {
      return ValidationContextId;
    }

    #endregion tableNames
  }
}
