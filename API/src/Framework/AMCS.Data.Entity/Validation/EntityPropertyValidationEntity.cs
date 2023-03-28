//-----------------------------------------------------------------------------
// <copyright file="EntityPropertyValidationEntity.cs" company="AMCS Group">
//   Copyright © 2014 AMCS Group. All rights reserved.
// </copyright>
//
// PROJECT: P119 - Elemos
//
// AMCS Elemos Project
//
//-----------------------------------------------------------------------------

namespace AMCS.Data.Entity.Validation
{
  using System;
  using System.Runtime.Serialization;
  using AMCS.Data.Entity.Interfaces;
  using Util.Extension;

  [Serializable]
  [DataContract(Namespace = "http://www.solutionworks.co.uk/wims")]
  [EntityTable("EntityPropertyValidation", "EntityPropertyValidationId", IdentityInsertMode = IdentityInsertMode.On)]
  public class EntityPropertyValidationEntity : EntityObject, IPositionableEntity
  {
    #region Properties

    private int? _EntityPropertyValidationId;

    [DataMember(Name = "EntityPropertyValidationId")]
    public int? EntityPropertyValidationId
    {
      get { return _EntityPropertyValidationId; }
      set { if (_EntityPropertyValidationId != value) { _EntityPropertyValidationId = value; NotifyChange(() => EntityPropertyValidationId); } }
    }

    private int? _EntityPropertyId;

    [DataMember(Name = "EntityPropertyId")]
    public int? EntityPropertyId
    {
      get { return _EntityPropertyId; }
      set { if (_EntityPropertyId != value) { _EntityPropertyId = value; NotifyChange(() => EntityPropertyId); } }
    }

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

    private string _FailValidationTest;
    [DataMember(Name = "FailValidationTest")]
    public string FailValidationTest
    {
      get { return _FailValidationTest; }
      set { if (_FailValidationTest != value) { _FailValidationTest = value; NotifyChange(() => FailValidationTest); } }
    }

    private string _ErrorText;
    [DataMember(Name = "ErrorText")]
    public string ErrorText
    {
      get { return _ErrorText; }
      set { if (_ErrorText != value) { _ErrorText = value; NotifyChange(() => ErrorText); } }
    }

    private bool mandatory;
    [DataMember(Name = "Mandatory")]
    public bool Mandatory
    {
      get { return mandatory; }
      set
      {
        if (mandatory != value)
        {
          mandatory = value;
          NotifyChange(() => Mandatory);
        }
      }
    }

    private string regex;
    [DataMember(Name = "Regex")]
    public string Regex
    {
      get { return regex; }
      set
      {
        if (regex != value)
        {
          regex = value;
          NotifyChange(() => Regex);
        }
      }
    }

    private bool display;
    [DataMember(Name = "Display")]
    public bool Display
    {
      get { return display; }
      set
      {
        if (display != value)
        {
          display = value;
          NotifyChange(() => Display);
        }
      }
    }

    private int? position;
    [DataMember(Name = "Position")]
    public int? Position
    {
      get { return position; }
      set
      {
        if (position != value)
        {
          position = value;
          NotifyChange(() => Position);
        }
      }
    }

    private bool _ExternalDisplay;
    [DataMember(Name = "ExternalDisplay")]
    public bool ExternalDisplay
    {
      get { return _ExternalDisplay; }
      set { if (_ExternalDisplay != value) { _ExternalDisplay = value; NotifyChange(() => ExternalDisplay); } }
    }

    private bool _ExternalMandatory;
    [DataMember(Name = "ExternalMandatory")]
    public bool ExternalMandatory
    {
      get { return _ExternalMandatory; }
      set { if (_ExternalMandatory != value) { _ExternalMandatory = value; NotifyChange(() => ExternalMandatory); } }
    }

    #region Dynamic Columns

    private string _ClassName;

    [DynamicColumn("ClassName")]
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

    private string _PropertyName;

    [DynamicColumn("PropertyName")]
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

    private string description;
    [DataMember(Name = "Description")]
    [DynamicColumn]
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

    private bool alwaysMandatory;
    [DataMember(Name = "AlwaysMandatory")]
    [DynamicColumn]
    public bool AlwaysMandatory
    {
      get { return alwaysMandatory; }
      set
      {
        if (alwaysMandatory != value)
        {
          alwaysMandatory = value;
          NotifyChange(() => AlwaysMandatory);
        }
      }
    }

    private bool allowRegexValidation;
    [DataMember(Name = "AllowRegexValidation")]
    [DynamicColumn]
    public bool AllowRegexValidation
    {
      get { return allowRegexValidation; }
      set
      {
        if (allowRegexValidation != value)
        {
          allowRegexValidation = value;
          NotifyChange(() => AllowRegexValidation);
        }
      }
    }

    #endregion Dynamic Columns

    #endregion Properties

    #region Validation

    public enum ValidationStrings
    {
      [StringValue("Fail Validation Test: Cannot be more than 255 characters.")]
      FailValidationTest,

      [StringValue("Error Text: Cannot be more than 255 characters.")]
      ErrorText,
    }

    private static readonly string[] ValidatedProperties =
    {
        "FailValidationTest",
        "ErrorText",
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
        case "FailValidationTest":
          if (!string.IsNullOrEmpty(FailValidationTest))
          {
            if (FailValidationTest.Length > 255)
            {
              error = ValidationStrings.FailValidationTest.GetStringValue();
            }
          }
          break;

        case "ErrorText":
          if (!string.IsNullOrEmpty(ErrorText) && ErrorText.Length > 255)
            error = ValidationStrings.ErrorText.GetStringValue();
          break;

        default:
          break;
      }

      return error;
    }

    #endregion Validation

    #region Overrides

    /// <summary>
    /// Returns the primary key value
    /// </summary>
    /// <returns>primary key value.</returns>
    public override int? GetId()
    {
      return EntityPropertyValidationId;
    }

    #endregion Overrides
  }
}