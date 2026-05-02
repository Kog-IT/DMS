using Abp.Domain.Repositories;
using ClosedXML.Excel;
using DMS.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;

namespace DMS.Controllers
{
    [Route("ExportProductsToExcelEndpoint")]
    [ApiController]
    [Authorize]
    public class ExportProductsController : DMSControllerBase
    {
        private readonly IRepository<Product, int> _productRepository;

        public ExportProductsController(IRepository<Product, int> productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet("AllProducts")]
        public IActionResult AllProducts(
            [FromQuery] string keyword = null,
            [FromQuery] string code = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] int? brandId = null,
            [FromQuery] int? productGroupId = null,
            [FromQuery] int? productStatus = null,
            [FromQuery] int? grade = null,
            [FromQuery] int? unit = null,
            [FromQuery] bool? isActive = null)
        {
            var query = _productRepository.GetAll()
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.ProductGroup)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(p => p.Name.Contains(keyword) || p.Description.Contains(keyword));
            if (!string.IsNullOrWhiteSpace(code))
                query = query.Where(p => p.Code.Contains(code));
            if (categoryId.HasValue)    query = query.Where(p => p.CategoryId == categoryId.Value);
            if (brandId.HasValue)       query = query.Where(p => p.BrandId == brandId.Value);
            if (productGroupId.HasValue) query = query.Where(p => p.ProductGroupId == productGroupId.Value);
            if (productStatus.HasValue) query = query.Where(p => p.ProductStatus == productStatus.Value);
            if (grade.HasValue)         query = query.Where(p => p.Grade == grade.Value);
            if (unit.HasValue)          query = query.Where(p => p.Unit == unit.Value);
            if (isActive.HasValue)      query = query.Where(p => p.IsActive == isActive.Value);

            var products = query.ToList();

            var headers = new[]
            {
                "Code", "Name", "Category", "Brand", "Product Group",
                "Unit", "Grade", "Product Status", "Pack Size",
                "Wholesale Price", "Retail Price", "VIP Price",
                "Safety Stocks", "Net Weight/KG", "Is Active"
            };

            byte[] fileBytes;

            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Products");

                for (int i = 0; i < headers.Length; i++)
                {
                    ws.Cell(1, i + 1).Value = headers[i];
                    ws.Cell(1, i + 1).Style.Font.Bold = true;
                    ws.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#6B21A8");
                    ws.Cell(1, i + 1).Style.Font.FontColor = XLColor.White;
                }

                int row = 2;
                foreach (var p in products)
                {
                    ws.Cell(row, 1).Value  = p.Code ?? "";
                    ws.Cell(row, 2).Value  = p.Name ?? "";
                    ws.Cell(row, 3).Value  = p.Category?.Name ?? "";
                    ws.Cell(row, 4).Value  = p.Brand?.Name ?? "";
                    ws.Cell(row, 5).Value  = p.ProductGroup?.Name ?? "";
                    ws.Cell(row, 6).Value  = p.Unit;
                    ws.Cell(row, 7).Value  = p.Grade;
                    ws.Cell(row, 8).Value  = p.ProductStatus;
                    ws.Cell(row, 9).Value  = (double)p.PackSize;
                    ws.Cell(row, 10).Value = (double)p.WholesalePrice;
                    ws.Cell(row, 11).Value = (double)p.RetailPrice;
                    ws.Cell(row, 12).Value = (double)p.VipClientsPrice;
                    ws.Cell(row, 13).Value = p.SafetyStocks;
                    ws.Cell(row, 14).Value = (double)p.NetWeightPerKG;
                    ws.Cell(row, 15).Value = p.IsActive;
                    row++;
                }

                ws.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    fileBytes = stream.ToArray();
                }
            }

            return File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "products.xlsx"
            );
        }
    }
}
