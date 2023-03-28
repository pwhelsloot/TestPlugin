using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.ApiService.Filtering
{
  internal struct FilterToken
  {
    public FilterTokenKind Kind { get; }

    public string Text { get; }

    public FilterToken(FilterTokenKind kind, string text)
    {
      Kind = kind;
      Text = text;
    }
  }
}
