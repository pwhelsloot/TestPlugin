//-----------------------------------------------------------------------------
// <copyright file="MappingManager.cs" company="AMCS Group">
//   Copyright © 2010-11 AMCS Group. All rights reserved.
// </copyright>
// 
// PROJECT: P142 - Elemos
//
// AMCS Elemos Project
//
//-----------------------------------------------------------------------------

namespace AMCS.Data.Configuration.Mapping.Manager
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using AMCS.Data.Configuration.Mapping.IO;
    using AMCS.Data.Configuration.Mapping.IO.Xml;
    using AMCS.Data.Entity;

    /// <summary>
    /// See IMappingManager for further comments
    /// </summary>
    public class MappingManager : IMappingManager
  {
    #region Properties/Variables

    // A string that signifies the specific project that mapping files are being imported for.
    // each project may have custom/overriden mappings and these will be merged in after the initial general import.
    private string projectIdentifer;

    // key = owner name, value = IMapping for owner 
    private IDictionary<string, IMapping> mappings = new Dictionary<string, IMapping>();
    private IDictionary<string, IMapping> mergeMappings = null;

    private IMappingImporter importer;
    private IMappingExporter exporter;

    // A secondary IMappingImporter can be used to import mappings from a different source to what importer is looking at.
    // This is useful if you are exporting to an existing mapping storage collection and don't want to blindly overwrite the exising 
    // stored mappings.  Effectively the mappings in exporter will be compared/merged with the mappings in mergeImporter.
    private IMappingImporter mergeImporter;

    private IList<TranslationMappingOverrideEntity> databaseTranslations;

    #endregion Properties/Variables

    #region ctors/dtors

    public MappingManager(string projectIdentifier, IMappingImporter importer) : this(projectIdentifier, importer, null, null, null)
    {
    }

    public MappingManager(string projectIdentifier, IMappingImporter importer, IList<TranslationMappingOverrideEntity> databaseTranslations)
      : this(projectIdentifier, importer, null, null, databaseTranslations)
    {
    }

    public MappingManager(IMappingExporter exporter) : this(null, null, exporter, null, null)
    {
    }

    public MappingManager(IMappingImporter importer, IMappingExporter exporter) : this(null, importer, exporter, null, null)
    {
    }

    /// <summary>
    /// See comments for the local mergeImporter variable
    /// </summary>
    /// <param name="importer"></param>
    /// <param name="exporter"></param>
    /// <param name="mergeImporter"></param>
    public MappingManager(string projectIdentifier, IMappingImporter importer, IMappingExporter exporter, IMappingImporter mergeImporter, IList<TranslationMappingOverrideEntity> databaseTranslations = null)
    {
      this.projectIdentifer = projectIdentifier;
      this.importer = importer;
      this.exporter = exporter;
      this.mergeImporter = mergeImporter;
      if (this.mergeImporter != null)
      {
        this.mergeMappings = this.mergeImporter.ImportAll();
      }

      this.databaseTranslations = databaseTranslations;
    }

    #endregion ctors/dtors

    #region Import/Export

    /// <summary>
    /// See IMappingManager
    /// </summary>
    public void ImportAll()
    {
      this.ImportAll(this.importer);
    }

    /// <summary>
    /// See IMappingManager
    /// </summary>
    /// <param name="importer"></param>
    public void ImportAll(IMappingImporter importer)
    {
      if (importer == null)
      {
        throw new ArgumentNullException("No valid IMappingImporter has been provided, current importer is null.");
      }

      this.mappings = importer.ImportAll();

      if (!string.IsNullOrWhiteSpace(this.projectIdentifer))
      {
        this.MergeProjectSpecificMappings(importer);
      }

      this.MergeDatabaseMappings(this.databaseTranslations);
    }

    public void MergeDatabaseMappings(IList<TranslationMappingOverrideEntity> databaseTranslations)
    {
      if (databaseTranslations != null)
      {
        foreach (TranslationMappingOverrideEntity databaseTranslation in databaseTranslations.Where(dt => !string.IsNullOrWhiteSpace(dt.ProjectIdentifier)))
        {
          if (this.mappings.ContainsKey(databaseTranslation.MappingId))
          {
            if (this.projectIdentifer.Equals(databaseTranslation.ProjectIdentifier))
            {
              this.mappings[databaseTranslation.MappingId].MergeWith(this.importer.ImportXML(XElement.Parse(databaseTranslation.Mapping)));
            }
          }
        }
      }
    }

    /// <summary>
    /// See IMappingManager
    /// </summary>
    /// <param name="mappingName"></param>
    public void Import(string mappingName)
    {
      this.Import(mappingName, this.importer);
    }

    /// <summary>
    /// See IMappingManager
    /// </summary>
    /// <param name="mappingName"></param>
    /// <param name="importer"></param>
    public void Import(string mappingName, IMappingImporter importer)
    {
      if (importer == null)
      {
        throw new ArgumentNullException("No valid IMappingImporter has been provided, current importer is null.");
      }

      IMapping mapping = importer.Import(mappingName);
      if (mapping == null)
      {
        throw new Exception("No mapping of name '" + mappingName + "' found to import");
      }

      this.mappings.Add(mappingName, mapping);
    }

    /// <summary>
    /// See IMappingManager
    /// </summary>
    /// <returns></returns>
    public KeyValuePair<IList<IMapping>, IList<Exception>> ExportAll()
    {
      return this.ExportAll(this.exporter);
    }

    /// <summary>
    /// See IMappingManager
    /// </summary>
    /// <param name="exporter"></param>
    /// <returns></returns>
    public KeyValuePair<IList<IMapping>, IList<Exception>> ExportAll(IMappingExporter exporter)
    {
      try
      {
        if (exporter == null)
        {
          throw new ArgumentNullException("No valid IMappingExporter has been provided, current exporter is null.");
        }

        if (this.mappings == null || this.mappings.Count == 0)
        {
          throw new Exception("There are no mappings to export");
        }

        if (this.mergeImporter != null)
        {
          this.MergeMappings();
        }

        return exporter.ExportAll(this.mappings.Values.ToList());
      }
      catch (Exception ex)
      {
        KeyValuePair<IList<IMapping>, IList<Exception>> result = new KeyValuePair<IList<IMapping>, IList<Exception>>(new List<IMapping>(), new List<Exception>());
        result.Value.Add(ex);
        return result;
      }
    }

    /// <summary>
    /// See IMappingManager
    /// </summary>
    /// <param name="mappingName"></param>
    public void Export(string mappingName)
    {
      this.Export(mappingName, this.exporter);
    }

    /// <summary>
    /// See IMappingManager
    /// </summary>
    /// <param name="mappingName"></param>
    /// <param name="exporter"></param>
    public void Export(string mappingName, IMappingExporter exporter)
    {
      if (exporter == null)
      {
        throw new ArgumentNullException("No valid IMappingExporter has been provided, current exporter is null.");
      }

      if (this.mappings == null || !this.mappings.ContainsKey(mappingName))
      {
        throw new Exception("There is no mapping of name '" + mappingName + "' to export");
      }

      if (this.mergeImporter != null)
      {
        this.MergeMapping(mappingName);
      }

      exporter.Export(this.mappings[mappingName]);
    }

    #endregion Import/Export

    #region Mapping Access

    /// <summary>
    /// See IMappingManager
    /// </summary>
    /// <param name="mappingName"></param>
    /// <returns></returns>
    public bool IsMapped(string mappingName)
    {
      return this.mappings.ContainsKey(mappingName);
    }

    /// <summary>
    /// See IMappingManager
    /// </summary>
    /// <param name="mappingName"></param>
    /// <returns></returns>
    public IMapping GetMapping(string mappingName)
    {
      if (!this.IsMapped(mappingName))
      {
        return null;
      }

      return this.mappings[mappingName];
    }

    /// <summary>
    /// See IMappingManager
    /// </summary>
    /// <returns></returns>
    public IDictionary<string, IMapping> GetMappings()
    {
      return this.mappings;
    }

    /// <summary>
    /// See IMappingManager
    /// </summary>
    /// <param name="mappings"></param>
    public void SetMappings(IDictionary<string, IMapping> mappings)
    {
      this.mappings = mappings;
    }

    #endregion Mapping Access

    #region General

    /// <summary>
    /// See IMappingManager
    /// </summary>
    public void SortMappings()
    {
      this.mappings = this.mappings.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    /// <summary>
    /// Will action a "MergeMapping(...) for every mapping held internally
    /// </summary>
    public virtual void MergeMappings()
    {
      if (this.mappings == null)
      {
        return;
      }

      // Copy the keys to an array so that the mappings structure can be modified within the loop
      string[] loopKeys = this.mappings.Keys.ToArray();
      foreach (string mappingName in loopKeys)
      {
        this.MergeMapping(mappingName);
      }
    }

    /// <summary>
    /// Actions a merge of a mapping held internally with it's representation as seen by the "mergeImporter" - see comments for mergeImporter variable.
    /// </summary>
    /// <param name="mappingName"></param>
    public virtual void MergeMapping(string mappingName)
    {
      if (this.mergeImporter == null)
      {
        throw new Exception("There is no instance of a merge importer, refer to the constructors and call the correct one if you wish to perform merging.");
      }

      if (this.mappings.ContainsKey(mappingName))
      {
        try
        {
          IMapping newMapping = this.mappings[mappingName];
          IMapping existingMapping = null;
          if (this.mergeMappings.ContainsKey(mappingName))
          {
            existingMapping = this.mergeMappings[mappingName];
          }

          if (existingMapping != null)
          {
            existingMapping.MergeWith(newMapping);
            this.mappings[mappingName] = existingMapping;
          }
        }
        catch (FileNotFoundException)
        {
          // Just means there is no existing mapping
        }
        catch (Exception ex)
        {
          throw new Exception("Problem merging '" + mappingName + "': " + ex.Message, ex);
        }
      }
    }

    #endregion General

    #region Customised Mappings (Project Specific)

    /// <summary>
    /// Merges the project specific mappings.
    /// It looks in a directory under the current mapping directory with a name matching the string held in "_projectIdentifier" and reads in any mappings.
    /// These mappings are then merged into the general set of mappings, the project specific ones overwriting the general ones.
    /// </summary>
    /// <param name="importer">The importer.</param>
    private void MergeProjectSpecificMappings(IMappingImporter importer)
    {
      if (string.IsNullOrWhiteSpace(this.projectIdentifer))
      {
        return;
      }

      var embeddedResources = EmbeddedResourceHelper.GetEmbeddedResourcePaths(importer.XmlAssembly, importer.XmlNamespace, XmlMappingImporter.XmlFileExtension);

      foreach (var fileName in embeddedResources.Where(e => e.ProjectIdentifier == this.projectIdentifer).Select(e => e.FileName))
      {
        IMapping customMapping = importer.Import(fileName);
        if (this.mappings.ContainsKey(customMapping.Id))
        {
          IMapping generalMapping = this.mappings[customMapping.Id];
          generalMapping.MergeWith(customMapping);
        }
      }
    }

    #endregion Customised Mappings (Project Specific)
  }
}
