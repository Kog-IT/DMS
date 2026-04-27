using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using DMS.Cities;

namespace DMS.EntityFrameworkCore.Seed.Host
{
    public class DefaultCityCreator
    {
        private readonly DMSDbContext _context;

        public DefaultCityCreator(DMSDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            if (_context.Cities.Any())
                return;

            var candidates = new[]
            {
                Path.Combine(Directory.GetCurrentDirectory(), "EntityFrameworkCore", "Seed", "Data", "cities.json"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EntityFrameworkCore", "Seed", "Data", "cities.json"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cities.json"),
            };

            var jsonPath = Array.Find(candidates, File.Exists);
            if (jsonPath == null) return; // No cities JSON found (e.g. test environment) — skip silently

            var json = File.ReadAllText(jsonPath);
            var items = JsonSerializer.Deserialize<List<CityJsonItem>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (items == null) return;

            // Build a lookup: JSON governorate id (string) -> DB governorate id (int)
            // Governorates were inserted in JSON order; sort by Id to match insertion order.
            var dbGovernorates = _context.Governorates
                .OrderBy(g => g.Id)
                .Select(g => g.Id)
                .ToList();

            // governorates.json ids are "1".."27" in order
            var govIdMap = new Dictionary<string, int>();
            for (int i = 0; i < dbGovernorates.Count; i++)
                govIdMap[(i + 1).ToString()] = dbGovernorates[i];

            foreach (var item in items)
            {
                if (!govIdMap.TryGetValue(item.Governorate_Id, out int dbGovId))
                    continue;

                _context.Cities.Add(new City
                {
                    Name = item.City_Name_Ar,
                    Name_EN = item.City_Name_En,
                    GovernorateId = dbGovId,
                    IsActive = true
                });
            }

            _context.SaveChanges();
        }

        private class CityJsonItem
        {
            public string Id { get; set; }
            public string Governorate_Id { get; set; }
            public string City_Name_Ar { get; set; }
            public string City_Name_En { get; set; }
        }
    }
}
