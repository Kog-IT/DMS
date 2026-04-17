using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Dependency;

namespace DMS.Customers;

public class CustomerClassificationResult
{
    public int CustomerId { get; set; }
    public CustomerClassification Classification { get; set; }
}

public class CustomerClassificationService : ITransientDependency
{
    public List<CustomerClassificationResult> Classify(
        IEnumerable<(int CustomerId, int VisitCount)> customers,
        int thresholdA,
        int thresholdB,
        int minCustomersForPercentile)
    {
        var list = customers.ToList();
        if (!list.Any())
            return new List<CustomerClassificationResult>();

        if (thresholdA < thresholdB)
            (thresholdA, thresholdB) = (thresholdB, thresholdA);

        if (list.Count >= minCustomersForPercentile)
            return ClassifyByThreshold(list, thresholdA, thresholdB);
        else
            return ClassifyByPercentile(list);
    }

    private static List<CustomerClassificationResult> ClassifyByThreshold(
        List<(int CustomerId, int VisitCount)> customers,
        int thresholdA,
        int thresholdB)
    {
        return customers.Select(c =>
        {
            var cls = c.VisitCount >= thresholdA ? CustomerClassification.A
                    : c.VisitCount >= thresholdB ? CustomerClassification.B
                    : CustomerClassification.C;
            return new CustomerClassificationResult { CustomerId = c.CustomerId, Classification = cls };
        }).ToList();
    }

    private static List<CustomerClassificationResult> ClassifyByPercentile(
        List<(int CustomerId, int VisitCount)> customers)
    {
        var n = customers.Count;
        var aCount = Math.Max(1, (int)Math.Floor(n * 0.20));
        var bCount = Math.Max(1, (int)Math.Floor(n * 0.30));

        var sorted = customers.OrderByDescending(c => c.VisitCount).ToList();
        var results = new List<CustomerClassificationResult>();

        for (int i = 0; i < sorted.Count; i++)
        {
            var cls = i < aCount ? CustomerClassification.A
                    : i < aCount + bCount ? CustomerClassification.B
                    : CustomerClassification.C;
            results.Add(new CustomerClassificationResult
            {
                CustomerId = sorted[i].CustomerId,
                Classification = cls
            });
        }

        return results;
    }
}
