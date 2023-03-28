namespace AMCS.Data.Util.GUI
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Reflection;

  public class DataEntryDetector : IDisposable
  {
    private const string DONT_MONITOR_PROPERTY_NOTIFICATION = "&!#DONTMONITOR#!&";
    private INotifyPropertyChanged _watchedObject = null;
    //originalValues and propertyHistory Dictionary Keys are a reference to the Property's <object ParentObject, PropertyInfo PropertyMetadata>
    private Dictionary<KeyValuePair<object, PropertyInfo>, object> _originalValues = new Dictionary<KeyValuePair<object,PropertyInfo>, object>();
    private Dictionary<KeyValuePair<object,PropertyInfo>, Stack<object>> _propertyHistories = new Dictionary<KeyValuePair<object,PropertyInfo>, Stack<object>>();
    private Stack<KeyValuePair<object,PropertyInfo>> _undoOrder = new Stack<KeyValuePair<object,PropertyInfo>>();

    private KeyValuePair<object,PropertyInfo>? _lastUndidProperty = null;
    private bool _isUndoing = false;
    private bool _isUndoLastAction = false;
    private bool _isUndoAllowed = false;
    private bool _hasElementChanged = false;
    private bool _isDataModified = false;

    public delegate void PropertyChangedHandler(object sender, DataChangedEventArgs e);
    public delegate void UndoStatusChangedHandler(object sender, DataChangedEventArgs e);
    private PropertyChangedHandler PropertyChanged;
    private UndoStatusChangedHandler UndoStatusChanged;

    /// <summary>
    /// TODO: This needs to be updated so that it works with nested objects/properties, i.e. Weighing.Job.ContainerId
    /// </summary>
    /// <param name="watchedObject"></param>
    /// <param name="propertyChangedHandler"></param>
    /// <param name="undoStatusChangedHandler"></param>
    public DataEntryDetector(INotifyPropertyChanged watchedObject, PropertyChangedHandler propertyChangedHandler, UndoStatusChangedHandler undoStatusChangedHandler)
    {
      _watchedObject = watchedObject;
      PropertyChanged = propertyChangedHandler;
      UndoStatusChanged = undoStatusChangedHandler;

      _watchedObject.PropertyChanged += OnWatchedPropertyChanged;
      foreach (PropertyInfo pInfo in _watchedObject.GetType().GetProperties())
      {
        if (pInfo.GetSetMethod() == null) continue; //Ignore read-only properties

        object _watchedObjectProperty = pInfo.GetValue(_watchedObject, null);
        if (_watchedObjectProperty is INotifyPropertyChanged && !(_watchedObjectProperty is System.Collections.IEnumerable))
        {
          string parentName = pInfo.Name; //Required for lambda-expression closure.
          (_watchedObjectProperty as INotifyPropertyChanged).PropertyChanged += (sender, e) => OnWatchedPropertyChanged(sender, e, parentName);
        }
      }

      CollectOriginalData(_watchedObject);
    }

    /// <summary>
    /// 
    /// </summary>
    private void CollectOriginalData(object parent)
    {

      PropertyInfo[] props = parent.GetType().GetProperties();
      foreach (PropertyInfo prop in props)
      {
        if (prop.GetSetMethod() == null) continue; //Ignore read-only properties

        object o = prop.GetValue(parent, null);
        if (o is INotifyPropertyChanged && !(o is System.Collections.IEnumerable))
        {
          CollectOriginalData(prop.GetValue(parent, null));
        }
        else
        {
          object value = RecordPropertyChange(prop, parent);

          if (value == null || !value.Equals(DONT_MONITOR_PROPERTY_NOTIFICATION))
            _originalValues.Add(new KeyValuePair<object, PropertyInfo>(parent,prop), value);
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="prop"></param>
    /// <returns></returns>
    private object RecordPropertyChange(PropertyInfo prop, object parentObject)
    {
      object propertyValue = null;
      if (prop.CanWrite)
        propertyValue = prop.GetValue(parentObject, null);
      else //a bit of an ugly way to inform the caller that we don't care about this property, but more efficient than throwing an exception
        return DONT_MONITOR_PROPERTY_NOTIFICATION;

      Stack<object> propertyHistory = null;
      bool recordOrder = true;

      if (!_propertyHistories.TryGetValue(new KeyValuePair<object,PropertyInfo>(parentObject,prop), out propertyHistory))
      {
        recordOrder = false;
        propertyHistory = new Stack<object>();
        _propertyHistories.Add(new KeyValuePair<object,PropertyInfo>(parentObject,prop), propertyHistory);
      }

      propertyHistory.Push(propertyValue);

      if (recordOrder)
        _undoOrder.Push(new KeyValuePair<object,PropertyInfo>(parentObject,prop));

      return propertyValue;
    }

    /// <summary>
    /// 
    /// </summary>
    public void PerformCompleteUndo()
    {
      _isUndoing = true;
      foreach (KeyValuePair<KeyValuePair<object,PropertyInfo>, object> propValue in _originalValues)
      {
        if (propValue.Key.Value.GetSetMethod(false) != null)
          propValue.Key.Value.SetValue(propValue.Key.Key, propValue.Value, null);
      }
      _isUndoing = false;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Undo()
    {
      _isUndoing = true;
      //PropertyInfo prop = null;
      KeyValuePair<object, PropertyInfo> propInfo;
      propInfo = _undoOrder.Pop();
      object previousValue = null;
      //there are no more undo levels left so must restore the original value
      if (_propertyHistories[propInfo].Count == 1)
        previousValue = _originalValues[propInfo];
      else
      {
        //if this is the first undo since adding something then the top of the stack will be
        //the current state, move past this.
        //Check for "_controlHistories.Count > 1" is required for when a complete load of undo has been performed
        //twice
        if ((!_isUndoLastAction || !_lastUndidProperty.HasValue || !propInfo.Equals(_lastUndidProperty.Value)) && _propertyHistories[propInfo].Count > 1)
          _propertyHistories[propInfo].Pop();
        if (_propertyHistories[propInfo].Count == 1)
          previousValue = _originalValues[propInfo];
        else
          previousValue = _propertyHistories[propInfo].Pop();
      }
      propInfo.Value.SetValue(propInfo.Key, previousValue, null);

      _isUndoing = false;
      _isUndoLastAction = true;
      _lastUndidProperty = propInfo;

      if (_undoOrder.Count == 0)
      {
        _isUndoAllowed = false;
        if (UndoStatusChanged != null)
        {
          DataChangedEventArgs dce = new DataChangedEventArgs();
          dce.PropertyOwner = propInfo.Key;
          dce.PropertyChanged = propInfo.Value;
          dce.HasOwnerStateChanged = _isUndoAllowed;
          UndoStatusChanged(propInfo, dce);
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnWatchedPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      OnWatchedPropertyChanged(sender, e, null);
    }
    private void OnWatchedPropertyChanged(object sender, PropertyChangedEventArgs e, string parentName)
    {
      DataChangedEventArgs dce = null;
      _hasElementChanged = true;

      //Get Property Changed and its Parent Object
      PropertyInfo changedProperty;
      object changedPropertyParent = _watchedObject;
      if (!string.IsNullOrEmpty(parentName))
      {
        changedPropertyParent = _watchedObject.GetType().GetProperty(parentName).GetValue(_watchedObject, null);
      }
      changedProperty = changedPropertyParent.GetType().GetProperty(e.PropertyName);

      //don't want to re-record the change
      if (!_isUndoing)
      {
        _isUndoLastAction = false;
        RecordPropertyChange(changedProperty, changedPropertyParent);
      }

      if (PropertyChanged != null)
      {
        dce = new DataChangedEventArgs();
        dce.PropertyOwner = changedPropertyParent;
        dce.HasOwnerStateChanged = IsDataModified();
        dce.PropertyChanged = changedProperty;

        PropertyChanged(_watchedObject, dce);
      }

      if (!_isUndoAllowed)
      {
        _isUndoAllowed = true;
        if (UndoStatusChanged != null)
        {
          if (dce == null)
          {
            dce = new DataChangedEventArgs();
            dce.PropertyOwner = changedPropertyParent;
            dce.HasOwnerStateChanged = IsDataModified();
            dce.PropertyChanged = changedProperty;
          }
          UndoStatusChanged(_isUndoAllowed, dce);
        }
      }
      //string validationError = ((IDataErrorInfo)_watchedObject)[e.PropertyName];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool IsDataModified()
    {
      if (!_hasElementChanged)
        return _isDataModified;
      _hasElementChanged = false;
      _isDataModified = false;
      foreach (KeyValuePair<KeyValuePair<object,PropertyInfo>, object> propValue in _originalValues)
      {
        object originalValue = _originalValues[propValue.Key];
        object currentValue = propValue.Key.Value.GetValue(propValue.Key.Key, null);

        if (currentValue != null)
          _isDataModified = !currentValue.Equals(originalValue);
        else if (currentValue == null && originalValue != null)
          _isDataModified = true;

        if (_isDataModified)
          break;
      }
      return _isDataModified;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
      _watchedObject.PropertyChanged-= OnWatchedPropertyChanged;
      foreach (PropertyInfo pInfo in _watchedObject.GetType().GetProperties())
      {
        if (pInfo.GetSetMethod() == null) continue; //Ignore read-only properties

        object _watchedObjectProperty = pInfo.GetValue(_watchedObject, null);
        if (_watchedObjectProperty is INotifyPropertyChanged && !(_watchedObjectProperty is System.Collections.IEnumerable))
        {
          string parentName = pInfo.Name; //Required for lambda-expression closure.
          (_watchedObjectProperty as INotifyPropertyChanged).PropertyChanged -= (sender, e) => OnWatchedPropertyChanged(sender, e, parentName);
        }
      }
    }
  }
}
