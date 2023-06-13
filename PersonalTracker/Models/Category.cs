using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalTracker.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string Title { get; set; }

        [Column(TypeName = "nvarchar(5)")]
        public string Icon { get; set; } = "";

        [Column(TypeName = "nvarchar(10)")]
        public string Type { get; set; } = "Expense";
    }
}
