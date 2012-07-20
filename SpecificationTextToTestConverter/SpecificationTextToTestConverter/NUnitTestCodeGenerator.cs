using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Windows.Media;

namespace SpecificationTextToTestConverter
{
    internal static class NUnitTestCodeGenerator
    {
        public static FlowDocument GenerateCode(string specification)
        {
            FlowDocument code = new FlowDocument();

            string[] lines = specification.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            if (!lines.Any())
            {
                return code;
            }

            string firstLine = lines.First();
            string className = firstLine.Reformat();

            var nUnitCode = new StringBuilder();

            Brush tealBrush = new SolidColorBrush(Color.FromRgb(43, 145, 175));
            Brush blueBrush = new SolidColorBrush(Colors.Blue);
            Brush redBrush = new SolidColorBrush(Color.FromRgb(163, 21, 21));

            Run regionRun = new Run("#region") { Foreground = blueBrush };
            Run regionName = new Run(" " + firstLine);
            Paragraph p = new Paragraph();
            p.Inlines.Add(regionRun);
            p.Inlines.Add(regionName);
            code.Blocks.Add(p);
            code.Blocks.Add(new Paragraph());

            nUnitCode.AppendLine("[TestFixture]");

            Run testFixture = new Run("TestFixture") { Foreground = tealBrush };
            p = new Paragraph();
            p.Inlines.Add(new Run("["));
            p.Inlines.Add(testFixture);
            p.Inlines.Add(new Run("]"));
            code.Blocks.Add(p);

            Run categoryRun = new Run("Category") { Foreground = tealBrush };
            Run quotesRun = new Run(@"""""") { Foreground = redBrush };
            p = new Paragraph();
            p.Inlines.Add(new Run("["));
            p.Inlines.Add(categoryRun);
            p.Inlines.Add(new Run("("));
            p.Inlines.Add(quotesRun);
            p.Inlines.Add(new Run(")"));
            p.Inlines.Add(new Run("]"));
            code.Blocks.Add(p);
            
            Run publicClassRun = new Run("public class ") { Foreground = blueBrush };
            p = new Paragraph();
            p.Inlines.Add(publicClassRun);
            p.Inlines.Add(new Run(className));
            code.Blocks.Add(p);

            p = new Paragraph(new Run("{"));
            code.Blocks.Add(p);

            bool isOnlyFixtureWithoutTests = true;
            foreach (var line in lines.Skip(1))
            {
                string reformattedLine = line.Reformat();
                if (string.IsNullOrWhiteSpace(reformattedLine))
                {
                    continue;
                }

                if (!isOnlyFixtureWithoutTests)
                {
                    p = new Paragraph();
                    code.Blocks.Add(p);
                }
                isOnlyFixtureWithoutTests = false;

                reformattedLine = reformattedLine[0].ToString().ToUpper()
                    + reformattedLine.Substring(1, reformattedLine.Length - 1);

                Run testRun = new Run("Test") { Foreground = tealBrush };
                p = new Paragraph();
                p.Inlines.Add(new Run("    ["));
                p.Inlines.Add(testRun);
                p.Inlines.Add(new Run("]"));
                code.Blocks.Add(p);

                Run publicRun = new Run("    public void ") { Foreground = blueBrush };
                p = new Paragraph();
                p.Inlines.Add(publicRun);
                p.Inlines.Add(new Run(reformattedLine));
                p.Inlines.Add(new Run("()"));
                code.Blocks.Add(p);

                p = new Paragraph(new Run("    {"));
                code.Blocks.Add(p);

                Run throwNewRun = new Run("        throw new ") { Foreground = blueBrush };
                Run exceptionRun = new Run("NotImplementedException") { Foreground = tealBrush };
                p = new Paragraph();
                p.Inlines.Add(throwNewRun);
                p.Inlines.Add(exceptionRun);
                p.Inlines.Add(new Run("();"));
                code.Blocks.Add(p);

                p = new Paragraph(new Run("    }"));
                code.Blocks.Add(p);

                nUnitCode.AppendLine("    {");
                nUnitCode.AppendLine("        throw new NotImplementedException();");
                nUnitCode.AppendLine("    }");
            }

            if (isOnlyFixtureWithoutTests)
            {
                p = new Paragraph(new Run("    "));
                code.Blocks.Add(p);
            }

            p = new Paragraph(new Run("}"));
            code.Blocks.Add(p);

            p = new Paragraph();
            code.Blocks.Add(p);

            Run endregionRun = new Run("#endregion") { Foreground = blueBrush };
            p = new Paragraph();
            p.Inlines.Add(endregionRun);
            code.Blocks.Add(p);

            return code;
        }

        private static string Reformat(this string source)
        {
            return source
                .Trim()
                .RemoveHyphens()
                .ReplaceIllegalCharactersBySpaces()
                .ReplaceMultipleSpacesBySingle()
                .Replace(" ", "_")
                .ReplaceMultipleUnderscoresBySingle()
                .Trim('_');
        }

        private static string RemoveHyphens(this string source)
        {
            return source.Replace("-", string.Empty);
        }

        private static string ReplaceMultipleSpacesBySingle(this string source)
        {
            return Regex.Replace(source, "[ ]+", " ", RegexOptions.IgnoreCase);
        }

        private static string ReplaceIllegalCharactersBySpaces(this string source)
        {
            return Regex.Replace(source, "[^a-z0-9äöüß_]", " ", RegexOptions.IgnoreCase);
        }

        private static string ReplaceMultipleUnderscoresBySingle(this string source)
        {
            return Regex.Replace(source, "_+", "_");
        }
    }
}
