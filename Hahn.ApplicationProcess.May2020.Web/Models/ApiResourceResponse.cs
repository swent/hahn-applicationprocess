using Swashbuckle.AspNetCore.Annotations;

namespace Hahn.ApplicationProcess.May2020.Web.Models {

    /// <summary>
    /// Represents a CRUD response from the applicant api except for successful reads
    /// (which directly return the data instead).
    /// </summary>
    public class ApiResourceResponse {

        /// <summary>
        /// Whether or not the request was successful
        /// </summary>
        [SwaggerSchema("Whether or not the request was successful")]
        public bool Success {
            get;
            set;
        }

        /// <summary>
        /// Id of the resource, if existing
        /// </summary>
        [SwaggerSchema("Id of the resource, if existing")]
        public int? Id {
            get;
            set;
        }

        /// <summary>
        /// Uri of the resource, if existing
        /// </summary>
        [SwaggerSchema("Uri of the resource, if existing")]
        public string Uri {
            get;
            set;
        }

        /// <summary>
        /// Any keys that identify problems of the request
        /// </summary>
        [SwaggerSchema("Any keys that identify problems of the request")]
        public string[] ErrorKeys {
            get;
            set;
        }
    }
}
