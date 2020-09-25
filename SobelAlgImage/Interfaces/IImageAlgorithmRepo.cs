using SobelAlgImage.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SobelAlgImage.Interfaces
{
    public interface IImageAlgorithmRepo
    {
        Task<ImageModel> GetImageByIdAsync(int id);
        Task CreateImageAsync(ImageModel img);
        Task<IReadOnlyList<ImageModel>> GetListOfImagesAsync();
        Task DeleteImageAsync(int id);
    }
}
