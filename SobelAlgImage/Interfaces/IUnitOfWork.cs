using System;
using System.Threading.Tasks;

namespace SobelAlgImage.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IImageAlgorithmRepo ImageSobelAlg { get; set; }
        Task<bool> SaveChangesAsync();
    }
}
