using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace WorkshowcaseApi.Features.Uploads.DTOs;

public sealed class UploadWorkMediaRequest
{
    [Required]
    public IFormFile File { get; set; } = null!;
}
