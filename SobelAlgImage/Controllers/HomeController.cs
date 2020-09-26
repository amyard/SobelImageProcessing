using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SobelAlgImage.Helpers;
using SobelAlgImage.Interfaces;
using SobelAlgImage.Models;

namespace SobelAlgImage.Controllers
{
    public class HomeController : Controller
    {
        #region constructor
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileManager _fileManager;     // for upload images on server

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, IFileManager fileManager)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _fileManager = fileManager;
        }

        #endregion

        public async Task<IActionResult> Index()
        {
            ImageModelVM vm = new ImageModelVM()
            {
                ImgModel = new ImageModel(),
                ImgModels = await _unitOfWork.ImageSobelAlg.GetListOfImagesAsync()
            };
            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> CreateImage(ImageModel img)
        {
            var files = HttpContext.Request.Form.Files;

            ImageModel collectedImgData = await CollectImageData(img, files);
            
            await _unitOfWork.ImageSobelAlg.CreateImageAsync(img);
            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var img = await _unitOfWork.ImageSobelAlg.GetImageByIdAsync(id);

            if (img == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            // delete images
            _fileManager.RemoveImage(img.SourceOriginal);
            _fileManager.RemoveImage(img.SourceTransformSlower);
            _fileManager.RemoveImage(img.SourceTransformFaster);


            await _unitOfWork.ImageSobelAlg.DeleteImageAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return Json(new { success = true, message = "Delete Successful" });
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region private methods
        private async Task<ImageModel> CollectImageData(ImageModel img, IFormFileCollection files)
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


            //// split one bitmap by Y on many small bitmaps
            //IEnumerable<Bitmap> collectedBitmaps = _fileManager.SplitBitmapsOnManyBitmaps(imgProcessFaster);

            //// есть склейка между картинками. imgProcessSlower - возвращает картинку с какими - то белыми краями по X
            //Bitmap resultBitmap = _fileManager.MergeBitmapsInOne(collectedBitmaps);
            //_fileManager.BitmapSaveTest(resultBitmap);

            return img;
        }

        #endregion
    }
}
