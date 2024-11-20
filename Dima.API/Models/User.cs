using Microsoft.AspNetCore.Identity;

namespace Dima.API.Models
{
    public class User : IdentityUser<long>
    {
        public List<IdentityRole<long>>? Roles { get; set; } 
    }
}
  