using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EyePatch.Core.Models.Forms
{
    public interface IFacebookForm
    {
        string Id { get; set; }
        string Type { get; set; }
        string Email { get; set; }
        string Phone { get; set; }
        string Image { get; set; }
        string StreetAddress { get; set; }
        string Locality { get; set; }
        string Region { get; set; }
        string Country { get; set; }
        string Postcode { get; set; }
        double? Longitude { get; set; }
        double? Latitude { get; set; }
    }

    public class FacebookForm : IFacebookForm
    {
        private static IList<KeyValuePair<string, string>> types;

        public IEnumerable<KeyValuePair<string, string>> Types
        {
            get
            {
                if (types == null)
                {
                    types = new List<KeyValuePair<string, string>>();
                    types.Add(new KeyValuePair<string, string>("article", "article"));
                    types.Add(new KeyValuePair<string, string>("blog", "blog"));
                    types.Add(new KeyValuePair<string, string>("actor", "actor"));
                    types.Add(new KeyValuePair<string, string>("album", "album"));
                    types.Add(new KeyValuePair<string, string>("athlete", "athlete"));
                    types.Add(new KeyValuePair<string, string>("author", "author"));
                    types.Add(new KeyValuePair<string, string>("band", "band"));
                    types.Add(new KeyValuePair<string, string>("bar", "bar"));
                    types.Add(new KeyValuePair<string, string>("book", "book"));
                    types.Add(new KeyValuePair<string, string>("cafe", "cafe"));
                    types.Add(new KeyValuePair<string, string>("cause", "cause"));
                    types.Add(new KeyValuePair<string, string>("city", "city"));
                    types.Add(new KeyValuePair<string, string>("company", "company"));
                    types.Add(new KeyValuePair<string, string>("country", "country"));
                    types.Add(new KeyValuePair<string, string>("director", "director"));
                    types.Add(new KeyValuePair<string, string>("drink", "drink"));
                    types.Add(new KeyValuePair<string, string>("food", "food"));
                    types.Add(new KeyValuePair<string, string>("game", "game"));
                    types.Add(new KeyValuePair<string, string>("government", "government"));
                    types.Add(new KeyValuePair<string, string>("hotel", "hotel"));
                    types.Add(new KeyValuePair<string, string>("landmark", "landmark"));
                    types.Add(new KeyValuePair<string, string>("movie", "movie"));
                    types.Add(new KeyValuePair<string, string>("musician", "musician"));
                    types.Add(new KeyValuePair<string, string>("non_profit", "non_profit"));
                    types.Add(new KeyValuePair<string, string>("politician", "politician"));
                    types.Add(new KeyValuePair<string, string>("product", "product"));
                    types.Add(new KeyValuePair<string, string>("public_figure", "public_figure"));
                    types.Add(new KeyValuePair<string, string>("restaurant", "restaurant"));
                    types.Add(new KeyValuePair<string, string>("school", "school"));
                    types.Add(new KeyValuePair<string, string>("song", "song"));
                    types.Add(new KeyValuePair<string, string>("sport", "sport"));
                    types.Add(new KeyValuePair<string, string>("sports_league", "sports_league"));
                    types.Add(new KeyValuePair<string, string>("sports_team", "sports_team"));
                    types.Add(new KeyValuePair<string, string>("state_province", "state_province"));
                    types.Add(new KeyValuePair<string, string>("tv_show", "tv_show"));
                    types.Add(new KeyValuePair<string, string>("university", "university"));
                    types.Add(new KeyValuePair<string, string>("website", "website"));
                }
                return types;
            }
        }

        #region IFacebookForm Members

        public string Id { get; set; }

        public string Type { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        public string Image { get; set; }

        public string StreetAddress { get; set; }

        public string Locality { get; set; }

        public string Region { get; set; }

        public string Country { get; set; }

        public string Postcode { get; set; }

        public double? Longitude { get; set; }

        public double? Latitude { get; set; }

        #endregion
    }
}