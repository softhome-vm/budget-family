using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace budget.models
{
    [Table("Бюджет")]
    public class Budgets
    {
        [Key]
        [Column("idБюджета")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdBudget { get; set; }

        [Column("idКатегории")]
        [Required]
        public int IdKategory { get; set; }

        [Column("Дата")]
        public DateTime Date { get; set; }

        [Column("Лимит")]
        public decimal Limit { get; set; }

        [ForeignKey("IdKategory")]
        public Kategories? Kategory { get; set; }
    }
}
