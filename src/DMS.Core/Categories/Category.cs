using System.Collections.ObjectModel;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using DMS.Products;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DMS.Categories
{
    public class Category : FullAuditedEntity<int>, IMustHaveTenant
    {
        public const int MaxNameLength = 128;

        [StringLength(MaxNameLength)]
        public string Name { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<Product> Products { get; set; } = new Collection<Product>();

        public int TenantId { get; set; }
    }
}
