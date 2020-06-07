using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Hahn.ApplicationProcess.May2020.Data.Models;
using Hahn.ApplicationProcess.May2020.Data.Repositories;
using Microsoft.Extensions.Logging;

namespace Hahn.ApplicationProcess.May2020.Domain.Services {

    /// <summary>
    /// Provides a generic managed service to CRUD generic entities.
    /// Also provides enhanced business logic, e.g. validation compared to the raw generic repository.
    /// </summary>
    /// <typeparam name="T">May be any GenericEntity inheritant</typeparam>
    public class GenericService<T> : IGenericService<T> where T : GenericEntity {

        private readonly ILogger _logger;
        private readonly IGenericRepository<T> _repository;
        private readonly AbstractValidator<T> _validator;

        public GenericService(ILogger<GenericService<T>> logger, IGenericRepository<T> repository, AbstractValidator<T> validator) {
            _logger = logger;
            _repository = repository;
            _validator = validator;
        }

        /// <summary>
        /// Reads all stored entities.
        /// </summary>
        public async Task<IEnumerable<T>> GetAllAsync() {
            return await _repository.GetAllAsync();
        }

        /// <summary>
        /// Reads one specific entity identified by id.
        /// </summary>
        /// <param name="id">The id to look up</param>
        /// <exception cref="KeyNotFoundException">If id is not to be found</exception>
        public async Task<T> GetAsync(int id) {
            return await _repository.GetAsync(id);
        }

        /// <summary>
        /// Creates a new entity based on the given dataset.
        /// <param name="entity">The dataset to use for entity creation</param>
        /// </summary>
        /// <exception cref="ValidationException">If validation fails</exception>
        public async Task<T> CreateAsync(T entity) {
            DoValidate(entity);

            return await _repository.CreateAsync(entity);
        }

        /// <summary>
        /// Updates an existing entity based on the given dataset.
        /// <param name="entity">The dataset to use for the entity update</param>
        /// </summary>
        /// <exception cref="KeyNotFoundException">If id is not to be found</exception>
        /// <exception cref="ValidationException">If validation fails</exception>
        public async Task UpdateAsync(T entity) {
            DoValidate(entity);

            await _repository.UpdateAsync(entity);
        }

        /// <summary>
        /// Deletes one specific entity identified by id.
        /// </summary>
        /// <param name="id">The id to look up</param>
        /// <exception cref="KeyNotFoundException">If id is not to be found</exception>
        public async Task DeleteAsync(int id) {
            await _repository.DeleteAsync(id);
        }

        /// <summary>
        /// Validates a given entity if validation is available.
        /// </summary>
        /// <param name="entity"></param>
        /// <exception cref="ValidationException">If validation fails</exception>
        private void DoValidate(T entity) {
            /* Validate entity if validators are available */
            if (_validator != null) {
                ValidationResult validationResult;
                try {
                    validationResult = _validator.Validate(entity);
                }
                catch (Exception e) {
                    _logger.LogError($"Entity validation encountered an error: {e.Message} {e.StackTrace}");
                    throw;
                }

                if (!validationResult.IsValid) {
                    throw new ValidationException("invalid", validationResult.Errors);
                }
            }
        }
    }
}
