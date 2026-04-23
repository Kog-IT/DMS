using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.Products.Dto
{
    public class GetProductByBarcodeDto
    {
        [Required]
        public string Barcode { get; set; }
    }
}
