using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace System
{
    public static class PinyinExtension
    {
        private const string RESOURCE_NAME = "Netcorext.Extensions.Pinyin.pinyin.dict";
        private static readonly Dictionary<string, string[]> ChtPinyin = new Dictionary<string, string[]>(StringComparer.CurrentCultureIgnoreCase);
        private static readonly Dictionary<string, string[]> ChtPinyinN = new Dictionary<string, string[]>(StringComparer.CurrentCultureIgnoreCase);
        private static readonly Dictionary<string, string[]> ChsPinyin = new Dictionary<string, string[]>(StringComparer.CurrentCultureIgnoreCase);
        private static readonly Dictionary<string, string[]> ChsPinyinN = new Dictionary<string, string[]>(StringComparer.CurrentCultureIgnoreCase);

        private static readonly Regex Regex = new Regex("\\d+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        static PinyinExtension()
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(RESOURCE_NAME) ?? throw new InvalidOperationException($"Resource '{RESOURCE_NAME}' not found."))
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var parts = line.Split('\t');

                    if (parts.Length < 3) continue;

                    var seed = Regex.Replace(parts[2], "").Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    var seedN = parts[2].Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    if (ChtPinyin.ContainsKey(parts[0]))
                    {
                        ChtPinyin[parts[0]] = ChtPinyin[parts[0]].Concat(seed).Distinct().ToArray();
                        ChtPinyinN[parts[0]] = ChtPinyinN[parts[0]].Concat(seedN).Distinct().ToArray();
                    }
                    else
                    {
                        ChtPinyin.Add(parts[0], seed);
                        ChtPinyinN.Add(parts[0], seedN);
                    }

                    if (ChsPinyin.ContainsKey(parts[1]))
                    {
                        ChsPinyin[parts[1]] = ChsPinyin[parts[1]].Concat(seed).Distinct().ToArray();
                        ChsPinyinN[parts[1]] = ChsPinyinN[parts[1]].Concat(seedN).Distinct().ToArray();
                    }
                    else
                    {
                        ChsPinyin.Add(parts[1], seed);
                        ChsPinyinN.Add(parts[1], seedN);
                    }
                }
            }
        }


        public static string[] GetPinyin(this string value, bool includeNoMatch = false)
        {
            var pinyin = new List<string>();
            foreach (var c in value)
            {
                if (ChsPinyin.TryGetValue(c.ToString(), out var cht))
                {
                    pinyin.Add(cht[0]);
                }
                else if (ChtPinyin.TryGetValue(c.ToString(), out var chs))
                {
                    pinyin.Add(chs[0]);
                }
                else if (includeNoMatch)
                {
                    pinyin.Add(string.Empty);
                }
            }

            return pinyin.ToArray();
        }
        public static string[][] GetFullPinyin(this string value, bool includeNoMatch = false)
        {
            var pinyin = new List<string[]>();

            foreach (var c in value)
            {
                if (ChsPinyin.TryGetValue(c.ToString(), out var cht))
                {
                    pinyin.Add(cht);
                }
                else if (ChtPinyin.TryGetValue(c.ToString(), out var chs))
                {
                    pinyin.Add(chs);
                }
                else if (includeNoMatch)
                {
                    pinyin.Add(Array.Empty<string>());
                }
            }

            return pinyin.ToArray();
        }

        public static string[] GetPinyinN(this string value, bool includeNoMatch = false)
        {
            var pinyin = new List<string>();

            foreach (var c in value)
            {
                if (ChsPinyinN.TryGetValue(c.ToString(), out var cht))
                {
                    pinyin.Add(cht[0]);
                }
                else if (ChtPinyinN.TryGetValue(c.ToString(), out var chs))
                {
                    pinyin.Add(chs[0]);
                }
                else if (includeNoMatch)
                {
                    pinyin.Add(string.Empty);
                }
            }

            return pinyin.ToArray();
        }

        public static string[][] GetFullPinyinN(this string value, bool includeNoMatch = false)
        {
            var pinyin = new List<string[]>();

            foreach (var c in value)
            {
                if (ChsPinyinN.TryGetValue(c.ToString(), out var cht))
                {
                    pinyin.Add(cht);
                }
                else if (ChtPinyinN.TryGetValue(c.ToString(), out var chs))
                {
                    pinyin.Add(chs);
                }
                else if (includeNoMatch)
                {
                    pinyin.Add(Array.Empty<string>());
                }
            }

            return pinyin.ToArray();
        }
    }
}
