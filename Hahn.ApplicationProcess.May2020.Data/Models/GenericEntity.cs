namespace Hahn.ApplicationProcess.May2020.Data.Models {

    /// <summary>
    /// A generic entity to be used as parent class for other entities that are required to use the EF db context.
    /// </summary>
    public abstract class GenericEntity {

        /// <summary>
        /// Primary key / id of any entity.
        /// </summary>
        public int Id {
            get;
            set;
        }
    }
}
