using Hahn.ApplicationProcess.May2020.Data.Models;

namespace Hahn.ApplicationProcess.May2020.Domain.Models {

    /// <summary>
    /// Applicant domain model.
    /// In this case used throughout all assemblies.
    /// </summary>
    public class Applicant : GenericEntity {

        /// <summary>
        /// First name
        /// </summary>
        public string Name {
            get;
            set;
        }

        /// <summary>
        /// Family name
        /// </summary>
        public string FamilyName {
            get;
            set;
        }

        /// <summary>
        /// Address
        /// </summary>
        public string Address {
            get;
            set;
        }

        /// <summary>
        /// Country of origin
        /// </summary>
        public string CountryOfOrigin {
            get;
            set;
        }

        /// <summary>
        /// E-Mail address
        /// </summary>
        public string EMailAddress {
            get;
            set;
        }

        /// <summary>
        /// Age
        /// </summary>
        public int Age {
            get;
            set;
        }

        /// <summary>
        /// Hired yes/no
        /// </summary>
        public bool Hired {
            get;
            set;
        }
    }
}
