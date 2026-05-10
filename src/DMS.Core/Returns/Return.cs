using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities.Auditing;
using DMS.Returns.Enums;

namespace DMS.Returns
{
    public class Return : FullAuditedEntity<Guid>
    {
        public string ReturnNumber { get; protected set; } // رقم المرتجع (R-2026-001)
        public Guid OrderId { get; protected set; } // الربط بالأوردر الأصلي
        public ReturnStatus Status { get; set; }
        public string Reason { get; set; }
        public ICollection<ReturnLine> Lines { get; set; }
        public ICollection<ReturnPhoto> Photos { get; set; }

        protected Return() { }

        public Return(Guid id, Guid orderId, string returnNumber, string reason) 
        {
            Id = id;
            OrderId = orderId;
            ReturnNumber = returnNumber;
            Reason = reason;
            Status = ReturnStatus.Pending;
            Lines = new List<ReturnLine>();
            Photos = new List<ReturnPhoto>();
        }
    }
}
