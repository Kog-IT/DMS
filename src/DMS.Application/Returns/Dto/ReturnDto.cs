using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using DMS.Returns.Enums;

namespace DMS.Returns.Dto
{
    public class ReturnDto : EntityDto<Guid>
    {
        public string ReturnNumber { get; set; }
        public Guid OrderId { get; set; }
        public ReturnStatus Status { get; set; }
        public string Reason { get; set; }
        public List<ReturnLineDto> Lines { get; set; }
    }
}
