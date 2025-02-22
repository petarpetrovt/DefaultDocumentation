﻿using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DefaultDocumentation.Helper
{
    internal static class StringExtension
    {
        private static readonly Dictionary<string, string> _invalidStrings =
            new Dictionary<string, string>(Path.GetInvalidFileNameChars().ToDictionary(c => $"{c}", _ => "-"))
            {
                ["="] = string.Empty,
                [" "] = string.Empty,
                [","] = "_",
                ["."] = "-",
                ["["] = "-",
                ["]"] = "-",
                ["&lt;"] = "-",
                ["&gt;"] = "-",
            };

        public static void ChangeInvalidReplacement(string value)
        {
            foreach (string key in _invalidStrings.Keys.ToList())
            {
                if (!string.IsNullOrEmpty(_invalidStrings[key]))
                {
                    _invalidStrings[key] = value;
                }
            }
        }

        public static string Clean(this string value)
        {
            foreach (KeyValuePair<string, string> pair in _invalidStrings)
            {
                value = value.Replace(pair.Key, pair.Value);
            }

            return value;
        }

        public static string Prettify(this string value)
        {
            int genericIndex = value.IndexOf('`');
            if (genericIndex > 0)
            {
                int memberIndex = value.IndexOf('.', genericIndex);
                int argsIndex = value.IndexOf('(', genericIndex);
                if (memberIndex > 0)
                {
                    value = $"{value.Substring(0, genericIndex)}&lt;&gt;{Prettify(value.Substring(memberIndex))}";
                }
                else if (argsIndex > 0)
                {
                    value = $"{value.Substring(0, genericIndex)}&lt;&gt;{Prettify(value.Substring(argsIndex))}";
                }
                else if (value.IndexOf('(') < 0)
                {
                    value = $"{value.Substring(0, genericIndex)}&lt;&gt;";
                }
            }

            return value.Replace('`', '@');
        }

        public static string AsLink(this string value, string displayedName) => $"[{displayedName.Prettify()}]({value} '{value}')";

        public static string AsDotNetApiLink(this string value, string displayedName = null)
        {
            displayedName = (displayedName ?? value).Prettify();

            string link = value;
            int parametersIndex = link.IndexOf("(");
            if (parametersIndex > 0)
            {
                string methodName = link.Substring(0, parametersIndex);

                link = $"{methodName}#{link.Replace('.', '_').Replace('`', '_').Replace('(', '_').Replace(')', '_')}";
            }

            return $"[{displayedName}](https://docs.microsoft.com/en-us/dotnet/api/{link.Replace('`', '-')} '{value}')";
        }
    }
}
