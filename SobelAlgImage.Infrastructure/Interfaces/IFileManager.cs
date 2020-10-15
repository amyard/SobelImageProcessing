using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace SobelAlgImage.Infrastructure.Interfaces
{
    public interface IFileManager
    {
        Task<string> SaveImageAsync(IFormFileCollection files, string imageBasePath, string imageResultPath, string filename);
        string SaveBitMapToImage(Bitmap bitMap, string imageBasePath, string filename);
        bool RemoveImage(string filePath);
        string ImageFullPath(string imgPath);
        Bitmap MergeBitmapsInOne(IEnumerable<Bitmap> images, int algorithmChooser);
        void BitmapSaveTest(Bitmap bitmap);
        IEnumerable<Bitmap> SplitBitmapsOnManyBitmaps(Bitmap bitmap, int tiles);
    }
}
