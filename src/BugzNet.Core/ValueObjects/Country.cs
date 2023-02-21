using BugzNet.Core.SharedKernel;

namespace BugzNet.Core.ValueObjects
{
    public class Country : ValueObject
    {
        public string Alpha2Code { get;  set; }
        public string Name { get;  set; }
        public string Alpha3Code { get;  set; }
        public int IsoNumericCode { get;  set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alpha2code"></param>
        /// <param name="name"></param>
        /// <param name="alpha3code"></param>
        /// <param name="isocode"></param>
        public Country(string alpha2Code, string alpha3Code, string name, int isocode)
        {
            Alpha2Code = alpha2Code;
            Name = name;
            Alpha3Code = alpha3Code;
            IsoNumericCode = isocode;
        }

        private Country(){}

        public static Country CreateNew(Country country)
        {
            return new Country(country.Alpha2Code,country.Alpha3Code,country.Name,country.IsoNumericCode);
        }
    }
}
