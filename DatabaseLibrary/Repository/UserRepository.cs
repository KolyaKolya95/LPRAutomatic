using DatabaseLibrary.Model;
using System.Data.Entity;

namespace DatabaseLibrary
{
    public class UserRepository : DbContext
    {
        public UserRepository() : base("DefaultConnection")
        {
            Database.SetInitializer<UserRepository>(new DropCreateDatabaseIfModelChanges<UserRepository>());

        }

        public DbSet<UserModel> Users { get; set; }
    }
}
