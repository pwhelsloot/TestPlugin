namespace AMCS.Data.Util.Email
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  public class ClientNotFoundEmailException : EmailException
  {
    public ClientNotFoundEmailException():base()
    {}

    public ClientNotFoundEmailException(string message):base(message)
    {}
  }
}
