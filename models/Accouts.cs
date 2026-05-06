using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace budget.models
{
    [Table("Аккаунты")]
    public class Accouts
    {
        [Key]
        [Column("idАккаунта")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdAccouts { get; set; }

        [Column("idMember")]
        [Required]
        public int IdMember { get; set; }

        [Column("Название банка")]
        public string? NameBank { get; set; }

        [Column("Balance")]
        public decimal Balance { get; set; }

        [ForeignKey("IdMember")]
        public Family? Member { get; set; }

        public ICollection<Transitions> TransitionsList { get; set; } = [];
        [NotMapped]
        public string DisplayName => $"{NameBank ?? "Без названия"} ({Balance:C})";
    }
}
