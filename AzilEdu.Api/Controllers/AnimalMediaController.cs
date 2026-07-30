using AzilEdu.Api.Data;
using AzilEdu.Shared.DTOs;
using AzilEdu.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzilEdu.Api.Controllers;

[ApiController]
[Route("api/animals/{animalId:int}/media")]
public class AnimalMediaController : ControllerBase
{
    private const long MaxFileSize = 25 * 1024 * 1024;

    private static readonly Dictionary<string, (AnimalMediaType Type, string Extension)>
        AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            ["image/jpeg"] = (AnimalMediaType.Image, ".jpg"),
            ["image/png"] = (AnimalMediaType.Image, ".png"),
            ["image/webp"] = (AnimalMediaType.Image, ".webp"),
            ["video/mp4"] = (AnimalMediaType.Video, ".mp4"),
            ["video/webm"] = (AnimalMediaType.Video, ".webm")
        };

    private readonly AzilEduDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public AnimalMediaController(
        AzilEduDbContext context,
        IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    [HttpGet]
    public async Task<ActionResult<List<AnimalMediaDto>>> GetMedia(int animalId)
    {
        var animalExists = await _context.Animals.AnyAsync(animal => animal.Id == animalId);

        if (!animalExists)
            return NotFound("Životinja nije pronađena.");

        var media = await _context.AnimalMedia
            .Where(item => item.AnimalId == animalId)
            .OrderByDescending(item => item.IsCover)
            .ThenBy(item => item.SortOrder)
            .ThenBy(item => item.Id)
            .ToListAsync();

        return Ok(media.Select(ToDto).ToList());
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(MaxFileSize)]
    public async Task<ActionResult<AnimalMediaDto>> Upload(
        int animalId,
        IFormFile file,
        [FromForm] string? caption)
    {
        var animalExists = await _context.Animals.AnyAsync(animal => animal.Id == animalId);

        if (!animalExists)
            return NotFound("Životinja nije pronađena.");

        if (file is null || file.Length == 0)
            return BadRequest("Odaberi datoteku.");

        if (file.Length > MaxFileSize)
            return BadRequest("Datoteka smije imati najviše 25 MB.");

        if (!AllowedContentTypes.TryGetValue(file.ContentType, out var allowedFile))
            return BadRequest("Dopušteni su JPG, PNG, WEBP, MP4 i WEBM formati.");

        var uploadDirectory = Path.Combine(
            _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot"),
            "uploads",
            "animals");

        Directory.CreateDirectory(uploadDirectory);

        var storedFileName = $"{Guid.NewGuid():N}{allowedFile.Extension}";
        var physicalPath = Path.Combine(uploadDirectory, storedFileName);

        await using (var stream = new FileStream(
            physicalPath,
            FileMode.CreateNew,
            FileAccess.Write))
        {
            await file.CopyToAsync(stream);
        }

        var imageCount = await _context.AnimalMedia.CountAsync(item =>
            item.AnimalId == animalId &&
            item.MediaType == AnimalMediaType.Image);

        var media = new AnimalMedia
        {
            AnimalId = animalId,
            StoredFileName = storedFileName,
            OriginalFileName = Path.GetFileName(file.FileName),
            ContentType = file.ContentType,
            MediaType = allowedFile.Type,
            FileSize = file.Length,
            Caption = string.IsNullOrWhiteSpace(caption) ? null : caption.Trim(),
            IsCover = allowedFile.Type == AnimalMediaType.Image && imageCount == 0,
            SortOrder = await _context.AnimalMedia.CountAsync(item => item.AnimalId == animalId),
            UploadedAt = DateTime.UtcNow
        };

        _context.AnimalMedia.Add(media);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetMedia),
            new { animalId },
            ToDto(media));
    }

    [HttpPut("{mediaId:int}/cover")]
    public async Task<IActionResult> SetCover(int animalId, int mediaId)
    {
        var media = await _context.AnimalMedia
            .Where(item => item.AnimalId == animalId)
            .ToListAsync();

        var selected = media.FirstOrDefault(item => item.Id == mediaId);

        if (selected is null)
            return NotFound();

        if (selected.MediaType != AnimalMediaType.Image)
            return BadRequest("Samo slika može biti naslovna datoteka.");

        foreach (var item in media)
            item.IsCover = item.Id == mediaId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{mediaId:int}")]
    public async Task<IActionResult> Delete(int animalId, int mediaId)
    {
        var media = await _context.AnimalMedia.FirstOrDefaultAsync(item =>
            item.Id == mediaId && item.AnimalId == animalId);

        if (media is null)
            return NotFound();

        var wasCover = media.IsCover;
        _context.AnimalMedia.Remove(media);

        if (wasCover)
        {
            var nextCover = await _context.AnimalMedia
                .Where(item => item.AnimalId == animalId &&
                               item.Id != mediaId &&
                               item.MediaType == AnimalMediaType.Image)
                .OrderBy(item => item.SortOrder)
                .FirstOrDefaultAsync();

            if (nextCover is not null)
                nextCover.IsCover = true;
        }

        await _context.SaveChangesAsync();

        var webRoot = _environment.WebRootPath
            ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
        var physicalPath = Path.Combine(
            webRoot,
            "uploads",
            "animals",
            Path.GetFileName(media.StoredFileName));

        if (System.IO.File.Exists(physicalPath))
            System.IO.File.Delete(physicalPath);

        return NoContent();
    }

    private AnimalMediaDto ToDto(AnimalMedia media)
    {
        var relativeUrl = $"/uploads/animals/{media.StoredFileName}";

        return new AnimalMediaDto
        {
            Id = media.Id,
            AnimalId = media.AnimalId,
            Url = $"{Request.Scheme}://{Request.Host}{relativeUrl}",
            OriginalFileName = media.OriginalFileName,
            ContentType = media.ContentType,
            MediaType = media.MediaType.ToString(),
            FileSize = media.FileSize,
            Caption = media.Caption,
            IsCover = media.IsCover,
            SortOrder = media.SortOrder,
            UploadedAt = media.UploadedAt
        };
    }
}
