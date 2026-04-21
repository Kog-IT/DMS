using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMS.Governorates;

namespace DMS.EntityFrameworkCore.Seed.Host
{
    public class DefaultGovernorateCreator
    {
        private readonly DMSDbContext _context;

        public DefaultGovernorateCreator(DMSDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateGovernorates();
        }

        private void CreateGovernorates()
        {
            
            if (!_context.Governorates.Any())
            {
                _context.Governorates.Add(new Governorate { Name = "القاهرة", Name_EN = "Cairo", GovernorateCode = "CAI", IsActive = true });
                _context.Governorates.Add(new Governorate { Name = "الإسكندرية", Name_EN = "Alexandria", GovernorateCode = "ALX", IsActive = true });
                _context.Governorates.Add(new Governorate { Name = "الجيزة", Name_EN = "Giza", GovernorateCode = "GIZ", IsActive = true });

                _context.SaveChanges();
            }
        }
    }
}
