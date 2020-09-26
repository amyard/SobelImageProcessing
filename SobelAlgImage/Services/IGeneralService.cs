using Microsoft.AspNetCore.Http;
using SobelAlgImage.Models;
using System.Threading.Tasks;

namespace SobelAlgImage.Services
{
    public interface IGeneralService
    {
        Task CreateImage(ImageModel img, IFormFileCollection files);
        Task<JsonMessageModel> DeleteImage(int id);
    }
}
