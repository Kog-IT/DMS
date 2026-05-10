using Abp.Application.Services.Dto;

namespace DMS.Products.Dto
{
    public class PagedProductResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public string Code { get; set; }
        public int? CategoryId { get; set; }
        public int? ProductGroupId { get; set; }
        public int? BrandId { get; set; }
        public int? ProductStatus { get; set; }
        public int? Grade { get; set; }
        public int? Unit { get; set; }
        public bool? IsActive { get; set; }
    }
}
