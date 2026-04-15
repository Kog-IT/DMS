using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using DMS.Products;

namespace DMS.Categories
{
    public class Category : FullAuditedEntity<int>, IMustHaveTenant
    {
        public const int MaxNameLength = 128;
        public string Name { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new Collection<Product>();
        public int TenantId { get; set; }

    }
}
