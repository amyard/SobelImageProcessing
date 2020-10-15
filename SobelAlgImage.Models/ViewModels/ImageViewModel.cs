using SobelAlgImage.Models.DataModels;
using System.Collections.Generic;

namespace SobelAlgImage.Models.ViewModels
{
    public class ImageViewModel
    {
        public ImageModel ImgModel { get; set; }
        public IReadOnlyList<ImageModel> ImgModels { get; set; }
    }
}
