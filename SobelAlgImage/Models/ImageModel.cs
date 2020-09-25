namespace SobelAlgImage.Models
{
    public class ImageModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null;
        public string SourceOriginal { get; set; }
        public string SourceTransform { get; set; } = null;
        public int? AmountOfThreads { get; set; }
    }
}
