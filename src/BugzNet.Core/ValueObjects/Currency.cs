using BugzNet.Core.SharedKernel;
using System;

namespace BugzNet.Core.ValueObjects
{
    public class Currency : ValueObject
    {
        public int Id { get; set; }
		public string Name { get; set; }
        public string DisplayName => $"{Name} ({Id})";
        public string Code { get; set; }
		public short Exponent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="exponent"></param>
        /// <param name="minorUnit"></param>
        /// <param name="code"></param>
        public Currency(int id ,string name, short exponent, string code)
        {
            Id = id;
            Name = name;
            Exponent = exponent;
            Code = code;
        }

        private Currency(){}

        public static Currency CreateNew(Currency currency)
        {
            return new Currency(currency.Id, currency.Name,currency.Exponent, currency.Code);
        }

        public override string ToString()
        {
            return $"{Code}-{Name}";
        }
    }
}
