using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.Returns.Dto
{
    public class CreateReturnDto
    {
        [Required]
        public Guid OrderId { get; set; }
        public string Reason { get; set; }
        public List<CreateReturnLineDto> Lines { get; set; }
        public List<string> PhotosBase64 { get; set; } 
    }
}
