using Microsoft.AspNet.Identity.EntityFramework;

namespace MyStoryMaker.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        public System.Data.Entity.DbSet<MyStoryMaker.Models.Collage> Collages { get; set; }

        public System.Data.Entity.DbSet<MyStoryMaker.Models.Story> Stories { get; set; }

        public System.Data.Entity.DbSet<MyStoryMaker.Models.StoryBlock> StoryBlocks { get; set; }

        public System.Data.Entity.DbSet<MyStoryMaker.Models.StoryCollection> StoryCollections { get; set; }
    }
}