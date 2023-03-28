namespace AMCS.Data.Entity
{
  public class DataType
  {
    public string DataTypeValue { get; }

    public static DataType String => new DataType("string");
    public static DataType Bool => new DataType("bool");
    public static DataType Integer => new DataType("int");
    public static DataType Decimal => new DataType("decimal");
    public static DataType Duration => new DataType("Duration");
    public static DataType LocalDate => new DataType("LocalDate");
    public static DataType LocalTime => new DataType("LocalTime");
    public static DataType LocalDateTime => new DataType("LocalDateTime");
    public static DataType OffsetDateTime => new DataType("OffsetDateTime");
    public static DataType ZonedDateTime => new DataType("ZonedDateTime");
    public static DataType UtcDateTime => new DataType("UtcDateTime");

    private DataType(string dataTypeValue)
    {
      DataTypeValue = dataTypeValue;
    }
  }
}