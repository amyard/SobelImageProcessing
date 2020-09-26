using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SobelAlgImage.Interfaces;
using SobelAlgImage.Models;
using SobelAlgImage.Services;

namespace SobelAlgImage.Controllers
{
    public class HomeController : Controller
    {
        #region constructor
        private readonly ILogger<HomeController> _logger;
        private readonly IGeneralService _service;
        private readonly IImageAlgorithmRepo _imageRepo;

        public HomeController(ILogger<HomeController> logger, IGeneralService service, IImageAlgorithmRepo imageRepo)
        {
            _logger = logger;
            _service = service;
            _imageRepo = imageRepo;
        }

        #endregion

        public async Task<IActionResult> Index()
        {
            ImageModelVM vm = new ImageModelVM()
            {
                ImgModel = new ImageModel(),
                ImgModels = await _imageRepo.GetListOfImagesAsync()
            };
            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> CreateImage(ImageModel img)
        {
            var files = HttpContext.Request.Form.Files;
            await _service.CreateImage(img, files);

            return RedirectToAction(nameof(Index));
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteImage(int id)
        {
            return Json(await _service.DeleteImage(id));
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
