using Microsoft.EntityFrameworkCore;
using SobelAlgImage.Models;

namespace SobelAlgImage.Data
{
    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions<StoreContext> options) : base(options)
        {
        }

        public DbSet<ImageModel> ImageModels { get; set; }
    }
}
