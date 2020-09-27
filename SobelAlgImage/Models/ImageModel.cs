namespace SobelAlgImage.Models
{
    public class ImageModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null;
        public string SourceOriginal { get; set; }
        public string SourceTransformSlower { get; set; } = null;
        public string SourceTransformFaster { get; set; } = null;
        public string SourceTransformTaskSlower { get; set; } = null;
        public string SourceTransformTaskFaster { get; set; } = null;
        public int? AmountOfThreads { get; set; }
    }
}
