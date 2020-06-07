using Hahn.ApplicationProcess.May2020.Web.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Hahn.ApplicationProcess.May2020.Web.ApiExamples {

    public class ApiResourceResponseExample : IExamplesProvider<ApiResourceResponse> {

        public ApiResourceResponse GetExamples() {
            return new ApiResourceResponse {
                Success = true,
                Id = 1,
                Uri = "applicant/1",
                ErrorKeys = null,
            };
        }
    }
}
