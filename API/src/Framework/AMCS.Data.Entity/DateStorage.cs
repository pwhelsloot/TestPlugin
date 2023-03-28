using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime;

namespace AMCS.Data.Entity
{
  /// <summary>
  /// Specifies the way a date is stored in the database.
  /// </summary>
  /// <remarks>
  /// <p>
  /// The <see cref="DateStorage"/> enum is used to tell the ORM layer how to interpret
  /// and store dates in the database. Setting this is required when working with fields
  /// that are time zone compliant (i.e. Noda Time types).
  /// </p>
  /// <p>
  /// At a high level, you should choose the following settings when mapping database columns:
  ///
  /// What's in the database        | Database type    | Storage type | .NET type
  /// ----------------------------- | ---------------- | ------------ | --------------
  /// Database time zone            | DATETIME         | Neutral      | ZonedDateTime
  /// UTC                           | DATETIME         | UTC          | ZonedDateTime
  /// Date time offset              | DATETIMEOFFSET   | Offset       | OffsetDateTime
  /// Local with a linked time zone | DATETIME         | Zoned        | ZonedDateTime
  /// Date without a time           | DATE or DATETIME | Date         | LocalDate
  /// Time without a date           | DATETIME         | Time         | LocalTime
  ///
  /// Note that you are required to specify the storage type. It would be very easy to infer
  /// the Offset storage type from the database type being <c>DATETIMEOFFSET</c>. However,
  /// we want to make all of this explicit, so the storage type is mandatory.
  ///
  /// For the Zoned data storage to work, you need to also set the TimeZoneMember attribute
  /// on the entity member. This references the column name of the property that contains the
  /// time zone for that field. It is required that this property is set appropriately for
  /// the system to be able to load and save dates. When saving entities that have such a
  /// property, before the framework save, ensure that this property is re-read from the database
  /// to ensure that the caller of the save method does not have control over the time zone.
  ///
  /// The time storage type is a bit of a strange one, since we're just cutting off the date
  /// part. To be able to properly format a <c>DATETIME</c> though, we do need a date component.
  /// This is set to the Unix epoch, i.e. 1970-01-01.
  /// </p>
  /// </remarks>
  public enum DateStorage
  {
    /// <summary>
    /// Applies to values that are not a date.
    /// </summary>
    None,

    /// <summary>
    /// Indicates a local time stored in UTC.
    /// </summary>
    /// <remarks>
    /// This storage type is used for <c>DATETIME</c> database types, which are physically
    /// stored in the database in UTC. The .NET data type can be either <see cref="Instant"/>
    /// or <see cref="ZonedDateTime"/>.
    /// </remarks>
    UTC,

    /// <summary>
    /// Indicates a <c>DATETIMEOFFSET</c> database type.
    /// </summary>
    /// <remarks>
    /// This storage type is used for <c>DATETIMEOFFSET</c> database types. The .NET data
    /// type can be either <see cref="Instant"/> or <see cref="OffsetDateTime"/>. Date/time
    /// offsets can be represented in <see cref="ZonedDateTime"/>, but the offset isn't
    /// really a time zone. As such, we don't support it.
    /// </remarks>
    Offset,

    /// <summary>
    /// Indicates a local time stored in the neutral (i.e. database) time zone.
    /// </summary>
    /// <remarks>
    /// This storage type is used for <c>DATETIME</c> database types. The .NET data
    /// type can be either <see cref="Instant"/> or <see cref="ZonedDateTime"/>. We could support
    /// <see cref="OffsetDateTime"/>, but this specifically is not supported because this type
    /// is discouraged. If necessary, one can be constructed from a <see cref="ZonedDateTime"/>.
    /// </remarks>
    Neutral,

    /// <summary>
    /// Indicates a time that has, in some way, an associated time zone.
    /// </summary>
    /// <remarks>
    /// This storage type is used for <c>DATETIME</c> database types that have an inferred
    /// time zone. This requires a target time zone to be provided. To provide a target time
    /// zone, set the <see cref="EntityMemberAttribute.TimeZoneMember"/> property to a dynamic
    /// property containing the time zone ID.
    /// </remarks>
    Zoned,

    /// <summary>
    /// Indicates a <c>DATE</c> database type or <c>DATETIME</c> with the time component
    /// set to midnight.
    /// </summary>
    /// <remarks>
    /// This storage type is used for <c>DATE</c> database types, or <c>DATETIME</c> database types
    /// where the time component is expected to be midnight always.
    /// </remarks>
    Date,

    /// <summary>
    /// Indicates a <c>DATETIME</c> database type with the date component set to 1970-01-01.
    /// </summary>
    /// <remarks>
    /// This storage type is used for <c>DATETIME</c> database types where only the time component
    /// is used. As a convention, we set the date part of this to 1970-01-01.
    /// </remarks>
    Time
  }
}
