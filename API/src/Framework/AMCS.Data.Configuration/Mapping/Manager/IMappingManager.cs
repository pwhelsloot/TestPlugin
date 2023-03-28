//-----------------------------------------------------------------------------
// <copyright file="IMappingManager.cs" company="AMCS Group">
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
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using AMCS.Data.Configuration.Mapping.IO;

  public interface IMappingManager
  {
    /// <summary>
    /// Implementation must import 'mappingName' using a default importer
    /// </summary>
    /// <param name="mappingName"></param>
    void Import(string mappingName);

    /// <summary>
    /// Implementation must import 'mappingName' using 'importer'
    /// </summary>
    /// <param name="mappingName"></param>
    /// <param name="importer"></param>
    void Import(string mappingName, IMappingImporter importer);

    /// <summary>
    /// Implementation must import all mappings using a default importer
    /// </summary>
    void ImportAll();

    /// <summary>
    /// Implementation must import all mappings using 'importer'
    /// </summary>
    /// <param name="importer"></param>
    void ImportAll(IMappingImporter importer);

    /// <summary>
    /// Implementation must export 'mappingName' with a default exporter.
    /// </summary>
    /// <param name="mappingName"></param>
    void Export(string mappingName);

    /// <summary>
    /// Implementation must export 'mappingName' using 'exporter'
    /// </summary>
    /// <param name="mappingName"></param>
    /// <param name="exporter"></param>
    void Export(string mappingName, IMappingExporter exporter);

    /// <summary>
    /// Implementation must export all mappings using a default exporter
    /// </summary>
    /// <returns></returns>
    KeyValuePair<IList<IMapping>, IList<Exception>> ExportAll();

    /// <summary>
    /// Implementation must export all mappings using 'exporter'
    /// </summary>
    /// <param name="exporter"></param>
    /// <returns></returns>
    KeyValuePair<IList<IMapping>, IList<Exception>> ExportAll(IMappingExporter exporter);

    /// <summary>
    /// Implementation must return true if 'mappingName' is mapped internally
    /// </summary>
    /// <param name="mappingName"></param>
    /// <returns></returns>
    bool IsMapped(string mappingName);

    /// <summary>
    /// Implementation must return a reference to a mapping with name 'mappingName', assuming 'mappingName' is mapped.
    /// It is up to the implementor what to do if a mapping does not exist.
    /// </summary>
    /// <param name="mappingName"></param>
    /// <returns></returns>
    IMapping GetMapping(string mappingName);

    /// <summary>
    /// Implementation must return a reference to the mappings held interally
    /// </summary>
    /// <returns></returns>
    IDictionary<string, IMapping> GetMappings();

    /// <summary>
    /// Implementation must overwrite the current mappings with "mappings"
    /// </summary>
    /// <param name="mappings"></param>
    void SetMappings(IDictionary<string, IMapping> mappings);

    /// <summary>
    /// Must sort the mappings held internally.  The order is left up to the implementor.
    /// </summary>
    void SortMappings();
  }
}
