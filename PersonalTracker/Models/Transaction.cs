using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalTracker.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public int Amount { get; set; }
        [Column(TypeName = "nvarchar(75)")]
        public string Note { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        [NotMapped]
        public string? CategoryTitleWithIcon
        {
            get
            {
                return Category == null ? "" : Category.Icon + " " + Category.Title;
            }
        }

        [NotMapped]
        public string? FromattedAmount
        {
            get
            {
                return ((Category == null || Category.Type == "Расход") ? "-" : "+ ") + Amount.ToString("C0");
            }
        }

    }
}
