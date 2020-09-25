﻿using Microsoft.EntityFrameworkCore;
using SobelAlgImage.Data;
using SobelAlgImage.Interfaces;
using SobelAlgImage.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SobelAlgImage.Repository
{
    public class ImageAlgorithmRepo : IImageAlgorithmRepo
    {
        private readonly StoreContext _context;

        public ImageAlgorithmRepo(StoreContext context)
        {
            _context = context;
        }

        public async Task<ImageModel> GetImageByIdAsync(int id)
        {
            return await _context.Set<ImageModel>().FindAsync(id);
        }


        public async Task<IReadOnlyList<ImageModel>> GetListOfImagesAsync()
        {
            return await _context.Set<ImageModel>().OrderByDescending(q => q.Id).ToListAsync();
        }


        public async Task CreateImageAsync(ImageModel img)
        {
            await _context.ImageModels.AddAsync(img);
        }

        public async Task DeleteImageAsync(int id)
        {
            var objData = await _context.Set<ImageModel>().FindAsync(id);
            _context.Remove(objData);
        }
    }
}
