using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MTGOPrices
{
    class RequestHandler
    {
        public readonly static String GoatUrl = "https://www.goatbots.com/card/{0}";

        public static string GetContent(string url)
        {
            string result = null;
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
            myRequest.Method = "GET";

            using (WebResponse myResponse = myRequest.GetResponse())
            {
                using (StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8))
                {
                    result = sr.ReadToEnd();
                }
            }

            return result;
        }

        public static IEnumerable<Card> GetCards(string html)
        {
            System.Collections.ObjectModel.Collection<Card> cards = new System.Collections.ObjectModel.Collection<Card>();

            string startCardListString = "<div class=\"deck-contents\">";
            string endCardListString = "<div class=\"deck-footer\">";
            string startCardString = "<li>";
            string endCardString = "</li>";

            int startCardListIndex = html.IndexOf(startCardListString);
            int endCardListIndex = html.IndexOf(endCardListString, startCardListIndex);
            int startCardIndex = html.IndexOf(startCardString, startCardListIndex);

            while (startCardIndex > -1 && startCardIndex < endCardListIndex)
            {
                int endCardIndex = html.IndexOf(endCardString, startCardIndex) + endCardString.Length;
                string cardSubString = html.Substring(startCardIndex, endCardIndex - startCardIndex);
                string cardName = cardSubString.TrimStart(startCardString.ToCharArray()).TrimEnd(endCardString.ToCharArray()).Trim().Substring(2);
                cards.Add(new Card(cardName));
                startCardIndex = html.IndexOf(startCardString, startCardIndex + 1);
            }

            return cards;
        }

        private static IEnumerable<decimal> GetPrices(string html)
        {
            System.Collections.ObjectModel.Collection<decimal> prices = new System.Collections.ObjectModel.Collection<decimal>();

            string startButtonString = "<button class=\"thumbnail\"";
            string endButtonString = "</button>";
            string startFoilString = "data-foil=\"";
            string endFoilString = "\"";
            string startPriceString = "data-price=\"";
            string endPriceString = "\"";

            int startButtonIndex = html.IndexOf(startButtonString);
            System.Globalization.CultureInfo parseCulture = new System.Globalization.CultureInfo("en-US");

            while (startButtonIndex > -1)
            {
                int endButtonIndex = html.IndexOf(endButtonString, startButtonIndex) + endButtonString.Length;
                string buttonSubString = html.Substring(startButtonIndex, endButtonIndex - startButtonIndex);

                int startFoilIndex = buttonSubString.IndexOf(startFoilString) + startFoilString.Length;
                int endFoilIndex = buttonSubString.IndexOf(endFoilString, startFoilIndex);
                string foil = buttonSubString.Substring(startFoilIndex, endFoilIndex - startFoilIndex);

                if (foil == "0")
                {
                    int startPriceIndex = buttonSubString.IndexOf(startPriceString) + startPriceString.Length;
                    int endPriceIndex = buttonSubString.IndexOf(endPriceString, startPriceIndex);
                    string price = buttonSubString.Substring(startPriceIndex, endPriceIndex - startPriceIndex);
                    prices.Add(decimal.Parse(price, parseCulture));
                }

                startButtonIndex = html.IndexOf(startButtonString, startButtonIndex + 1);
            }

            return prices;
        }

        public static void SetLowestPrice(Card card)
        {
            string html = GetContent(card.Uri);
            card.Price = GetPrices(html).Min();
        }
    }
}
