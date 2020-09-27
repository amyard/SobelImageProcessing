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
        Bitmap grey50, grey80, grey100, convolutionTasks;

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

            if (tiles == 1)
            {
                grey50 = SobelAlgorithm.SobelProcessStart(imageSource, 1, 50);
                grey80 = SobelAlgorithm.SobelProcessStart(imageSource, 1, 80);
                grey100 = SobelAlgorithm.SobelProcessStart(imageSource, 1, 100);
                convolutionTasks = SobelAlgorithm.SobelProcessStart(imageSource, 2, 0);
            }
            else
            { 
                grey50 = ConvertImageWithTasks(imageSource, tiles, 1, 50);
                grey80 = ConvertImageWithTasks(imageSource, tiles, 1, 80);
                grey100 = ConvertImageWithTasks(imageSource, tiles, 1, 100);
                convolutionTasks = ConvertImageWithTasks(imageSource, tiles, 2, 0);
            }

            img.SourceGrey50 = _fileManager.SaveBitMapToImage(grey50, HelperConstants.TransformImageResultPath, fileName + "_grey50");
            img.SourceGrey80 = _fileManager.SaveBitMapToImage(grey80, HelperConstants.TransformImageResultPath, fileName + "_grey80");
            img.SourceGrey100 = _fileManager.SaveBitMapToImage(grey100, HelperConstants.TransformImageResultPath, fileName + "_grey100");
            img.SourcConvolutionTasks = _fileManager.SaveBitMapToImage(convolutionTasks, HelperConstants.TransformImageResultPath, fileName + "_convTasks");

            await _imageAlgorithm.CreateImageAsync(img);
            await _imageAlgorithm.SaveChangesAsync();

            grey50.Dispose();
            grey80.Dispose();
            grey100.Dispose();
            convolutionTasks.Dispose();
 
        }

        public Bitmap ConvertImageWithTasks(Bitmap sourceOriginal, int tiles, int algorithmChooser, int greyScale)
        {
            // split one bitmap by Y on many small bitmaps
            IEnumerable<Bitmap> collectedBitmaps = _fileManager.SplitBitmapsOnManyBitmaps(sourceOriginal, tiles);

            // create our tasks
            var tasks = new List<Task>();
            List<Bitmap> resultedListOfBitmaps = new Bitmap[tiles].ToList();

            foreach (var i in Enumerable.Range(0, tiles))
                tasks.Add(new Task(() => SobelAlgorithm.SobelProcessTaskChooser(collectedBitmaps.ToList()[i], algorithmChooser, i, resultedListOfBitmaps, greyScale)));

            foreach (var t in tasks)
                t.Start();

            Task.WaitAll(tasks.ToArray());

            // есть склейка между картинками. imgProcessSlower - возвращает картинку с какими - то белыми краями по X
            Bitmap resultBitmap = _fileManager.MergeBitmapsInOne(resultedListOfBitmaps, algorithmChooser);

            //_fileManager.BitmapSaveTest(resultBitmap);

            return resultBitmap;
        }

        public async Task<JsonMessageModel> DeleteImage(int id)
        {
            var img = await _imageAlgorithm.GetImageByIdAsync(id);

            if (img == null)
                return new JsonMessageModel { Success = false, Message = "Error while deleting" };

            // delete images
            _fileManager.RemoveImage(img.SourceOriginal);
            _fileManager.RemoveImage(img.SourceGrey50);
            _fileManager.RemoveImage(img.SourceGrey80);
            _fileManager.RemoveImage(img.SourceGrey100);
            _fileManager.RemoveImage(img.SourcConvolutionTasks);

            await _imageAlgorithm.DeleteImageAsync(id);
            await _imageAlgorithm.SaveChangesAsync();

            return new JsonMessageModel { Success = true, Message = "Delete Successful" };
        }
    }
}
