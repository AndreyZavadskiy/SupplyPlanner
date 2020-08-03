using System.ComponentModel.DataAnnotations.Schema;

namespace SP.Core.Model
{
    /// <summary>
    /// Территории, назначенные менеджеру
    /// </summary>
    [Table("PersonTerritory")]
    public class PersonTerritory
    {
        /// <summary>
        /// ID записи
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ID персоны
        /// </summary>
        public int PersonId { get; set; }
        /// <summary>
        /// ID территории
        /// </summary>
        public int RegionalStructureId { get; set; }

        #region Navigation properties

        [ForeignKey("PersonId")]
        public Person Person { get; set; }
        [ForeignKey("RegionalStructureId")]
        public RegionalStructure RegionalStructure { get; set; }
        #endregion
    }
}
