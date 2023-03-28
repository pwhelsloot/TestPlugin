namespace AMCS.Data.Util.Email
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Linq;
  using System.Text;
  using Microsoft.Office.Interop.Outlook;

  public class EmailHelper
  {
    public static void OpenEmailClient(IList<string> emailAddress, IList<string> ccEmailAddress, IList<string> bccEmailAddress, string subject, string message, IList<string> attachments)
    {
      if (attachments == null || !attachments.Any())
      {
        OpenEmailClient(emailAddress, ccEmailAddress, bccEmailAddress, subject, message);
      }
      else
      {
        try
        {
          OpenOutlook(emailAddress, ccEmailAddress, bccEmailAddress, subject, message, attachments);
        }
        catch (System.Runtime.InteropServices.COMException)
        {
          //We want to try and avoid using this as it could potentially leak memory and the email is opened in a modal dialog.
          OpenMAPI(emailAddress, ccEmailAddress, bccEmailAddress, subject, message, attachments);
        }
      }
    }

    private static void OpenOutlook(IList<string> emailAddress, IList<string> ccEmailAddress, IList<string> bccEmailAddress, string subject, string message, IList<string> attachments)
    {
      Microsoft.Office.Interop.Outlook.Application app = new Microsoft.Office.Interop.Outlook.Application();
      MailItem mailItem = (MailItem)app.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);
      mailItem.Subject = subject;
      StringBuilder toBuilder = new StringBuilder();
      int i = 0;
      foreach (string address in emailAddress ?? new List<string>())
      {
        if (i != 0)
        {
          toBuilder.Append(";");
        }
        toBuilder.Append(address);
        i++;
      }
      mailItem.To = toBuilder.ToString();

      StringBuilder ccBuilder = new StringBuilder();
      i = 0;
      foreach (string address in ccEmailAddress ?? new List<string>())
      {
        if (i != 0)
        {
          ccBuilder.Append(";");
        }
        ccBuilder.Append(address);
        i++;
      }
      mailItem.CC = ccBuilder.ToString();

      StringBuilder bccBuilder = new StringBuilder();
      i = 0;
      foreach (string address in bccEmailAddress ?? new List<string>())
      {
        if (i != 0)
        {
          bccBuilder.Append(";");
        }
        bccBuilder.Append(address);
        i++;
      }
      mailItem.BCC = bccBuilder.ToString();
      mailItem.Body = message;

      foreach (string attachment in attachments ?? new List<string>())
      {
        if (attachment != null)
        {
          mailItem.Attachments.Add(attachment);
        }
      }
      mailItem.Display();
    }

    private static void OpenMAPI(IList<string> emailAddress, IList<string> ccEmailAddress, IList<string> bccEmailAddress, string subject, string message, IList<string> attachments)
    {
      MAPIWrapper mapiWrapper = new MAPIWrapper();

      foreach (string address in emailAddress ?? new List<string>())
      {
        mapiWrapper.AddRecipientTo(address);
      }

      foreach (string address in ccEmailAddress ?? new List<string>())
      {
        mapiWrapper.AddRecipientCC(address);
      }

      foreach (string address in bccEmailAddress ?? new List<string>())
      {
        mapiWrapper.AddRecipientBCC(address);
      }
      foreach (string attachment in attachments ?? new List<string>())
      {
        if (attachment != null)
        {
          mapiWrapper.AddAttachment(attachment);
        }
      }

      mapiWrapper.SendMailPopup(subject, message);

    }


    public static void OpenEmailClient(IList<string> emailAddress, IList<string> ccEmailAddress, IList<string> bccEmailAddress, string subject, string message)
    {
      StringBuilder mailToBuilder = new StringBuilder();
      mailToBuilder.Append("mailto:");
      int i = 0;
      foreach (string address in emailAddress ?? new List<string>())
      {
        if (i != 0)
        {
          mailToBuilder.Append(",");
        }
        mailToBuilder.Append(address);
        i++;
      }

      i = 0;
      foreach (string address in ccEmailAddress ?? new List<string>())
      {
        if (i == 0)
        {
          mailToBuilder.Append("?cc=");
        }
        else
        {
          mailToBuilder.Append(",");
        }
        mailToBuilder.Append(address);
        i++;
      }

      i = 0;
      foreach (string address in bccEmailAddress ?? new List<string>())
      {
        if (i == 0)
        {
          mailToBuilder.Append("?bcc=");
        }
        else
        {
          mailToBuilder.Append(",");
        }
        mailToBuilder.Append(address);
        i++;
      }

      if (!string.IsNullOrWhiteSpace(subject))
      {
        mailToBuilder.Append("subject=");
        mailToBuilder.Append(subject);
      }

      if (!string.IsNullOrWhiteSpace(message))
      {
        mailToBuilder.Append("body=");
        mailToBuilder.Append(message);
      }

      Process.Start(mailToBuilder.ToString());

    }

    public static void OpenEmailClient(IList<string> emailAddress, IList<string> ccEmailAddress, string subject, string message, string attachment = null)
    {
      if (attachment == null)
      {
        OpenEmailClient(emailAddress, ccEmailAddress, null, subject, message, null);
      }
      else
      {
        OpenEmailClient(emailAddress, ccEmailAddress, null, subject, message, new List<string> { attachment });
      }
    }

    public static void OpenEmailClient(IList<string> emailAddress, string subject, string message, string attachment = null)
    {
      if (attachment == null)
      {
        OpenEmailClient(emailAddress, null, null, subject, message, null);
      }
      else
      {
        OpenEmailClient(emailAddress, null, null, subject, message, new List<string> { attachment });
      }
    }

    public static void OpenEmailClient(string emailAddress, string subject, string message, string attachment = null)
    {
      if (attachment == null)
      {
        OpenEmailClient(new List<string> { emailAddress }, null, null, subject, message, null);
      }
      else
      {
        OpenEmailClient(new List<string> { emailAddress }, null, null, subject, message, new List<string> { attachment });
      }
    }

    public static void OpenEmailClient(string emailAddress)
    {
      OpenEmailClient(new List<string> { emailAddress }, null, null, null, null, null);
    }

    public static void OpenEmailClientWithAttachment(string attachment)
    {
      OpenEmailClient(null, null, null, null, null, new List<string>{attachment});
    }

    public static void OpenEmailClientWithAttachment(IList<string> emailAddress, string attachment)
    {
      OpenEmailClient(emailAddress, null, null, null, null, new List<string> { attachment });
    }

    public static void OpenEmailClientWithAttachments(IList<string> emailAddress, IList<string> attachments)
    {
      OpenEmailClient(emailAddress, null, null, null, null, attachments);
    }

    public static void OpenEmailClientWithAttachments(IList<string> attachments)
    {
      OpenEmailClient(null, null, null, null, null, attachments);
    }
  }
}
