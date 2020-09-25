using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SobelAlgImage.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace SobelAlgImage.Repository
{
    public class FileManager : IFileManager
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        public FileManager(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }


        public bool RemoveImage(string filePath)
        {
            string webRootPath = _hostEnvironment.WebRootPath;

            if(!string.IsNullOrWhiteSpace(filePath))
            { 
                var imagePath = Path.Combine(webRootPath, filePath.TrimStart('\\'));

                if (System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagePath);
            }
            return true;
        }

        public async Task<string> SaveImage(IFormFileCollection files, string imageBasePath, string imageResultPath, string fileName)
        {
            string webRootPath = _hostEnvironment.WebRootPath;                  // get path to image folder
            var uploads = Path.Combine(webRootPath, imageBasePath);             // full path to save image
            var extension = Path.GetExtension(files[0].FileName);

            using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
            {
                await files[0].CopyToAsync(fileStreams);
            }
            return imageResultPath + fileName + extension;
        }
    }
}
