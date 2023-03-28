namespace AMCS.Data.Server.UserDefinedField
{
  using System;
  using AMCS.Data.Server.UserDefinedField.Restrictions;
  using AMCS.PluginData.Data.Metadata.UserDefinedFields;
  using AMCS.Data.Configuration;
  using Autofac;
  using AMCS.Data.Server.Plugin;
  using WriteValidations;

  public static class DataConfigurationExtensions
  {
    public static void ConfigureUserDefinedFields(this DataConfiguration self)
    {
      self.ContainerBuilder
        .RegisterType<UdfMetadataCacheService>()
        .SingleInstance()
        .AsSelf()
        .As<IUdfMetadataCacheService>();

      self.ContainerBuilder
        .RegisterType<UdfMetadataService>()
        .SingleInstance()
        .AsSelf()
        .As<IUdfMetadataService>();

      self.ContainerBuilder
        .RegisterType<UdfDataService>()
        .SingleInstance()
        .AsSelf()
        .As<IUdfDataService>();

      self.ContainerBuilder
        .RegisterType<UdfValidationService>()
        .SingleInstance()
        .AsSelf()
        .As<IUdfValidationService>();

      self.AddUdfMetadataMetadataProcessor<UdfMetadataMetadataProcessor>();

      ConfigureUdfWriteValidations(self);
      ConfigureUdfRestrictions(self);
    }

    private static void ConfigureUdfWriteValidations(this DataConfiguration self)
    {
      self.ContainerBuilder
        .RegisterType<AllFieldsExistUdfWriteValidation>()
        .SingleInstance()
        .As<IUdfWriteValidation>();

      self.ContainerBuilder
        .RegisterType<NamespaceExistsUdfWriteValidation>()
        .SingleInstance()
        .As<IUdfWriteValidation>();

      self.ContainerBuilder
        .RegisterType<RequiredFieldsPresentUdfWriteValidation>()
        .SingleInstance()
        .As<IUdfWriteValidation>();
    }

    private static void ConfigureUdfRestrictions(this DataConfiguration self)
    {
      ConfigureUdfRestriction(self, RestrictionType.MaximumLengthRestriction.ToString(), typeof(UdfMaximumLengthRestriction));
      ConfigureUdfRestriction(self, RestrictionType.MinimumLengthRestriction.ToString(), typeof(UdfMinimumLengthRestriction));
      ConfigureUdfRestriction(self, RestrictionType.MaximumValueRestriction.ToString(), typeof(UdfMaximumValueRestriction));
      ConfigureUdfRestriction(self, RestrictionType.MinimumValueRestriction.ToString(), typeof(UdfMinimumValueRestriction));
      ConfigureUdfRestriction(self, RestrictionType.MustBeFutureRestriction.ToString(), typeof(UdfMustBeFutureRestriction));
      ConfigureUdfRestriction(self, RestrictionType.MustBePastRestriction.ToString(), typeof(UdfMustBePastRestriction));
      ConfigureUdfRestriction(self, RestrictionType.MustBeNegativeRestriction.ToString(), typeof(UdfMustBeNegativeRestriction));
      ConfigureUdfRestriction(self, RestrictionType.MustBePositiveRestriction.ToString(), typeof(UdfMustBePositiveRestriction));
      ConfigureUdfRestriction(self, RestrictionType.NumericPrecisionRestriction.ToString(), typeof(UdfNumericPrecisionRestriction));
    }

    private static void ConfigureUdfRestriction(this DataConfiguration self, string restrictionType,
      Type restriction)
    {
      self.ContainerBuilder
        .RegisterType(restriction)
        .Keyed<IUdfRestriction>(restrictionType)
        .SingleInstance();
    }
  }
}