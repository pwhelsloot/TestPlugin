namespace AMCS.Data.Util.Email
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  public class EmailException : Exception
  {
    public EmailException():base(){}

    public EmailException(string message) : base(message) { }
  }
}
