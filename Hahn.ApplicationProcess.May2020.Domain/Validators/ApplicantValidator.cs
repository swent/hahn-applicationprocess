using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using FluentValidation;
using Hahn.ApplicationProcess.May2020.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Hahn.ApplicationProcess.May2020.Domain.Validators {

    /// <summary>
    /// Applicant validator to be consumed in an GenericService<Applicant>.
    /// Uses data from restcountries.eu api for some validation checks.
    /// </summary>
    public class ApplicantValidator : AbstractValidator<Applicant> {

        /// <summary>
        /// Cache that holds all valid countries.
        /// </summary>
        private HashSet<string> CountryCache {
            get {
                if (_countryCache.Count == 0) {
                    LoadCaches();
                }
                return _countryCache;
            }
        }

        /// <summary>
        /// Cache that holds all valid top level domains.
        /// </summary>
        private HashSet<string> TldCache {
            get {
                if (_countryCache.Count == 0) {
                    LoadCaches();
                }
                return _tldCache;
            }
        }

        private readonly ILogger _logger;
        private readonly Regex _eMailRegex;
        private readonly HashSet<string> _countryCache;
        private readonly HashSet<string> _tldCache;

        public ApplicantValidator(ILogger<ApplicantValidator> logger) {
            _logger = logger;
            _eMailRegex = new Regex(".*@.*\\.[a-z]+", RegexOptions.IgnoreCase);
            _countryCache = new HashSet<string>();
            _tldCache = new HashSet<string>();

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("required")
                .MinimumLength(5).WithMessage("minlen-5");
            RuleFor(x => x.FamilyName)
                .NotEmpty().WithMessage("required")
                .MinimumLength(5).WithMessage("minlen-5");
            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("required")
                .MinimumLength(10).WithMessage("minlen-10");
            RuleFor(x => x.CountryOfOrigin)
                .NotEmpty().WithMessage("required")
                .Must(BeValidCountry).WithMessage("invalid");
            RuleFor(x => x.EMailAddress)
                .NotEmpty().WithMessage("required")
                .Must(BeValidEMailAddress).WithMessage("invalid");
            RuleFor(x => x.Age)
                .InclusiveBetween(20, 60).WithMessage("range-20-60");
        }

        /// <summary>
        /// Checks for a country to be valid according to restcountries.eu api.
        /// </summary>
        /// <param name="country">The country to check</param>
        private bool BeValidCountry(string country) {
            if (string.IsNullOrEmpty(country)) {
                return false;
            }

            return CountryCache.Contains(country.ToLower());
        }

        /// <summary>
        /// Checks for an email to be of valid format
        /// and contain a valid top level domain according to restcountries.eu api.
        /// </summary>
        /// <param name="eMailAddress">The email to check</param>
        private bool BeValidEMailAddress(string eMailAddress) {
            if (string.IsNullOrEmpty(eMailAddress)) {
                return false;
            }

            if (!_eMailRegex.IsMatch(eMailAddress)) {
                return false;
            }

            var eMailTld = eMailAddress
                .Split('.')
                .Last()
                .ToLower();
            return TldCache.Contains(eMailTld);
        }

        /// <summary>
        /// Loads the country and top level domain caches from restcountries.eu data.
        /// </summary>
        private void LoadCaches() {
            _countryCache.Clear();
            _tldCache.Clear();

            /* Add international tlds */
            _tldCache.Add("com");
            _tldCache.Add("net");
            _tldCache.Add("info");
            _tldCache.Add("org");
            _tldCache.Add("at");
            _tldCache.Add("ch");
            _tldCache.Add("li");
            _tldCache.Add("de");
            _tldCache.Add("eu");

            try {
                /* Request all countries */
                var responseText = WebGet("https://restcountries.eu/rest/v2/all");
                var countries = JsonSerializer.Deserialize<IEnumerable<CountryWebResponse>>(responseText,
                    new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase,});

                /* Add countries and their tlds to cache */
                foreach (var country in countries) {
                    _countryCache.Add(country.Name.ToLower());
                    foreach (var tld in country.TopLevelDomain) {
                        if (string.IsNullOrEmpty(tld)) {
                            continue;
                        }
                        _tldCache.Add(tld.Substring(1).ToLower());
                    }
                }
            }
            catch (Exception e) {
                _logger.LogError($"Cache loading encountered an error: {e.Message} ${e.StackTrace}");
                throw;
            }
        }

        /// <summary>
        /// Requests data from a given uri and returns the response body as string.
        /// </summary>
        /// <param name="uri">The uri to request data from</param>
        public string WebGet(string uri) {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream)) {
                return reader.ReadToEnd();
            }
        }
    }
}
