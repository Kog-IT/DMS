using Abp.Application.Services.Dto;
using DMS.Media;
using System.Collections.Generic;

namespace DMS.Application.Media.Dto;

public class MediaFileDto : EntityDto<int>
{
    public MediaType MediaType { get; set; }
    public string MediaTypeName { get; set; }
    public int ModelId { get; set; }
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public List<string> Path { get; set; }
    public string ContentType { get; set; }
    public long FileSizeBytes { get; set; }
}
