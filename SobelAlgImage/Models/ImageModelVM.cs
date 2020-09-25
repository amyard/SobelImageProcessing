using System.Collections.Generic;

namespace SobelAlgImage.Models
{
    public class ImageModelVM
    {
        public ImageModel ImgModel { get; set; }
        public IReadOnlyList<ImageModel> ImgModels { get; set; }
    }
}
