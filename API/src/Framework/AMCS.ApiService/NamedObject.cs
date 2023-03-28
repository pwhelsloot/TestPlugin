using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService
{
  internal class NamedObject
  {
    private readonly string name;

    public NamedObject(string name)
    {
      this.name = name;
    }

    public override string ToString()
    {
      return name;
    }
  }
}