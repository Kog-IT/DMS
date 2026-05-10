using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.Returns.Dto
{
    public class ReturnLineDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
