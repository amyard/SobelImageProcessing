using Microsoft.AspNetCore.Http;
using SobelAlgImage.Models;
using System.Drawing;
using System.Threading.Tasks;

namespace SobelAlgImage.Services
{
    public interface IGeneralService
    {
        Task CreateImage(ImageModel img, IFormFileCollection files);
        Task<JsonMessageModel> DeleteImage(int id);

        Bitmap ConvertImageWithTasks(Bitmap sourceOriginal, int tiles, int algorithmChooser, int greyScale);
    }
}
