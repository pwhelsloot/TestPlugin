namespace AMCS.Data.Server.Services
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;
  using System.Xml;
  using System.Xml.Serialization;
  using AMCS.Data.Configuration;
  using Entity;

  public class BusinessObjectService : IBusinessObjectService, IDelayedStartup
  {
    private readonly List<BusinessObjectResult> internalList =
      new List<BusinessObjectResult>();

    private readonly string fileName;
    private readonly Assembly assembly;
    private readonly ITypeManager typeManager;

    public BusinessObjectService(Assembly assembly, ITypeManager typeManager, string fileName)
    {
      this.assembly = assembly;
      this.typeManager = typeManager;
      this.fileName = string.IsNullOrWhiteSpace(fileName) ? "BusinessObjects.xml" : fileName;
    }

    public void Start()
    {
      if (assembly == null)
        throw new ArgumentNullException(nameof(assembly), "Must pass in a valid assembly");

      if (typeManager == null)
        throw new ArgumentNullException(nameof(typeManager), "Must pass in a valid type manager");

      var serializer = new XmlSerializer(typeof(List<BusinessObject>), new XmlRootAttribute("BusinessObjects"));
      List<BusinessObject> businessObjects = null;

      using (var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{fileName}"))
      using (var reader = XmlReader.Create(stream))
      {
        businessObjects = (List<BusinessObject>)serializer.Deserialize(reader);
      }

      var entityTypeByBusinessObject = new Dictionary<string, List<Type>>(StringComparer.OrdinalIgnoreCase);
      var entityTypeByTableName = new Dictionary<string, List<Type>>(StringComparer.OrdinalIgnoreCase);

      foreach (var type in typeManager.GetTypes().Where(p => typeof(EntityObject).IsAssignableFrom(p) && p.CanConstruct()))
      {
        var types = new List<Type>();
        string objectName = EntityMetadataReader.GetObjectName(type);

        if (!String.IsNullOrEmpty(objectName))
        {
          if (!entityTypeByBusinessObject.TryGetValue(objectName, out types))
          {
            types = new List<Type>();
            entityTypeByBusinessObject.Add(objectName, types);
          }
          types.Add(type);
        }

        var instance = (EntityObject)Activator.CreateInstance(type);
        string tableNameWithSchema = instance.GetTableNameWithSchema();
        if (!entityTypeByTableName.TryGetValue(tableNameWithSchema, out types))
        {
          types = new List<Type>();
          entityTypeByTableName.Add(tableNameWithSchema, types);
        }
        types.Add(type);
      }

      foreach (var businessObject in businessObjects)
      {
        if (internalList.Any(item => item.BusinessObject.Name == businessObject.Name))
          throw new InvalidOperationException($"Multiple instances of business object of {businessObject.Name} found");

        if (!string.IsNullOrEmpty(businessObject.TableName) && businessObject.TableName.Split('.').Length != 2)
          throw new InvalidOperationException(
            $"Business object {businessObject.Name} has invalid TableName; Must follow [schema].[table] pattern");
        
        if (!string.IsNullOrWhiteSpace(businessObject.MappedApiEntity))
        {
          var apiType = typeManager.GetTypes().SingleOrDefault(type => type.FullName == businessObject.MappedApiEntity);
          if (apiType == null)
            throw new InvalidOperationException($"Could not find associated type for {businessObject.MappedApiEntity}");

          businessObject.MappedApiEntity = apiType.AssemblyQualifiedName;
        }

        var types = GetBusinessObjectEntityTypes(entityTypeByBusinessObject, entityTypeByTableName, businessObject).ToList();

        var result = new BusinessObjectResult(businessObject);
        result.Types.AddRange(types);

        internalList.Add(result);
      }
    }

    public IList<BusinessObjectResult> GetAll()
    {
      return new List<BusinessObjectResult>(internalList);
    }

    public BusinessObjectResult Get(string objectName)
    {
      return GetAll().SingleOrDefault(item => item.BusinessObject.Name == objectName);
    }

    public BusinessObjectResult Get(Type type)
    {
      return GetAll().SingleOrDefault(item => item.Types.Contains(type));
    }

    private IEnumerable<Type> GetBusinessObjectEntityTypes(Dictionary<string, List<Type>> entityTypeByBusinessObject, Dictionary<string, List<Type>> entityTypeByTableName, BusinessObject currentBusinessObject)
    {
      List<Type> types;
      var results = new List<Type>();

      if (entityTypeByBusinessObject.TryGetValue(currentBusinessObject.Name, out types))
        results.AddRange(types);

      if (
        !string.IsNullOrEmpty(currentBusinessObject.TableName) &&
        entityTypeByTableName.TryGetValue(currentBusinessObject.TableName, out types)
      )
      {
        foreach (Type type in types)
        {
          if (!results.Contains(type))
            results.Add(type);
        }
      }

      return results;
    }
  }
}