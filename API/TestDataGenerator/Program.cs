namespace TestDataGenerator
{
  using System;
  using System.IO;
  using CommandLine;
  using CommandLine.Text;
  using log4net;
  using log4net.Config;

  public class Program
  {
    public static int Main(string[] args)
    {
      try
      {
        XmlConfigurator.Configure(LogManager.GetRepository(typeof(Program).Assembly), LoadLog4NetConfiguration());
        var parsed = Parser.Default.ParseArguments<Arguments>(args);

        if (parsed is NotParsed<Arguments>)
        {
          var helpText = HelpText.AutoBuild(parsed);

          Console.WriteLine(helpText.ToString());

          return 1;
        }

        new Runner(((Parsed<Arguments>)parsed).Value).Run();

        return 0;
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine(ex);
        Console.ReadLine();
        return 2;
      }
    }

    private static Stream LoadLog4NetConfiguration()
    {
      return typeof(Program).Assembly.GetManifestResourceStream(typeof(Program).Namespace + ".log4net.xml");
    }
  }
}
