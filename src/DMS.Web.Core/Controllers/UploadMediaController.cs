using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using DMS.Application.Media.Dto;
using DMS.Media;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Controllers
{
    public class UploadMediaRequest
    {
        public IFormFile Files { get; set; }
        public MediaType MediaType { get; set; }
        public int ModelId { get; set; }
    }

    [ApiController]
    [Route("api/UploadMediaEndPoint")]
    [Authorize]
    public class UploadMediaController : DMSControllerBase
    {
        private readonly IRepository<MediaFile, int> _mediaRepository;
        private readonly IAbpSession _abpSession;

        private static readonly string[] AllowedExtensions =
            { "jpg", "jpeg", "png", "gif", "webp", "pdf", "mp4", "mov" };

        public UploadMediaController(
            IRepository<MediaFile, int> mediaRepository,
            IAbpSession abpSession)
        {
            _mediaRepository = mediaRepository;
            _abpSession = abpSession;
        }

        [HttpPost("UploadMedia")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(MediaFileDto), 200)]
        public async Task<IActionResult> UploadMedia([FromForm] UploadMediaRequest request)
        {
            var file = request.Files;
            if (file == null || file.Length == 0)
                throw new UserFriendlyException("No file provided.");

            var ext = Path.GetExtension(file.FileName)?.ToLowerInvariant().TrimStart('.');
            if (!AllowedExtensions.Contains(ext))
                throw new UserFriendlyException(
                    $"File type '{ext}' is not allowed. Allowed: {string.Join(", ", AllowedExtensions)}");

            var tenantId = _abpSession.TenantId ?? 0;
            var folder = Path.Combine("wwwroot", "uploads", "media",
                request.MediaType.ToString().ToLower(), tenantId.ToString(), request.ModelId.ToString());
            Directory.CreateDirectory(folder);

            var uniqueName = $"{Guid.NewGuid():N}.{ext}";
            var fullPath = Path.Combine(folder, uniqueName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = Path.Combine("uploads", "media",
                    request.MediaType.ToString().ToLower(), tenantId.ToString(),
                    request.ModelId.ToString(), uniqueName)
                .Replace('\\', '/');

            // Return the file path only — DB record is created when the entity is saved.
            return Ok(new MediaFileDto
            {
                Id = 0,
                MediaType = request.MediaType,
                MediaTypeName = request.MediaType.ToString(),
                ModelId = request.ModelId,
                FileName = file.FileName,
                FilePath = relativePath,
                Path = new List<string> { relativePath },
                ContentType = file.ContentType,
                FileSizeBytes = file.Length
            });
        }

        [HttpGet("GetMedia")]
        [ProducesResponseType(typeof(List<MediaFileDto>), 200)]
        public IActionResult GetMedia([FromQuery] MediaType mediaType, [FromQuery] int modelId)
        {
            var tenantId = _abpSession.TenantId ?? 0;
            var files = _mediaRepository.GetAll()
                .Where(m => m.TenantId == tenantId
                         && m.MediaType == mediaType
                         && m.ModelId == modelId)
                .OrderByDescending(m => m.CreationTime)
                .ToList()
                .Select(ToDto)
                .ToList();

            return Ok(files);
        }

        [HttpDelete("DeleteMedia/{id}")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> DeleteMedia(int id)
        {
            var tenantId = _abpSession.TenantId ?? 0;
            var media = await _mediaRepository.FirstOrDefaultAsync(
                m => m.Id == id && m.TenantId == tenantId);

            if (media == null)
                throw new UserFriendlyException("Media file not found.");

            var fullPath = Path.Combine("wwwroot", media.FilePath);
            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);

            await _mediaRepository.DeleteAsync(media);
            return Ok("Deleted successfully.");
        }

        private static MediaFileDto ToDto(MediaFile m)
        {
            return new MediaFileDto
            {
                Id = m.Id,
                MediaType = m.MediaType,
                MediaTypeName = m.MediaType.ToString(),
                ModelId = m.ModelId,
                FileName = m.FileName,
                FilePath = m.FilePath,
                Path = new List<string> { m.FilePath },
                ContentType = m.ContentType,
                FileSizeBytes = m.FileSizeBytes
            };
        }
    }
}
