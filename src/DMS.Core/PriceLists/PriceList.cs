using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using DMS.Customers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace DMS.PriceLists;

public class PriceList : FullAuditedEntity<int>, IMustHaveTenant
{
    public const int MaxNameLength = 128;
    public const int MaxDescriptionLength = 512;

    public int TenantId { get; set; }

    [Required]
    [StringLength(MaxNameLength)]
    public string Name { get; set; }

    [StringLength(MaxDescriptionLength)]
    public string Description { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public CustomerClassification? ForClassification { get; set; }

    public virtual ICollection<PriceListItem> Items { get; set; } = new Collection<PriceListItem>();
    public virtual ICollection<PriceListAssignment> Assignments { get; set; } = new Collection<PriceListAssignment>();
}
