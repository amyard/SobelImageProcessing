using SobelAlgImage.Data;
using SobelAlgImage.Interfaces;
using System.Threading.Tasks;

namespace SobelAlgImage.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext _context;

        public UnitOfWork(StoreContext context)
        {
            _context = context;
            ImageSobelAlg = new ImageAlgorithmRepo(_context);
        }

        public IImageAlgorithmRepo ImageSobelAlg { get; set; }


        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<bool> SaveChangesAsync()
        {
            if (await _context.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }
    }
}
