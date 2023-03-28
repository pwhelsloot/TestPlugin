using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;
using NodaTime;

namespace AMCS.ApiService.Tests.Filters
{
    public class FilterEntity : EntityObject
    {
        public override string GetTableName()
        {
            return "Filter";
        }

        public override string GetKeyName()
        {
            return "FilterId";
        }

        [DataMember(Name = "StringProp")]
        public string StringProp { get; set; }

        [DataMember(Name = "BoolProp")]
        public bool BoolProp { get; set; }

        [DataMember(Name = "DoubleProp")]
        public double DoubleProp { get; set; }

        [DataMember(Name = "DecimalProp")]
        public decimal DecimalProp { get; set; }

        [DataMember(Name = "IntProp")]
        public int IntProp { get; set; }

        [DataMember(Name = "DateProp")]
        public DateTime DateProp { get; set; }

        [EntityMember(DateStorage = DateStorage.Date)]
        public LocalDate LocalDateProp { get; set; }

        [DataMember(Name = "ZonedDateTimeProp")]
        public ZonedDateTime ZonedDateTimeProp { get; set; }
    }
}
