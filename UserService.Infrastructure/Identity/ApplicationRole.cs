using Microsoft.AspNetCore.Identity;

namespace UserService.Infrastructure.Identity
{
    public class ApplicationRole:IdentityRole<Guid>
    {
        //Here, Guid will be the data type of the Primary column in the Roles table
        //You can also specify String, Integer 

        public string? Description { get; set; }
    }
}
