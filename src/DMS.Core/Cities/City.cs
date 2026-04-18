using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities.Auditing;
using DMS.Governorates;

namespace DMS.Cities
{
    public class City : FullAuditedEntity<int>
    {
        public string Name { get; set; }
        public string Name_EN { get; set; }
        public bool IsActive { get; set; }

        public int GovernorateId { get; set; }
        [ForeignKey("GovernorateId")]
        public virtual Governorate Governorate { get; set; }

        public City()
        {
            IsActive = true;
        }
    }
}
