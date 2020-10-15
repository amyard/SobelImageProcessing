using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SobelAlgImage.Infrastructure.Interfaces;
using SobelAlgImage.Models.DataModels;
using SobelAlgImage.Models.ViewModels;

namespace SobelAlgImage.Controllers
{
    public class ImageProcessController : Controller
    {
        #region constructor
        private readonly ILogger<ImageProcessController> _logger;
        private readonly IGeneralService _service;
        private readonly IImageAlgorithmRepo _imageRepo;

        public ImageProcessController(ILogger<ImageProcessController> logger, IGeneralService service, IImageAlgorithmRepo imageRepo)
        {
            _logger = logger;
            _service = service;
            _imageRepo = imageRepo;
        }

        #endregion

        public async Task<IActionResult> Index()
        {
            ImageViewModel vm = new ImageViewModel()
            {
                ImgModel = new ImageModel(),
                ImgModels = await _imageRepo.GetListOfImagesAsync()
            };
            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> CreateImage(ImageViewModel img)
        {
            var files = HttpContext.Request.Form.Files;
            await _service.CreateImageAsync(img.ImgModel, files);

            return RedirectToAction(nameof(Index));
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteImage(int id)
        {
            return Json(await _service.DeleteImageAsync(id));
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
