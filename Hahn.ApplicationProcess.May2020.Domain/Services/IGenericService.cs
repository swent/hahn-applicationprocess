using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;

namespace Hahn.ApplicationProcess.May2020.Domain.Services {

    /// <summary>
    /// Provides a generic managed service to CRUD generic entities.
    /// Also provides enhanced business logic, e.g. validation compared to the raw generic repository.
    /// </summary>
    /// <typeparam name="T">May be any GenericEntity inheritant</typeparam>
    public interface IGenericService<T> {

        /// <summary>
        /// Reads all stored entities.
        /// </summary>
        public Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Reads one specific entity identified by id.
        /// </summary>
        /// <param name="id">The id to look up</param>
        /// <exception cref="KeyNotFoundException">If id is not to be found</exception>
        public Task<T> GetAsync(int id);

        /// <summary>
        /// Creates a new entity based on the given dataset.
        /// <param name="entity">The dataset to use for entity creation</param>
        /// </summary>
        /// <exception cref="ValidationException">If validation fails</exception>
        public Task<T> CreateAsync(T entity);

        /// <summary>
        /// Updates an existing entity based on the given dataset.
        /// <param name="entity">The dataset to use for the entity update</param>
        /// </summary>
        /// <exception cref="KeyNotFoundException">If id is not to be found</exception>
        /// <exception cref="ValidationException">If validation fails</exception>
        public Task UpdateAsync(T entity);

        /// <summary>
        /// Deletes one specific entity identified by id.
        /// </summary>
        /// <param name="id">The id to look up</param>
        /// <exception cref="KeyNotFoundException">If id is not to be found</exception>
        public Task DeleteAsync(int id);
    }
}
