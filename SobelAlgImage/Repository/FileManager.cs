using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SobelAlgImage.Interfaces;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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

        public string ImageFullPath(string imgPath)
        {
            string webRootPath = _hostEnvironment.WebRootPath;

            return webRootPath + imgPath;
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

        public string SaveBitMapToImage(Bitmap bitMap, string imageBasePath, string filename)
        {
            string extension = ".jpg";
            string webRootPath = _hostEnvironment.WebRootPath;
            string rootPathToImage = imageBasePath + filename + extension;
            string fullPath = webRootPath + rootPathToImage;

            using (MemoryStream memory = new MemoryStream())
            {
                using (FileStream fs = new FileStream(fullPath, FileMode.Create, FileAccess.ReadWrite))
                {
                    bitMap.Save(memory, ImageFormat.Jpeg);
                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);

                    ((System.IDisposable)fs).Dispose();
                }

                ((System.IDisposable)memory).Dispose();
            }

            return rootPathToImage;
        }

        public async Task<string> SaveImage(IFormFileCollection files, string imageBasePath, string imageResultPath, string fileName)
        {
            string webRootPath = _hostEnvironment.WebRootPath;                  // get path to image folder
            var uploads = Path.Combine(webRootPath, imageBasePath);             // full path to save image
            var extension = Path.GetExtension(files[0].FileName);

            using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
            {
                // microsoft docs
                try
                {
                    await files[0].CopyToAsync(fileStreams);
                }
                finally
                {
                    if (fileStreams != null)
                        ((System.IDisposable)fileStreams).Dispose();
                }

                fileStreams.Close();
            }

            return imageResultPath + fileName + extension;
        }

        public Bitmap MergeBitmapsInOne(IEnumerable<Bitmap> images)
        {
            var enumerable = images as IList<Bitmap> ?? images.ToList();

            var width = 0;
            var height = 0;

            foreach (var image in enumerable)
            {
                width += image.Width;
                height = image.Height > height
                    ? image.Height
                    : height;
            }

            width = width - (images.Count()*2);

            var bitmap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bitmap))
            {
                var localWidth = 0;
                foreach (var image in enumerable)
                {
                    g.DrawImage(image, localWidth, 0);
                    localWidth += image.Width;

                    // remove white border between neighborn pictures
                    localWidth -= 2;
                }
            }
            return bitmap;
        }

        public IEnumerable<Bitmap> SplitBitmapsOnManyBitmaps(Bitmap bitmap, int tiles)
        {
            // slice bitmap on small parts by Y - easy to merge them  - WORKED!!!
            var width = bitmap.Width / tiles;
            var height = bitmap.Height;

            List<Bitmap> collectedBitmaps = new List<Bitmap>();

            foreach (var i in Enumerable.Range(0, tiles))
            {
                Bitmap newBitmap = new Bitmap(width, height);

                Rectangle cloneRect = new Rectangle(i * width, 0, width, height);
                PixelFormat format = bitmap.PixelFormat;
                Bitmap cloneBitmap = bitmap.Clone(cloneRect, format);

                using (var g = Graphics.FromImage(newBitmap))
                {
                    g.DrawImage(cloneBitmap, 0, 0);
                }

                collectedBitmaps.Add(newBitmap);
            }

            return collectedBitmaps;
        }




        public void BitmapSaveTest(Bitmap bitmap)
        {
            // Save the bitmap as a JPEG file with quality level 75.
            Encoder myEncoder = Encoder.Quality;
            ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 75L);
            myEncoderParameters.Param[0] = myEncoderParameter;

            bitmap.Save("BitmapTestSaving.jpg", myImageCodecInfo, myEncoderParameters);

            bitmap.Dispose();
        }



        #region private methods
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        #endregion
    }
}
