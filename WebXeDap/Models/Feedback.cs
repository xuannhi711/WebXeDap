using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebXeDap.Models
{
    public class Feedback
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }
        
        public ApplicationUser User { get; set; }

        public string NoiDung { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.Now;
    }

}
