using Abp.Modules;
using Abp.Reflection.Extensions;
using DMS.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace DMS.Web.Host.Startup
{
    [DependsOn(
       typeof(DMSWebCoreModule))]
    public class DMSWebHostModule : AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public DMSWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(DMSWebHostModule).GetAssembly());
        }
    }
}
