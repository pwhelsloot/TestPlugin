using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AMCS.ApiService.Support.JsonDom
{
  public static class JsonDomValue
  {
    public static IJsonDomElement From(string value)
    {
      if (value == null)
        return null;

      return new StringElement(value);
    }

    public static IJsonDomElement From(int value)
    {
      return new Int32Element(value);
    }

    public static IJsonDomElement From(bool value)
    {
      return new BooleanElement(value);
    }

    public static IJsonDomElement From(decimal value)
    {
      return new DecimalElement(value);
    }

    private class StringElement : IJsonDomElement
    {
      private readonly string value;

      public StringElement(string value)
      {
        this.value = value;
      }

      public void Write(JsonWriter writer)
      {
        writer.WriteValue(value);
      }
    }

    private class Int32Element : IJsonDomElement
    {
      private readonly int value;

      public Int32Element(int value)
      {
        this.value = value;
      }

      public void Write(JsonWriter writer)
      {
        writer.WriteValue(value);
      }
    }

    private class DecimalElement : IJsonDomElement
    {
      private readonly decimal value;

      public DecimalElement(decimal value)
      {
        this.value = value;
      }

      public void Write(JsonWriter writer)
      {
        writer.WriteValue(value);
      }
    }

    private class BooleanElement : IJsonDomElement
    {
      private readonly bool value;

      public BooleanElement(bool value)
      {
        this.value = value;
      }

      public void Write(JsonWriter writer)
      {
        writer.WriteValue(value);
      }
    }
  }
}
