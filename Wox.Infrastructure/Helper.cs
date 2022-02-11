using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Wox.Infrastructure
{
    public static class Helper
    {
        /// <summary>
        /// http://www.yinwang.org/blog-cn/2015/11/21/programming-philosophy
        /// </summary>
        public static T NonNull<T>(this T obj)
        {
            if (obj == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return obj;
            }
        }

        public static void RequireNonNull<T>(this T obj)
        {
            if (obj == null)
            {
                throw new NullReferenceException();
            }
        }

        public static void ValidateDataDirectory(string bundledDataDirectory, string dataDirectory)
        {
            if (!Directory.Exists(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
            }

            foreach (var bundledDataPath in Directory.GetFiles(bundledDataDirectory))
            {
                var data = Path.GetFileName(bundledDataPath);
                var dataPath = Path.Combine(dataDirectory, data.NonNull());
                if (!File.Exists(dataPath))
                {
                    File.Copy(bundledDataPath, dataPath);
                }
                else
                {
                    var time1 = new FileInfo(bundledDataPath).LastWriteTimeUtc;
                    var time2 = new FileInfo(dataPath).LastWriteTimeUtc;
                    if (time1 != time2)
                    {
                        File.Copy(bundledDataPath, dataPath, true);
                    }
                }
            }
        }

        public static void ValidateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static string Formatted<T>(this T t)
        {
            var formatted = JsonConvert.SerializeObject(
                t,
                Formatting.Indented,
                new StringEnumConverter()
            );
            return formatted;
        }


        private const string
            Eng = "qwertyuiop[]asdfghjkl;'zxcvbnm,.",
            Rus = "йцукенгшщзхъфывапролджэячсмитьбю";

        public static bool IsRus(this string input) => Regex.IsMatch(input, "[А-Яа-я]");

        public static bool IsEng(this string input) => Regex.IsMatch(input, "[A-Za-z]");

        public static string EngToRus(this string input)
        {
            var result = new StringBuilder(input.Length);
            int index;
            foreach (var symbol in input.ToLower())
                result.Append((index = Eng.IndexOf(symbol)) != -1 ? Rus[index] : symbol);
            return result.ToString();
        }
        
        public static string RusToEng(this string input)
        {
            var result = new StringBuilder(input.Length);
            int index;
            foreach (var symbol in input.ToLower())
                result.Append((index = Rus.IndexOf(symbol)) != -1 ? Eng[index] : symbol);
            return result.ToString();
        }


    }
}
