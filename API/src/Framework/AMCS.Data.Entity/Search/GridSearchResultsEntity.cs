using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Types;

namespace AMCS.Data.Entity.Search
{
  [Serializable]
  [DataContract(Namespace = "http://www.amcsgroup.com/elemos")]
  [KnownType(typeof(SerializableDynamicObject))]
  [KnownType(typeof(DateTimeOffset))]
  [KnownType(typeof(SqlGeography))]
  public class GridSearchResultsEntity : INotifyPropertyChanged
  {
    /// <summary>
    /// Gets or sets the audit table.
    /// </summary>
    /// <value>The audit table.</value>
    public string AuditTable { get; set; }

    /// <summary>
    /// Gets or sets the audit query.
    /// </summary>
    /// <value>The audit table.</value>
    public string AuditQuery { get; set; }

    /// <summary>
    /// Gets or sets the audit key field.
    /// </summary>
    /// <value>The audit key field.</value>
    public string AuditKeyField { get; set; }

    /// <summary>
    /// Gets or sets the search result id.
    /// </summary>
    /// <value>The search result id.</value>
    public string SearchResultId { get; set; }

    private ObservableCollection<dynamic> _Collection;
    private ObservableCollection<GridColumnDefinition> _ColumnDefinitions;

    public GridSearchResultsEntity()
    {
      _Collection = new ObservableCollection<dynamic>();
      _ColumnDefinitions = new ObservableCollection<GridColumnDefinition>();
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    [DataMember]
    public string Id { get; set; }

    [DataMember]
    public ObservableCollection<dynamic> Collection
    {
      get
      {
        return _Collection;
      }
      set
      {
        _Collection = value;
        OnPropertyChanged("Collection");
      }
    }

    [DataMember]
    public ObservableCollection<GridColumnDefinition> ColumnDefinitions
    {
      get
      {
        return _ColumnDefinitions;
      }
      set
      {
        _ColumnDefinitions = value;
        OnPropertyChanged("ColumnDefinitions");
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void SetMappingFile(string mappingFileName)
    {
      foreach (GridColumnDefinition colDef in ColumnDefinitions)
      {
        colDef.ParentId = mappingFileName;
      }
    }
  }
}