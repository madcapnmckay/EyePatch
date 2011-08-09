using System.Collections.Generic;
using System.Linq;

namespace EyePatch.Core.Util.Extensions
{
    public static class StringExtensions
    {
        public static string Or(this string primary, string secondary)
        {
            return string.IsNullOrWhiteSpace(primary) ? secondary : primary;
        }

        public static bool NoneAreNullOrWhiteSpace(params string[] strings)
        {
            return new List<string>(strings).TrueForAll(s => !string.IsNullOrWhiteSpace(s));
        }

        public static bool AllAreNullOrWhiteSpace(params string[] strings)
        {
            return new List<string>(strings).TrueForAll(string.IsNullOrWhiteSpace);
        }

        public static bool OneIsNullOrWhiteSpace(params string[] strings)
        {
            return new List<string>(strings).Any(string.IsNullOrWhiteSpace);
        }

        public static bool OneIsNotNullOrWhiteSpace(params string[] strings)
        {
            return new List<string>(strings).Any(s => !string.IsNullOrWhiteSpace(s));
        }
    }
}