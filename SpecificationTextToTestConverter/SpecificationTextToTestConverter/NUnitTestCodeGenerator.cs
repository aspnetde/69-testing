using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpecificationTextToTestConverter
{
    internal static class NUnitTestCodeGenerator
    {
        public static string GenerateCode(string specification)
        {
            string[] lines = specification.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            if (!lines.Any())
            {
                return string.Empty;
            }

            string firstLine = lines.First();
            string className = firstLine.Reformat();

            var nUnitCode = new StringBuilder();

            nUnitCode.AppendFormat("#region {0}", firstLine);
            nUnitCode.AppendLine();
            nUnitCode.AppendLine();

            nUnitCode.AppendLine("[TestFixture]");
            nUnitCode.AppendLine(@"[Category("""")]");
            nUnitCode.AppendFormat("public class {0}", className);
            nUnitCode.AppendLine();
            nUnitCode.AppendLine("{");

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
                    nUnitCode.AppendLine();
                }
                isOnlyFixtureWithoutTests = false;

                reformattedLine = reformattedLine[0].ToString().ToUpper()
                    + reformattedLine.Substring(1, reformattedLine.Length - 1);

                nUnitCode.AppendLine("    [Test]");
                nUnitCode.AppendFormat("    public void {0}()", reformattedLine);
                nUnitCode.AppendLine();
                nUnitCode.AppendLine("    {");
                nUnitCode.AppendLine("        throw new NotImplementedException();");
                nUnitCode.AppendLine("    }");
            }

            if (isOnlyFixtureWithoutTests)
            {
                nUnitCode.AppendLine("    ");
            }

            nUnitCode.AppendLine("}");
            
            nUnitCode.AppendLine();
            nUnitCode.AppendLine("#endregion");

            return nUnitCode.ToString().Trim();
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
