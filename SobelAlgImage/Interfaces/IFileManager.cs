using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace SobelAlgImage.Interfaces
{
    public interface IFileManager
    {
        Task<string> SaveImage(IFormFileCollection files, string imageBasePath, string imageResultPath, string filename);
        bool RemoveImage(string filePath);
    }
}
