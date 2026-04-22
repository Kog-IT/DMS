using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.Cities.Dto
{
    public class CreateCityDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Name_EN { get; set; }
        public bool IsActive { get; set; }
        [Required]
        public int GovernorateId { get; set; }
    }
}
