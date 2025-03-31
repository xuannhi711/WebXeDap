using Microsoft.AspNetCore.Identity;

namespace WebXeDap.Models
{
    
public class ApplicationUser : IdentityUser
    {
        // Thêm các thuộc tính tùy chỉnh cho người dùng
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? ProfilePicture { get; set; } 
    }


}
