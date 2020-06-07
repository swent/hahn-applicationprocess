using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using FluentValidation;
using Hahn.ApplicationProcess.May2020.Domain.Models;
using Hahn.ApplicationProcess.May2020.Domain.Services;
using Hahn.ApplicationProcess.May2020.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace Hahn.ApplicationProcess.May2020.Web.Controllers {

    /// <summary>
    /// Api-controller to manage all applicant requests.
    /// </summary>
    [ApiController]
    [Route("/applicant")]
    [SwaggerTag("Create, read, update and delete applicants")]
    public class ApplicantsController : ControllerBase {

        private readonly ILogger<ApplicantsController> _logger;
        private readonly IGenericService<Applicant> _applicantService;

        public ApplicantsController(ILogger<ApplicantsController> logger, IGenericService<Applicant> applicantService) {
            _logger = logger;
            _applicantService = applicantService;
        }

        /// <summary>
        /// Returns an array of all stored applicants.
        /// </summary>
        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, "Applicants successfully retrieved", typeof(Applicant[]))]
        [SwaggerResponse(500, "A severer error occured")]
        [Produces( MediaTypeNames.Application.Json )]
        public async Task<IActionResult> GetAll() {
            try {
                return Ok(await _applicantService.GetAllAsync());
            }
            catch (Exception e) {
                _logger.LogError($"Api error in [{Request.Method}] {Request.Path}", e);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Returns one specific applicant identified by a given id.
        /// </summary>
        /// <param name="id" example="1">Id of the applicant to return</param>
        [HttpGet("{id:int}")]
        [SwaggerResponse(200, "Applicant successfully retrieved", typeof(Applicant))]
        [SwaggerResponse(404, "Applicant id could not be found", typeof(ApiResourceResponse))]
        [SwaggerResponse(500, "A severer error occured")]
        [Produces( MediaTypeNames.Application.Json )]
        public async Task<IActionResult> Get(int id) {
            try {
                return Ok(await _applicantService.GetAsync(id));
            }
            catch (KeyNotFoundException) {
                return NotFound(new ApiResourceResponse {
                    Success = false,
                    ErrorKeys = new []{ "id-not-found" },
                });
            }
            catch (Exception e) {
                _logger.LogError($"Api error in [{Request.Method}] {Request.Path}", e);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Creates a new applicant based on the given data and returns the id and uri upon success.
        /// </summary>
        [HttpPost]
        [SwaggerResponse(201, "Applicant successfully created", typeof(ApiResourceResponse))]
        [SwaggerResponse(400, "Applicant data is invalid", typeof(ApiResourceResponse))]
        [SwaggerResponse(500, "A severer error occured")]
        [Consumes( MediaTypeNames.Application.Json )]
        [Produces( MediaTypeNames.Application.Json )]
        public async Task<IActionResult> Create(Applicant applicant) {
            try {
                var newApplicant = await _applicantService.CreateAsync(applicant);
                var uri = $"applicant/{newApplicant.Id}";
                return Created(uri, new ApiResourceResponse {Success = true, Id = newApplicant.Id, Uri = uri,});
            }
            catch (ValidationException e) {
                return BadRequest(new ApiResourceResponse {
                    Success = false,
                    ErrorKeys = e.Errors.Select(e => $"{e.ErrorMessage}:{e.PropertyName}").ToArray(),
                });
            }
            catch (Exception e) {
                _logger.LogError($"Api error in [{Request.Method}] {Request.Path}", e);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Updates an existing applicant using the given data and returns the id and uri upon success.
        /// </summary>
        /// <param name="id" example="1">Id of the applicant to update</param>
        [HttpPut("{id}")]
        [SwaggerResponse(200, "Applicant successfully created", typeof(ApiResourceResponse))]
        [SwaggerResponse(400, "Applicant data is invalid", typeof(ApiResourceResponse))]
        [SwaggerResponse(404, "Applicant id could not be found", typeof(ApiResourceResponse))]
        [SwaggerResponse(500, "A severer error occured")]
        [Consumes( MediaTypeNames.Application.Json )]
        [Produces( MediaTypeNames.Application.Json )]
        public async Task<IActionResult> Update(int id, [FromBody]Applicant applicant) {
            try {
                applicant.Id = id;
                await _applicantService.UpdateAsync(applicant);
                return Ok(new ApiResourceResponse {
                    Success = true,
                    Id = id,
                    Uri = $"applicant/{id}",
                });
            }
            catch (ValidationException e) {
                return BadRequest(new ApiResourceResponse {
                    Success = false,
                    ErrorKeys = e.Errors.Select(e => $"{e.ErrorMessage}:{e.PropertyName}").ToArray(),
                });
            }
            catch (KeyNotFoundException) {
                return NotFound(new ApiResourceResponse {
                    Success = false,
                    ErrorKeys = new []{ "id-not-found" },
                });
            }
            catch (Exception e) {
                _logger.LogError($"Api error in [{Request.Method}] {Request.Path}", e);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Deletes an existing applicant identified by a given id.
        /// </summary>
        /// <param name="id" example="1">Id of the applicant to delete</param>
        [HttpDelete("{id}")]
        [SwaggerResponse(200, "Applicant successfully created", typeof(ApiResourceResponse))]
        [SwaggerResponse(404, "Applicant id could not be found", typeof(ApiResourceResponse))]
        [SwaggerResponse(500, "A severer error occured")]
        [Produces( MediaTypeNames.Application.Json )]
        public async Task<IActionResult> Delete(int id) {
            try {
                await _applicantService.DeleteAsync(id);
                return Ok(new ApiResourceResponse {
                    Success = true,
                    Id = null,
                    Uri = null,
                });
            }
            catch (KeyNotFoundException) {
                return NotFound(new ApiResourceResponse {
                    Success = false,
                    ErrorKeys = new []{ "id-not-found" },
                });
            }
            catch (Exception e) {
                _logger.LogError($"Api error in [{Request.Method}] {Request.Path}", e);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
