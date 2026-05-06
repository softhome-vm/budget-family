using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace budget.models
{
    [Table("Категории")]
    public class Kategories
    {
        [Key]
        [Column("idКатегории")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdKategory { get; set; }

        [Column("Название_категории")]
        public string? NameKategory { get; set; }

        // true = доход (зачисление), false = расход
        [Column("Тип")]
        [Required]
        public bool IsIncome { get; set; }

        public ICollection<Transitions> TransitionsList { get; set; } = [];
        public ICollection<Budgets> BudgetsList { get; set; } = [];

        [NotMapped]
        public string TypeName => IsIncome ? "Доход" : "Расход";

        [NotMapped]
        public string DisplayName => $"{NameKategory} ({TypeName})";
    }
}
