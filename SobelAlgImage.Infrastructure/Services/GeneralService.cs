using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SobelAlgImage.Helper;
using SobelAlgImage.Infrastructure.Interfaces;
using SobelAlgImage.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace SobelAlgImage.Infrastructure.Services
{
    public class GeneralService : IGeneralService
    {
        private readonly ILogger<GeneralService> _logger;
        private readonly IFileManager _fileManager;
        private readonly IImageAlgorithmRepo _imageAlgorithm;

        public GeneralService(ILogger<GeneralService> logger, IFileManager fileManager, IImageAlgorithmRepo imageAlgorithm)
        {
            _logger = logger;
            _imageAlgorithm = imageAlgorithm;
            _fileManager = fileManager;
        }


        public async Task CreateImageAsync(ImageModel img, IFormFileCollection files)
        {
            Bitmap grey50, grey80, grey100, convolutionTasks;

            // generate new file name
            string fileName = Guid.NewGuid().ToString();
            int tiles = img.GetCorrectTilesForProcessing();

            img.AmountOfThreads = tiles;

            img.Title = fileName;
            img.SourceOriginal = await _fileManager.SaveImageAsync(files, ProjectConstants.OriginalImageBasePath, ProjectConstants.OriginalImageResultPath, fileName);

            string fullPath = _fileManager.ImageFullPath(img.SourceOriginal);
            Bitmap imageSource = (Bitmap)Image.FromFile(fullPath);


            if (tiles == 1)
            {
                SobelAlgorithm imageProcessAlg = new SobelAlgorithm();

                grey50 = imageProcessAlg.SobelProcessStart(imageSource, 1, 50);
                grey80 = imageProcessAlg.SobelProcessStart(imageSource, 1, 80);
                grey100 = imageProcessAlg.SobelProcessStart(imageSource, 1, 100);
                convolutionTasks = imageProcessAlg.SobelProcessStart(imageSource, 2, 0);
            }
            else
            { 
                grey50 = ConvertImageWithTasks(imageSource, tiles, 1, 50);
                grey80 = ConvertImageWithTasks(imageSource, tiles, 1, 80);
                grey100 = ConvertImageWithTasks(imageSource, tiles, 1, 100);
                convolutionTasks = ConvertImageWithTasks(imageSource, tiles, 2, 0);
            }

            img.SourceGrey50 = _fileManager.SaveBitMapToImage(grey50, ProjectConstants.TransformImageResultPath, fileName + "_grey50");
            img.SourceGrey80 = _fileManager.SaveBitMapToImage(grey80, ProjectConstants.TransformImageResultPath, fileName + "_grey80");
            img.SourceGrey100 = _fileManager.SaveBitMapToImage(grey100, ProjectConstants.TransformImageResultPath, fileName + "_grey100");
            img.SourcConvolutionTasks = _fileManager.SaveBitMapToImage(convolutionTasks, ProjectConstants.TransformImageResultPath, fileName + "_convTasks");

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
            SobelAlgorithm imageProcessAlg = new SobelAlgorithm();

            foreach (var i in Enumerable.Range(0, tiles))
                tasks.Add(new Task(() =>
                    imageProcessAlg.SobelProcessTaskChooser(collectedBitmaps.ToList()[i], algorithmChooser, i, resultedListOfBitmaps, greyScale)));

            foreach (var t in tasks)
                t.Start();

            Task.WaitAll(tasks.ToArray());

            Bitmap resultBitmap = _fileManager.MergeBitmapsInOne(resultedListOfBitmaps, algorithmChooser);

            //_fileManager.BitmapSaveTest(resultBitmap);

            return resultBitmap;
        }

        public async Task<JsonMessageModel> DeleteImageAsync(int id)
        {
            try { 
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
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message, "Stopped deleting. Some error.");

                return new JsonMessageModel { Success = false, Message = "Error while deleting. Check loggs." };
            }

            return new JsonMessageModel { Success = true, Message = "Delete Successful" };
        }
    }
}
