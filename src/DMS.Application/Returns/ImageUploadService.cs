using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;

namespace DMS.Returns
{

    public class ImageUploadService : IImageUploadService, ITransientDependency
    {
        public async Task<string> UploadAsync(string base64Image)
        {
            if (string.IsNullOrEmpty(base64Image)) return null;

            
            var folderPath = Path.Combine("wwwroot", "uploads", "returns");

           
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

          
            var fileName = $"{Guid.NewGuid()}.jpg";
            var filePath = Path.Combine(folderPath, fileName);

          
            var base64Data = base64Image.Contains(",") ? base64Image.Split(',')[1] : base64Image;
            byte[] imageBytes = Convert.FromBase64String(base64Data);

            await File.WriteAllBytesAsync(filePath, imageBytes);

           
            return $"/uploads/returns/{fileName}";
        }
    }
}
