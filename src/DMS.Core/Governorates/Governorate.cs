using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities.Auditing;
using DMS.Cities;

namespace DMS.Governorates
{
    public class Governorate : FullAuditedEntity<int>
    {
        public string Name { get; set; }
        public string Name_EN { get; set; }
        public string GovernorateCode { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<City> Cities { get; set; }

        public Governorate()
        {
            Cities = new Collection<City>();
            IsActive = true;
        }
    }
}
