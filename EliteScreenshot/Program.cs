using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using CommandLine;
using CommandLine.Text;
using EliteTrader.EliteOcr;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EliteTrader.EliteScreenshot
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter helpWriter = new StringWriter(sb);
            Parser parser = new Parser(a => { a.CaseSensitive = false;
                                                a.HelpWriter = helpWriter;
            });

            

            ParserResult<Options> parserResult = parser.ParseArguments<Options>(args);

            string helpText = sb.ToString();
            if (!string.IsNullOrEmpty(helpText))
            {
                Console.WriteLine(GetHelpText(parserResult));
                return 1;
            }

            if (parserResult.Errors != null && parserResult.Errors.Any())
            {
                foreach (Error error in parserResult.Errors)
                {
                    Console.WriteLine(error);
                }
                Console.WriteLine(GetHelpText(parserResult));
                
                return 1;
            }

            Options options = parserResult.Value;
            if (options == null)
            {
                Console.WriteLine(GetHelpText(parserResult));
                return 1;
            }

            List<string> screenshots = new List<string>();
            if (options.Screenshots != null)
            {
                screenshots = options.Screenshots.ToList();
            }

            if (screenshots.Count == 0)
            {
                Console.WriteLine("Please provide at least one screenshot");
                Console.WriteLine(GetHelpText(parserResult));
                return 1;
            }

            if (options.Format.ToLower() != "json")
            {
                Console.WriteLine("The only supported format is \"json\"");
                Console.WriteLine(GetHelpText(parserResult));
                return 1;
            }

            List<Bitmap> bitmaps = new List<Bitmap>();
            foreach (string screenshot in screenshots)
            {
                bitmaps.Add(new Bitmap(screenshot));
            }

            ScreenshotParser screenshotParser = new ScreenshotParser(Environment.CurrentDirectory);

            ParsedScreenshot result = screenshotParser.ParseMultiple(bitmaps);

            string str = Serialize(result);
            if (!string.IsNullOrEmpty(options.OutputFile))
            {
                using (FileStream fs = new FileStream(options.OutputFile, FileMode.Create))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(str);

                    return 0;
                }
            }

            Console.WriteLine(str);

            return 0;
        }

        private static string Serialize(ParsedScreenshot parsedScreenshot)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
            string str = JsonConvert.SerializeObject(parsedScreenshot, Formatting.Indented, settings);
            return str;
        }

        private static string GetHelpText(ParserResult<Options> result)
        {
            HelpText helpText = HelpText.AutoBuild(result);
            helpText.Heading = new HeadingInfo("EliteScreenshot <screenshot.bmp> [<more from same station>]");
            helpText.Copyright = "";

            return helpText.ToString();
        }
    }

    

    public enum EnumOutputFormat
    {
        Json = 1
    }

    public class Options
    {
        //[Value(1, Required = true)]
        //public int IntValue { get; set; }

        [Value(0, Required = true)]
        public IEnumerable<string> Screenshots { get; set; }

        [Option('o', Required = false, HelpText = "Output to file")]
        public string OutputFile { get; set; }

        [Option('f', Required = false, HelpText = "Output format", DefaultValue = "json")]
        public string Format { get; set; }
        

        //[HelpOption]
        //public string GetUsage()
        //{
        //    return HelpText.AutoBuild(this,
        //      (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        //}
    }
}










































