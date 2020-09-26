using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace SobelAlgImage.Interfaces
{
    public interface IFileManager
    {
        Task<string> SaveImage(IFormFileCollection files, string imageBasePath, string imageResultPath, string filename);
        string SaveBitMapToImage(Bitmap bitMap, string imageBasePath, string filename);
        bool RemoveImage(string filePath);
        string ImageFullPath(string imgPath);
        Bitmap MergeBitmapsInOne(IEnumerable<Bitmap> images);
        void BitmapSaveTest(Bitmap bitmap);
    }
}
