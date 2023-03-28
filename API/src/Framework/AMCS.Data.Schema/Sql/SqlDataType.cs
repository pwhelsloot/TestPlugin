using System;
using System.Runtime.Serialization;

namespace AMCS.Data.Schema.Sql
{
  [Serializable]
  public enum SqlDataType
  {
    Unknown,

    @bit,

    @tinyint,
    @smallint,
    @int,
    @bigint,
    @numeric,
    @decimal,
    @float,
    @real,
    @smallmoney,
    @money,
    
    @date,
    @datetime,
    @datetime2,
    @datetimeoffset,
    @smalldatetime,
    @time,

    @char,
    @varchar,
    @text,
    @nchar,
    @nvarchar,
    @ntext,
    @xml,

    @binary,
    @varbinary,
    @image,

    @geography,
    @geometry,

    @timestamp,
    @uniqueidentifier,
  }
}
