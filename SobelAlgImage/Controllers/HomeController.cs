using System;
using System.Diagnostics;
using System.Drawing;
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

            // delete images and comments -> post
            _fileManager.RemoveImage(img.SourceOriginal);
            _fileManager.RemoveImage(img.SourceTransform);


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

            Bitmap imgProcess = SobelAlgorithm.SobelProcessStart(_fileManager.ImageFullPath(img.SourceOriginal));
            img.SourceTransform = _fileManager.SaveBitMapToImage(imgProcess, HelperConstants.TransformImageResultPath, fileName);

            imgProcess.Dispose();

            return img;
        }

        #endregion
    }
}
