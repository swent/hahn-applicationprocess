using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hahn.ApplicationProcess.May2020.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Hahn.ApplicationProcess.May2020.Data.Repositories {

    /// <summary>
    /// Repository that can be used with a generic db context and any generic entity to CRUD EF databases.
    /// </summary>
    /// <typeparam name="T">May be any GenericEntity inheritant</typeparam>
    public class GenericRepository<T> : IGenericRepository<T> where T : GenericEntity {

        private readonly GenericDbContext<T> _applicantsDbContext;
        private static readonly Random _rnd = new Random(); /* We just use random integers as ids */

        public GenericRepository(GenericDbContext<T> applicantsDbContext) {
            _applicantsDbContext = applicantsDbContext;
        }

        /// <summary>
        /// Reads all stored entities.
        /// </summary>
        public async Task<IEnumerable<T>> GetAllAsync() {
            return await _applicantsDbContext.Entities.ToListAsync();
        }

        /// <summary>
        /// Reads one specific entity identified by id.
        /// </summary>
        /// <param name="id">The id to look up</param>
        /// <exception cref="KeyNotFoundException">If id is not to be found</exception>
        public async Task<T> GetAsync(int id) {
            var readingEntity = await _applicantsDbContext.Entities.FindAsync(id);

            if (readingEntity != null) {
                return readingEntity;
            }
            else {
                throw new KeyNotFoundException("entity with given id could not be found");
            }
        }

        /// <summary>
        /// Creates a new entity based on the given dataset.
        /// <param name="entity">The dataset to use for entity creation</param>
        /// </summary>
        public async Task<T> CreateAsync(T entity) {
            var unusedId = await GetUnusedIdAsync();
            entity.Id = unusedId;
            var newEntry = await _applicantsDbContext.Entities.AddAsync(entity);
            await _applicantsDbContext.SaveChangesAsync();
            return newEntry.Entity;
        }

        /// <summary>
        /// Updates an existing entity based on the given dataset.
        /// <param name="entity">The dataset to use for the entity update</param>
        /// </summary>
        /// <exception cref="KeyNotFoundException">If id is not to be found</exception>
        public async Task UpdateAsync(T entity) {
            var updatingEntity = await _applicantsDbContext.Entities.FindAsync(entity.Id);

            if (updatingEntity != null) {
                var entry = _applicantsDbContext.Entry(updatingEntity);
                entry.CurrentValues.SetValues(entity);
                entry.State = EntityState.Modified;
                await _applicantsDbContext.SaveChangesAsync();
            }
            else {
                throw new KeyNotFoundException("entity with given id could not be found");
            }
        }

        /// <summary>
        /// Deletes one specific entity identified by id.
        /// </summary>
        /// <param name="id">The id to look up</param>
        /// <exception cref="KeyNotFoundException">If id is not to be found</exception>
        public async Task DeleteAsync(int id) {
            var deletingEntity = await _applicantsDbContext.Entities.FindAsync(id);

            if (deletingEntity != null) {
                _applicantsDbContext.Entities.Remove(deletingEntity);
                await _applicantsDbContext.SaveChangesAsync();
            }
            else {
                throw new KeyNotFoundException("entity with given id could not be found");
            }
        }

        /// <summary>
        /// Identifies an unused id and returns it.
        /// </summary>
        private async Task<int> GetUnusedIdAsync() {
            int unusedId;
            /* Note: this will get slower for large datasets -> only use for testing purposes */
            do {
                unusedId = _rnd.Next();
            } while (await _applicantsDbContext.Entities.AnyAsync(e => e.Id == unusedId));
            return unusedId;
        }
    }
}
