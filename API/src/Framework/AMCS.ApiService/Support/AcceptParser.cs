using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Support
{
  public class AcceptParser
  {
    public static AcceptParser Parse(string header)
    {
      var accepts = new List<Accept>();

      if (!string.IsNullOrEmpty(header))
      {
        foreach (string part in header.Split(','))
        {
          string contentType;
          double weight = 1;

          int pos = part.IndexOf(';');
          if (pos == -1)
          {
            contentType = part.Trim();
          }
          else
          {
            contentType = part.Substring(0, pos).Trim();
            weight = double.Parse(part.Substring(pos + 1).Trim(), CultureInfo.InvariantCulture);
          }

          accepts.Add(new Accept(contentType, weight));
        }
      }

      return new AcceptParser(accepts);
    }

    private readonly List<Accept> accepts;

    private AcceptParser(List<Accept> accepts)
    {
      this.accepts = accepts;
    }

    public bool TryGetWeight(string contentType, out double weight)
    {
      weight = 0;

      foreach (var accept in accepts)
      {
        if (string.Equals(accept.ContentType, contentType, StringComparison.OrdinalIgnoreCase))
        {
          weight = accept.Weight;
          return true;
        }
      }

      return false;
    }

    private class Accept
    {
      public string ContentType { get; }

      public double Weight { get; }

      public Accept(string contentType, double weight)
      {
        ContentType = contentType;
        Weight = weight;
      }
    }
  }
}
