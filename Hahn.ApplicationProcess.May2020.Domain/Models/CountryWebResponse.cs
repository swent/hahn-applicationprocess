namespace Hahn.ApplicationProcess.May2020.Domain.Models {

    /// <summary>
    /// Model to process data from restcountries.eu api.
    /// </summary>
    public class CountryWebResponse {

        /// <summary>
        /// Country name
        /// </summary>
        public string Name {
            get;
            set;
        }

        /// <summary>
        /// Array of lop level domains
        /// </summary>
        public string[] TopLevelDomain {
            get;
            set;
        }
    }
}
