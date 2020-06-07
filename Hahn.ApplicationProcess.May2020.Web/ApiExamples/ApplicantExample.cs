using Hahn.ApplicationProcess.May2020.Domain.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Hahn.ApplicationProcess.May2020.Web.ApiExamples {

    public class ApplicantExample : IExamplesProvider<Applicant> {

        public Applicant GetExamples() {
            return new Applicant {
                Id = 1,
                Name = "Maximilian",
                FamilyName = "Mustermann",
                Address = "Musterstraße 7, 56351 Hausen",
                CountryOfOrigin = "Germany",
                EMailAddress = "max.mustermann@gmail.com",
                Age = 30,
                Hired = true,
            };
        }
    }
}
