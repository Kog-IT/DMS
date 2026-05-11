using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp;
using AutoMapper.Internal.Mappers;
using DMS.Returns.Dto;
using Abp.UI;

namespace DMS.Returns
{
    public class ReturnAppService : DMSAppServiceBase, IReturnAppService
    {
        private readonly IRepository<Return, Guid> _returnRepository;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IImageUploadService _imageUploadService;


        public ReturnAppService(
            IRepository<Return, Guid> returnRepository,
            IGuidGenerator guidGenerator,
            IImageUploadService imageUploadService)
        {
            _returnRepository = returnRepository;
            _guidGenerator = guidGenerator;
            _imageUploadService = imageUploadService;
        }

        public async Task<ReturnDto> CreateAsync(CreateReturnDto input)
        {
            if (input.Lines == null || input.Lines.Count == 0)
            {
                throw new UserFriendlyException("لا يمكن عمل مرتجع بدون اختيار منتجات.");
            }

            var returnId = _guidGenerator.Create();
            var returnNumber = $"RET-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString()[..5].ToUpper()}";

            var @return = new Return(
                returnId,
                input.OrderId,
                returnNumber,
                input.Reason
            );

            
            foreach (var line in input.Lines)
            {
                @return.Lines.Add(new ReturnLine
                {
                    Id = _guidGenerator.Create(),
                    ProductId = line.ProductId,
                    Quantity = line.Quantity
                });
            }

          
            if (input.PhotosBase64 != null && input.PhotosBase64.Any())
            {
                foreach (var base64 in input.PhotosBase64)
                {
                   
                    var imageUrl = await _imageUploadService.UploadAsync(base64);

                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        @return.Photos.Add(new ReturnPhoto
                        {
                            Id = _guidGenerator.Create(),
                            ReturnId = returnId,
                            Url = imageUrl
                        });
                    }
                }
            }

            await _returnRepository.InsertAsync(@return);
            return ObjectMapper.Map<ReturnDto>(@return);
        }
    }
}
