using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace budget.models
{
    [Table("Члены семьи")]
    public class Family
    {
        [Key]
        [Column("idMember")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdMember { get; set; }

        [Column("Фамилия")]
        [Required]
        public string LastName { get; set; } = string.Empty;

        [Column("Имя")]
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Column("Отчество")]
        public string? MiddleName { get; set; }

        [Column("Номер телефона")]
        public string? NumberPhone { get; set; }

        [Column("Эл_почта")]
        public string? Email { get; set; }

        public ICollection<Accouts> AccoutsList { get; set; } = [];

        [NotMapped]
        public string FullName => $"{LastName} {FirstName} {(string.IsNullOrEmpty(MiddleName) ? "" : MiddleName)}".Trim();
    }
}
