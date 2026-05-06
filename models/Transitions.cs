using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace budget.models
{
    [Table("Транзакции")]
    public class Transitions
    {
        [Key]
        [Column("idТранзакции")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdTransitions { get; set; }

        [Column("idАккаунта")]
        [Required]
        public int IdAccout { get; set; }

        [Column("idКатегории")]
        [Required]
        public int IdKategories { get; set; }

        [Column("Сумма")]
        public decimal Amount { get; set; }

        [Column("Дата")]
        public DateTime Date { get; set; }

        [Column("Комментарий")]
        public string? Description { get; set; }

        [ForeignKey("IdAccout")]
        public Accouts? Accout { get; set; }

        [ForeignKey("IdKategories")]
        public Kategories? Kategory { get; set; }
    }
}
