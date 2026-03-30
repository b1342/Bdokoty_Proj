using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace WorkshowcaseApi.Features.Uploads.DTOs.Requests;

public sealed class UploadRequest
{
    [Required]
    public IFormFile File { get; set; } = null!;
}
