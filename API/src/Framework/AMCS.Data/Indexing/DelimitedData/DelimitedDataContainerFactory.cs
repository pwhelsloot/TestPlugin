using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Indexing.DelimitedData
{
  public class DelimitedDataContainerFactory<ContainerType, RecordType>
    where ContainerType : IDelimitedDataContainer, new()
    where RecordType : ISerializableArray, new()
  {
    public string FieldDelimeter = ",";
    public string FieldDelimeterEscapeCharacter = "\\,";
    public string RecordDelimeter = "\n";
    public string RecordDelimeterEscapeCharacter = "<\n>";

    public DelimitedDataContainerFactory()
    {
    }

    public DelimitedDataContainerFactory(string fieldDelimeter, string fieldDelimeterEscapeCharacter, string recordDelimeter, string recordDelimeterEscapeCharacter)
    {
      FieldDelimeter = fieldDelimeter;
      FieldDelimeterEscapeCharacter = fieldDelimeterEscapeCharacter;
      RecordDelimeter = recordDelimeter;
      RecordDelimeterEscapeCharacter = recordDelimeterEscapeCharacter;
    }

    public virtual ContainerType CreateFrom(RecordType[] recordsToContain)
    {
      StringBuilder csvDataBuilder = new StringBuilder();
      foreach (RecordType record in recordsToContain)
      {
        string escapedRecordAsString =
          DelimitedDataSerializer<RecordType>.Serialize(record, FieldDelimeter, FieldDelimeterEscapeCharacter) ?? ""
            .Replace(RecordDelimeter, RecordDelimeterEscapeCharacter);
        csvDataBuilder.Append(escapedRecordAsString);
        csvDataBuilder.Append(RecordDelimeter);
      }

      return new ContainerType()
      {
        FieldDelimiter = FieldDelimeter,
        FieldDelimiterEscapeCharacter = FieldDelimeterEscapeCharacter,
        RecordDelimiter = RecordDelimeter,
        RecordDelimiterEscapeCharacter = RecordDelimeterEscapeCharacter,
        Data = csvDataBuilder.ToString()
      };
    }

    public IList<RecordType> GetRecords(ContainerType dataContainer)
    {
      List<RecordType> output = new List<RecordType>();
      if (dataContainer == null || string.IsNullOrEmpty(dataContainer.Data)) { return output; }
      string[] stringRecords = Utils.SplitString(dataContainer.Data
            .Replace(dataContainer.RecordDelimiterEscapeCharacter, dataContainer.RecordDelimiter)
            , dataContainer.RecordDelimiter, true);
      foreach (string stringRecord in stringRecords)
      {
        string unescapedStringRecord = stringRecord.Replace(dataContainer.RecordDelimiterEscapeCharacter, dataContainer.RecordDelimiter);
        output.Add(DelimitedDataSerializer<RecordType>.Parse(unescapedStringRecord, dataContainer.FieldDelimiter, dataContainer.FieldDelimiterEscapeCharacter));
      }
      return output;
    }


    /// <summary>
    /// Gets all records using a Stream as Data Source
    /// </summary>
    /// <param name="dataContainer">DelimitedDataContainer that represents data in dataStream</param>
    /// <param name="dataStream">DelimitedDataContainer Data As a Stream</param>
    /// <returns>Collection of Records</returns>
    public IList<RecordType> GetRecordsFromStream(ContainerType dataContainer, Stream dataStream)
    {
      List<RecordType> output = new List<RecordType>();

      if (dataStream.Length > Int32.MaxValue)
      {
        throw new ArgumentOutOfRangeException("Data Stream exceeds maximum supported length");
      }

      //Get reader from stream (but do NOT dispose! leave that to the calling method)
      StreamReader reader = new StreamReader(dataStream);

      //Build buffer to size expected
      char[] buffer = new char[dataStream.Length];
      reader.Read(buffer, 0, buffer.Length); //Count=buffer.Length could become a problem here if number is greater than Int32.MaxValue
      //Convert to string
      string searchString = new string(buffer);
      //Extract records
      string[] stringRecords = Utils.SplitString(searchString
            .Replace(dataContainer.RecordDelimiterEscapeCharacter, dataContainer.RecordDelimiter)
            , dataContainer.RecordDelimiter, true);
      foreach (string stringRecord in stringRecords)
      {
        string unescapedStringRecord = stringRecord.Replace(dataContainer.RecordDelimiterEscapeCharacter, dataContainer.RecordDelimiter);
        output.Add(DelimitedDataSerializer<RecordType>.Parse(unescapedStringRecord, dataContainer.FieldDelimiter, dataContainer.FieldDelimiterEscapeCharacter));
      }
      return output;
    }


    public enum RecordsTemplateSearchType { ObjectsEqual, CaseSensitiveStringValues, CaseInsensitiveStringValues, StartsWithCaseInsensisitveStringValues }


    /// <summary>
    /// Get Records using template entities, limit search to records stored against an Index Key
    /// </summary>
    /// <param name="templateOfMustHaveAllValues">Template that specifies properties that all returned records must have</param>
    /// <param name="templateOfMustHaveAtLeastOneOfTheseValues">Template that specifies properties that all individual records have at least one matching</param>
    /// <returns>Records that match search criteria specified in templates and search type</returns>
    public IList<RecordType> GetRecordsByTemplate(IEnumerable<RecordType> records, RecordType templateOfMustHaveAllValues, RecordType templateOfMustHaveAtLeastOneOfTheseValues)
    {
      return GetRecordsByTemplate(records, templateOfMustHaveAllValues, templateOfMustHaveAtLeastOneOfTheseValues, RecordsTemplateSearchType.ObjectsEqual);
    }


    /// <summary>
    /// Get Records using template entities, limit search to records stored against an Index Key
    /// </summary>
    /// <param name="templateOfMustHaveAllValues">Template that specifies properties that all returned records must have</param>
    /// <param name="templateOfMustHaveAtLeastOneOfTheseValues">Template that specifies properties that all individual records have at least one matching</param>
    /// <param name="searchType">Type of match of property values required for record to be returned</param>
    /// <returns>Records that match search criteria specified in templates and search type</returns>
    public IList<RecordType> GetRecordsByTemplate(IEnumerable<RecordType> records, RecordType templateOfMustHaveAllValues, RecordType templateOfMustHaveAtLeastOneOfTheseValues, RecordsTemplateSearchType searchType)
    {
      IEnumerable<RecordType> results = records;
      if (templateOfMustHaveAllValues != null)
      {
        PropertyInfo[] ANDtemplateProperties = templateOfMustHaveAllValues.GetType().GetProperties();

        foreach (PropertyInfo property in ANDtemplateProperties)
        {
          object templatePropertyValue = property.GetValue(templateOfMustHaveAllValues, null);
          object defaultPropValue = null;
          if (property.PropertyType.IsValueType)
            defaultPropValue = Activator.CreateInstance(property.PropertyType);


          //Only use property if is not null, not equal to default value
          if (templatePropertyValue != null && !templatePropertyValue.Equals(defaultPropValue))
          {
            //If we are not doing ObjectEquals, disregard any IEnumerables
            if (searchType == DelimitedDataContainerFactory<ContainerType, RecordType>.RecordsTemplateSearchType.ObjectsEqual
              && templatePropertyValue is IEnumerable)
            {
              continue;
            }
            switch (searchType)
            {
              case DelimitedDataContainerFactory<ContainerType, RecordType>.RecordsTemplateSearchType.ObjectsEqual:
                results = results.Where(o => property.GetValue(o, null) == templatePropertyValue);
                break;
              case DelimitedDataContainerFactory<ContainerType, RecordType>.RecordsTemplateSearchType.CaseSensitiveStringValues:
                results = results.Where(o => property.GetValue(o, null).ToString() == templatePropertyValue.ToString());
                break;
              case DelimitedDataContainerFactory<ContainerType, RecordType>.RecordsTemplateSearchType.CaseInsensitiveStringValues:
                results = results.Where(o => property.GetValue(o, null).ToString().ToLower() == templatePropertyValue.ToString().ToLower());
                break;
              case DelimitedDataContainerFactory<ContainerType, RecordType>.RecordsTemplateSearchType.StartsWithCaseInsensisitveStringValues:
                results = results.Where(o => property.GetValue(o, null).ToString().ToLower().StartsWith(templatePropertyValue.ToString().ToLower()));
                break;
            }
          }
        }
      }


      if (templateOfMustHaveAtLeastOneOfTheseValues != null)
      {
        PropertyInfo[] ORtemplateProperties = templateOfMustHaveAtLeastOneOfTheseValues.GetType().GetProperties();

        //Run all OR Template's filters against results
        //Dictionary will store results that have passed at least one of the filters
        List<RecordType> foundInAtLeastOneFilterCase = new List<RecordType>();
        foreach (PropertyInfo property in ORtemplateProperties)
        {
          object templatePropertyValue = property.GetValue(templateOfMustHaveAtLeastOneOfTheseValues, null);
          object defaultPropValue = null;
          if (property.PropertyType.IsValueType)
            defaultPropValue = Activator.CreateInstance(property.PropertyType);


          //Only use property if is not null, not equal to default value, not an empty IEnumerable & not a DynamicColumn
          if (templatePropertyValue != null && !templatePropertyValue.Equals(defaultPropValue) && (!(templatePropertyValue is System.Collections.ICollection) || (templatePropertyValue as System.Collections.ICollection).Count > 0))
          {
            //filters.Add(o => property.GetValue(o, null) == templatePropertyValue);
            //If we are not doing ObjectEquals, disregard any IEnumerables
            if (searchType == DelimitedDataContainerFactory<ContainerType, RecordType>.RecordsTemplateSearchType.ObjectsEqual
              && templatePropertyValue is IEnumerable)
            {
              continue;
            }
            else
            {
              IEnumerable<RecordType> resultsThatPassFilter;
              switch (searchType)
              {
                default:
                case DelimitedDataContainerFactory<ContainerType, RecordType>.RecordsTemplateSearchType.ObjectsEqual:
                  resultsThatPassFilter = results.Where(o => property.GetValue(o, null) == templatePropertyValue);
                  break;
                case DelimitedDataContainerFactory<ContainerType, RecordType>.RecordsTemplateSearchType.CaseSensitiveStringValues:
                  resultsThatPassFilter = results.Where(o => property.GetValue(o, null).ToString() == templatePropertyValue.ToString());
                  break;
                case DelimitedDataContainerFactory<ContainerType, RecordType>.RecordsTemplateSearchType.CaseInsensitiveStringValues:
                  resultsThatPassFilter = results.Where(o => property.GetValue(o, null).ToString().ToLower() == templatePropertyValue.ToString().ToLower());
                  break;
                case DelimitedDataContainerFactory<ContainerType, RecordType>.RecordsTemplateSearchType.StartsWithCaseInsensisitveStringValues:
                  resultsThatPassFilter = results.Where(o => property.GetValue(o, null).ToString().ToLower().StartsWith(templatePropertyValue.ToString().ToLower()));
                  break;
              }
              //Add results that pass this filter to our foundInAtLeastOneFilterCase list, to ensure only one of each applicable results is returned
              foreach (RecordType result in resultsThatPassFilter)
              {
                if (!foundInAtLeastOneFilterCase.Contains(result))
                {
                  foundInAtLeastOneFilterCase.Add(result);
                }
              }
            }
          }
        }
        results = foundInAtLeastOneFilterCase;
      }

      return results.ToList();
    }
  }
}
