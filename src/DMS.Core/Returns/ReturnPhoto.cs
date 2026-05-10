using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities;

namespace DMS.Returns
{
    public class ReturnPhoto : Entity<Guid>
    {
        public Guid ReturnId { get; set; }
        public string Url { get; set; } 
    }
}
