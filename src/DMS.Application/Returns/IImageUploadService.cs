using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;

namespace DMS.Returns
{
    public interface IImageUploadService : ITransientDependency
    {
        Task<string> UploadAsync(string base64Image);
    }
}
