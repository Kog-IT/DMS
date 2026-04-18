using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Timing;
using DMS.Customers.Dto;
using DMS.Visits;
using Microsoft.EntityFrameworkCore;

namespace DMS.Customers;

public class ClassifyCustomersAppService : DMSAppServiceBase, IClassifyCustomersAppService
{
    private readonly IRepository<Customer, int> _customerRepository;
    private readonly IRepository<Visit, int> _visitRepository;
    private readonly CustomerClassificationService _classificationService;
    private readonly ISettingManager _settingManager;

    public ClassifyCustomersAppService(
        IRepository<Customer, int> customerRepository,
        IRepository<Visit, int> visitRepository,
        CustomerClassificationService classificationService,
        ISettingManager settingManager)
    {
        _customerRepository = customerRepository;
        _visitRepository = visitRepository;
        _classificationService = classificationService;
        _settingManager = settingManager;
    }

    public async Task<List<ClassificationResultDto>> ClassifyAllAsync()
    {
        var lookbackDays = await _settingManager.GetSettingValueAsync<int>(ClassificationSettingNames.LookbackDays);
        var thresholdA = await _settingManager.GetSettingValueAsync<int>(ClassificationSettingNames.ThresholdA);
        var thresholdB = await _settingManager.GetSettingValueAsync<int>(ClassificationSettingNames.ThresholdB);
        var minForPercentile = await _settingManager.GetSettingValueAsync<int>(ClassificationSettingNames.MinCustomersForPercentile);

        var cutoff = Clock.Now.AddDays(-lookbackDays);

        var customers = await _customerRepository.GetAllListAsync(c => c.IsActive);
        if (!customers.Any())
            return new List<ClassificationResultDto>();

        var customerIds = customers.Select(c => c.Id).ToList();

        var visitCounts = await _visitRepository.GetAll()
            .Where(v => customerIds.Contains(v.CustomerId)
                        && v.Status == VisitStatus.Completed
                        && v.CheckInTime >= cutoff)
            .GroupBy(v => v.CustomerId)
            .Select(g => new { CustomerId = g.Key, Count = g.Count() })
            .ToListAsync();

        var visitCountMap = visitCounts.ToDictionary(v => v.CustomerId, v => v.Count);

        var input = customers
            .Select(c => (c.Id, visitCountMap.TryGetValue(c.Id, out var cnt) ? cnt : 0))
            .ToList();

        var classificationResults = _classificationService.Classify(input, thresholdA, thresholdB, minForPercentile);
        var classificationMap = classificationResults.ToDictionary(r => r.CustomerId, r => r.Classification);

        var now = Clock.Now;
        var resultDtos = new List<ClassificationResultDto>();

        foreach (var customer in customers)
        {
            var oldClass = customer.Classification;
            var newClass = classificationMap.TryGetValue(customer.Id, out var cls)
                ? cls
                : CustomerClassification.C;

            customer.Classification = newClass;
            customer.LastClassifiedAt = now;

            resultDtos.Add(new ClassificationResultDto
            {
                CustomerId = customer.Id,
                CustomerName = customer.Name,
                OldClassification = oldClass,
                NewClassification = newClass,
                VisitCount = visitCountMap.TryGetValue(customer.Id, out var vc) ? vc : 0,
            });
        }

        await CurrentUnitOfWork.SaveChangesAsync();
        return resultDtos;
    }
}
