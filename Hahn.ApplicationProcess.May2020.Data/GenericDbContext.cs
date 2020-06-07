using Hahn.ApplicationProcess.May2020.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Hahn.ApplicationProcess.May2020.Data {

    /// <summary>
    /// A generic db context to be used for manipulating a generic entity T in EF.
    /// </summary>
    /// <typeparam name="T">May be any GenericEntity inheritant</typeparam>
    public class GenericDbContext<T> : DbContext where T : GenericEntity {

        public DbSet<T> Entities { get; set; }

        public GenericDbContext(DbContextOptions<GenericDbContext<T>> options) : base(options) { }
    }
}
