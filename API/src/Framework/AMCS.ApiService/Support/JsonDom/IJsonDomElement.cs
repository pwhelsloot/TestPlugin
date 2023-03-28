using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AMCS.ApiService.Support.JsonDom
{
  public interface IJsonDomElement
  {
    void Write(JsonWriter writer);
  }
}
