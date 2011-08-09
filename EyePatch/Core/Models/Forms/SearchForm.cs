using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EyePatch.Core.Models.Forms
{
    public interface ISearchForm
    {
        int Id { get; set; }
        string Description { get; set; }
        string Keywords { get; set; }
        int? Language { get; set; }
        string Charset { get; set; }
        string Author { get; set; }
        string Copyright { get; set; }
        string Robots { get; set; }
    }

    public class SearchForm : ISearchForm
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public string Keywords { get; set; }

        public int? Language { get; set; }

        public string Charset { get; set; }

        public string Author { get; set; }

        public string Copyright { get; set; }

        public string Robots { get; set; }

        private static IEnumerable<KeyValuePair<int, string>> languages;

        public IEnumerable<KeyValuePair<int, string>> Languages
        {
            get
            {
                if (languages == null)
                {
                    var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
                    languages = cultures.Select(c => new KeyValuePair<int, string>(c.LCID, c.DisplayName)).OrderBy(l => l.Value).ToList();
                }
                return languages;
            }
        }

        private static IList<KeyValuePair<string, string>> charsets;

        public IEnumerable<KeyValuePair<string, string>> Charsets
        {
            get
            {
                if (charsets == null)
                {
                    charsets = new List<KeyValuePair<string, string>>();
                    charsets.Add(new KeyValuePair<string, string>("UTF-8", "UTF-8 (1 to 4 byte Unicode)"));
                    charsets.Add(new KeyValuePair<string, string>("UTF-16", "UTF-16 (16-bit Unicode)"));
                    charsets.Add(new KeyValuePair<string, string>("ISO-8859-1", "ISO-8859-1 (Latin alphabet part 1)"));
                    charsets.Add(new KeyValuePair<string, string>("ISO-8859-1", "ISO-8859-2 (Latin alphabet part 2)"));
                    charsets.Add(new KeyValuePair<string, string>("ISO-8859-3", "ISO-8859-3 (Latin alphabet part 3)"));
                    charsets.Add(new KeyValuePair<string, string>("ISO-8859-4", "ISO-8859-4 (Latin alphabet part 4)"));
                    charsets.Add(new KeyValuePair<string, string>("ISO-8859-5", "ISO-8859-5 (Latin/Cyrillic part 5)"));
                    charsets.Add(new KeyValuePair<string, string>("ISO-8859-6", "ISO-8859-6 (Latin/Arabic part 6)"));
                    charsets.Add(new KeyValuePair<string, string>("ISO-8859-7", "ISO-8859-7 (Latin/Greek part 7)"));
                    charsets.Add(new KeyValuePair<string, string>("ISO-8859-8", "ISO-8859-8 (Latin/Hebrew part 8)"));
                    charsets.Add(new KeyValuePair<string, string>("ISO-8859-9", "ISO-8859-9 (Latin 5 part 9)"));
                    charsets.Add(new KeyValuePair<string, string>("ISO-8859-10", "ISO-8859-10 (Latin 6 Lappish, Nordic)"));
                    charsets.Add(new KeyValuePair<string, string>("ISO-8859-15", "ISO-8859-15 (Latin 9 (aka Latin 0))"));
                    charsets.Add(new KeyValuePair<string, string>("ISO-2022-JP", "ISO-2022-JP (Latin/Japanese part 1)"));
                    charsets.Add(new KeyValuePair<string, string>("ISO-2022-JP-2", "ISO-2022-JP-2 (Latin/Japanese part 2)"));
                    charsets.Add(new KeyValuePair<string, string>("ISO-2022-KR", "ISO-2022-KR (Latin/Korean part 1)"));
                }
                return charsets;
            }
        }
    }
}