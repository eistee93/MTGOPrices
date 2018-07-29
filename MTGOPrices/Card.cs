using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTGOPrices
{
    class Card
    {
        public Card()
        {
        }

        public Card(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Uri { get; set; }
    }
}
