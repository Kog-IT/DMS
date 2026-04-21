using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.Governorates.Dto
{
    public class CreateGovernorateDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Name_EN { get; set; }
        [Required]
        public string GovernorateCode { get; set; }
        public bool IsActive { get; set; }
    }
}
