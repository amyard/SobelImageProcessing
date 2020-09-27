using Microsoft.AspNetCore.Http;
using SobelAlgImage.Helpers;
using SobelAlgImage.Interfaces;
using SobelAlgImage.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
            int tiles = img.AmountOfThreads ?? 4;

            img.AmountOfThreads = tiles;

            img.Title = fileName;
            img.AmountOfThreads = img.AmountOfThreads ?? HelperConstants.AmountOfProcesses;
            img.SourceOriginal = await _fileManager.SaveImage(files, HelperConstants.OriginalImageBasePath, HelperConstants.OriginalImageResultPath, fileName);

            string fullPath = _fileManager.ImageFullPath(img.SourceOriginal);
            Bitmap imageSource = (Bitmap)Image.FromFile(fullPath);

            Bitmap imgProcessSlower = SobelAlgorithm.SobelProcessStart(imageSource, 1);
            Bitmap imgProcessFaster = SobelAlgorithm.SobelProcessStart(imageSource, 2);

            Bitmap convertedBitmapSlower = ConvertImageWithTasks(imageSource, tiles, 1);
            Bitmap convertedBitmapFaster = ConvertImageWithTasks(imgProcessFaster, tiles, 2);

            img.SourceTransformSlower = _fileManager.SaveBitMapToImage(imgProcessSlower, HelperConstants.TransformImageResultPath, fileName + "_slower");
            img.SourceTransformFaster = _fileManager.SaveBitMapToImage(imgProcessFaster, HelperConstants.TransformImageResultPath, fileName + "_faster");
            img.SourceTransformTaskSlower = _fileManager.SaveBitMapToImage(convertedBitmapSlower, HelperConstants.TransformImageResultPath, fileName + "_slower_tasks");
            img.SourceTransformTaskFaster = _fileManager.SaveBitMapToImage(convertedBitmapFaster, HelperConstants.TransformImageResultPath, fileName + "_faster_tasks");

            await _imageAlgorithm.CreateImageAsync(img);
            await _imageAlgorithm.SaveChangesAsync();


        }

        public Bitmap ConvertImageWithTasks(Bitmap sourceOriginal, int tiles, int algorithmChooser)
        {
            // split one bitmap by Y on many small bitmaps
            IEnumerable<Bitmap> collectedBitmaps = _fileManager.SplitBitmapsOnManyBitmaps(sourceOriginal, tiles);

            // create our tasks
            var tasks = new List<Task>();
            List<Bitmap> resultedListOfBitmaps = new Bitmap[tiles].ToList();

            foreach (var i in Enumerable.Range(0, tiles))
                tasks.Add(new Task(() => SobelAlgorithm.SobelProcessTaskChooser(collectedBitmaps.ToList()[i], algorithmChooser, i, resultedListOfBitmaps)));

            foreach (var t in tasks)
                t.Start();

            Task.WaitAll(tasks.ToArray());

            // есть склейка между картинками. imgProcessSlower - возвращает картинку с какими - то белыми краями по X
            Bitmap resultBitmap = _fileManager.MergeBitmapsInOne(resultedListOfBitmaps);

            // faster отличается с тасками от нормального
            //_fileManager.BitmapSaveTest(resultBitmap);

            return resultBitmap;
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
            _fileManager.RemoveImage(img.SourceTransformTaskSlower);
            _fileManager.RemoveImage(img.SourceTransformTaskFaster);


            await _imageAlgorithm.DeleteImageAsync(id);
            await _imageAlgorithm.SaveChangesAsync();

            return new JsonMessageModel { Success = true, Message = "Delete Successful" };
        }
    }
}
