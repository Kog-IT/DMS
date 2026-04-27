using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
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
            if (_context.Governorates.Any())
                return;

            var candidates = new[]
            {
                Path.Combine(Directory.GetCurrentDirectory(), "EntityFrameworkCore", "Seed", "Data", "governorates.json"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EntityFrameworkCore", "Seed", "Data", "governorates.json"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "governorates.json"),
            };

            var jsonPath = Array.Find(candidates, File.Exists);

            if (jsonPath != null)
            {
                var json = File.ReadAllText(jsonPath);
                var items = JsonSerializer.Deserialize<List<GovernorateJsonItem>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (items == null) return;

                foreach (var item in items)
                {
                    _context.Governorates.Add(new Governorate
                    {
                        Name = item.Governorate_Name_Ar,
                        Name_EN = item.Governorate_Name_En,
                        GovernorateCode = item.Governorate_Code,
                        IsActive = true
                    });
                }
            }
            else
            {
                // Fallback for test environments where JSON files are not present
                _context.Governorates.Add(new Governorate { Name = "القاهرة", Name_EN = "Cairo", GovernorateCode = "CAI", IsActive = true });
                _context.Governorates.Add(new Governorate { Name = "الإسكندرية", Name_EN = "Alexandria", GovernorateCode = "ALX", IsActive = true });
                _context.Governorates.Add(new Governorate { Name = "الجيزة", Name_EN = "Giza", GovernorateCode = "GIZ", IsActive = true });
            }

            _context.SaveChanges();
        }

        private class GovernorateJsonItem
        {
            public string Id { get; set; }
            public string Governorate_Name_Ar { get; set; }
            public string Governorate_Code { get; set; }
            public string Governorate_Name_En { get; set; }
        }
    }
}
