﻿namespace SobelAlgImage.Models.DataModels
{
    public class ImageModel
    {
        private const int DefaultTiles = 4;

        public int Id { get; set; }
        public string Title { get; set; } = null;
        public string SourceOriginal { get; set; }
        public string SourceGrey50 { get; set; } = null;
        public string SourceGrey80 { get; set; } = null;
        public string SourceGrey100 { get; set; } = null;
        public string SourcConvolutionTasks { get; set; } = null;
        public int? AmountOfThreads { get; set; }

        public int GetCorrectTilesForProcessing()
        {
            int tiles = this.AmountOfThreads ?? DefaultTiles;

            return tiles > 8 ? DefaultTiles : tiles;
        }
    }
}
