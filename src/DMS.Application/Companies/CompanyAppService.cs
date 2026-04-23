using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using DMS.Authorization;
using DMS.Common;
using DMS.Companies.Dto;
using System.Linq;

namespace DMS.Companies;

[AbpAuthorize(PermissionNames.Pages_Companies)]
public class CompanyAppService : DmsCrudAppService<
    Company,
    CompanyDto,
    int,
    PagedCompanyResultRequestDto,
    CreateCompanyDto,
    UpdateCompanyDto>, ICompanyAppService
{
    public CompanyAppService(IRepository<Company, int> repository)
        : base(repository)
    {
        GetPermissionName = PermissionNames.Pages_Companies;
        GetAllPermissionName = PermissionNames.Pages_Companies;
        CreatePermissionName = PermissionNames.Pages_Companies_Create;
        UpdatePermissionName = PermissionNames.Pages_Companies_Edit;
        DeletePermissionName = PermissionNames.Pages_Companies_Delete;
    }

    protected override IQueryable<Company> CreateFilteredQuery(PagedCompanyResultRequestDto input)
    {
        return Repository.GetAll()
            .WhereIf(
                !input.Keyword.IsNullOrWhiteSpace(),
                c => c.Name.Contains(input.Keyword)
            );
    }
}
