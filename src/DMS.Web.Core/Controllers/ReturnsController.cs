using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMS.Returns.Dto;
using DMS.Returns;
using Microsoft.AspNetCore.Mvc;

namespace DMS.Controllers
{
    [Route("api/returns")]
    public class ReturnsController : DMSControllerBase
    {
        private readonly IReturnAppService _returnAppService;

        public ReturnsController(IReturnAppService returnAppService)
        {
            _returnAppService = returnAppService;
        }

        [HttpPost]
        public virtual Task<ReturnDto> CreateAsync(CreateReturnDto input)
        {
            return _returnAppService.CreateAsync(input);
        }
    }
}
