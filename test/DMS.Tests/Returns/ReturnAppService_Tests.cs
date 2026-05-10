using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMS.Returns.Dto;
using DMS.Returns.Enums;
using DMS.Returns;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace DMS.Tests.Returns
{
    public class Return_Tests : DMSTestBase
    {
        private readonly IReturnAppService _returnService;

        public Return_Tests()
        {
            _returnService = Resolve<IReturnAppService>();
        }

        [Fact]
        public async Task Create_Return_Succeeds_And_Sets_Pending_Status()
        {
            LoginAsDefaultTenantAdmin();

            // Arrange: افتراض وجود OrderId سابق
            var orderId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            // Act
            var result = await _returnService.CreateAsync(new CreateReturnDto
            {
                OrderId = orderId,
                Reason = "Item damaged upon arrival",
                Lines = new List<CreateReturnLineDto>
            {
                new CreateReturnLineDto { ProductId = productId, Quantity = 1 }
            },
                PhotosBase64 = new List<string> { "base64_string_test_data" }
            });

            // Assert
            result.ShouldNotBeNull();
            result.ReturnNumber.ShouldStartWith("RET-");
            result.Status.ShouldBe(ReturnStatus.Pending);
        }

        [Fact]
        public async Task Create_Return_With_Empty_Lines_Should_Throw()
        {
            LoginAsDefaultTenantAdmin();

            await Should.ThrowAsync<Exception>(async () =>
                await _returnService.CreateAsync(new CreateReturnDto
                {
                    OrderId = Guid.NewGuid(),
                    Reason = "Invalid return",
                    Lines = new List<CreateReturnLineDto>() // فارغة
                })
            );
        }

        [Fact]
        public async Task Create_Return_And_Verify_Photo_Attachment()
        {
            LoginAsDefaultTenantAdmin();

           
            var validBase64Image = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNk+A8AAQUBAScY42YAAAAASUVORK5CYII=";

            // Act
            var result = await _returnService.CreateAsync(new CreateReturnDto
            {
                OrderId = Guid.NewGuid(),
                Reason = "Wrong item received",
                Lines = new List<CreateReturnLineDto>
        {
            new CreateReturnLineDto { ProductId = Guid.NewGuid(), Quantity = 1 }
        },
                PhotosBase64 = new List<string> { validBase64Image, validBase64Image }
            });

            // Assert
            result.ShouldNotBeNull();

            var savedReturn = await UsingDbContextAsync(async ctx =>
                await ctx.Set<DMS.Returns.Return>()
                         .Include(r => r.Photos)
                         .FirstOrDefaultAsync(r => r.Id == result.Id));

            savedReturn.ShouldNotBeNull();
            savedReturn.Photos.Count.ShouldBe(2);
        }
    }
}