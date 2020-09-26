using Microsoft.AspNetCore.Http;
using SobelAlgImage.Helpers;
using SobelAlgImage.Interfaces;
using SobelAlgImage.Models;
using System;
using System.Drawing;
using System.Threading.Tasks;

namespace SobelAlgImage.Services
{
    public class GeneralService : IGeneralService
    {
        private readonly IFileManager _fileManager;
        private readonly IImageAlgorithmRepo _imageAlgorithm;

        public GeneralService(IFileManager fileManager, IImageAlgorithmRepo imageAlgorithm)
        {
            _imageAlgorithm = imageAlgorithm;
            _fileManager = fileManager;
        }

        public async Task CreateImage(ImageModel img, IFormFileCollection files)
        {
            // generate new file name
            string fileName = Guid.NewGuid().ToString();

            img.Title = fileName;
            img.AmountOfThreads = img.AmountOfThreads ?? HelperConstants.AmountOfProcesses;
            img.SourceOriginal = await _fileManager.SaveImage(files, HelperConstants.OriginalImageBasePath, HelperConstants.OriginalImageResultPath, fileName);

            Bitmap imgProcessSlower = SobelAlgorithm.SobelProcessStart(_fileManager.ImageFullPath(img.SourceOriginal), 1);
            Bitmap imgProcessFaster = SobelAlgorithm.SobelProcessStart(_fileManager.ImageFullPath(img.SourceOriginal), 2);

            img.SourceTransformSlower = _fileManager.SaveBitMapToImage(imgProcessSlower, HelperConstants.TransformImageResultPath, fileName + "_slower");
            img.SourceTransformFaster = _fileManager.SaveBitMapToImage(imgProcessFaster, HelperConstants.TransformImageResultPath, fileName + "_faster");

            await _imageAlgorithm.CreateImageAsync(img);
            await _imageAlgorithm.SaveChangesAsync();

            //// split one bitmap by Y on many small bitmaps
            //IEnumerable<Bitmap> collectedBitmaps = _fileManager.SplitBitmapsOnManyBitmaps(imgProcessFaster);

            //// есть склейка между картинками. imgProcessSlower - возвращает картинку с какими - то белыми краями по X
            //Bitmap resultBitmap = _fileManager.MergeBitmapsInOne(collectedBitmaps);
            //_fileManager.BitmapSaveTest(resultBitmap);
        }

        public async Task<JsonMessageModel> DeleteImage(int id)
        {
            var img = await _imageAlgorithm.GetImageByIdAsync(id);

            if (img == null)
            {
                return new JsonMessageModel { Success = false, Message = "Error while deleting" };
            }

            // delete images
            _fileManager.RemoveImage(img.SourceOriginal);
            _fileManager.RemoveImage(img.SourceTransformSlower);
            _fileManager.RemoveImage(img.SourceTransformFaster);


            await _imageAlgorithm.DeleteImageAsync(id);
            await _imageAlgorithm.SaveChangesAsync();

            return new JsonMessageModel { Success = true, Message = "Delete Successful" };
        }
    }
}
