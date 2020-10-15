using Microsoft.AspNetCore.Http;
using SobelAlgImage.Models.DataModels;
using System.Drawing;
using System.Threading.Tasks;

namespace SobelAlgImage.Infrastructure.Interfaces
{
    public interface IGeneralService
    {
        Task CreateImageAsync(ImageModel img, IFormFileCollection files);
        Task<JsonMessageModel> DeleteImageAsync(int id);
        Bitmap ConvertImageWithTasks(Bitmap sourceOriginal, int tiles, int algorithmChooser, int greyScale);
    }
}
