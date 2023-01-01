using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter_API.models
{
    class Currency
    {
        private int id;
        private string name;
        private decimal value;

        public int Id { get { return id; } set { id = value; } }

        public string Name { get { return name; } set { name = value; } }

        public decimal Value { get { return value; } set { this.value = value; } }

        public Currency() { }

        public Currency(int id, string name, decimal value)
        {
            this.Id = id;
            this.Name = name;
            this.Value = value;
        }

    }
}
