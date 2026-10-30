using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery.Web.Services;
public interface IImageStorage
{
    bool IsValid(IFormFile file);
    Task<string> SaveAsync(IFormFile file, string folder);
    void Delete(string? url);
}

public class ImageStorage(IWebHostEnvironment env) : IImageStorage
{
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private const long MaxSize = 5 * 1024 * 1024;

    public bool IsValid(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return file.Length > 0 && file.Length <= MaxSize && AllowedExtensions.Contains(extension);
    }

    public async Task<string> SaveAsync(IFormFile file, string folder)
    {
        var directory = Path.Combine(env.WebRootPath, "uploads", folder);
        Directory.CreateDirectory(directory);

        var fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName).ToLowerInvariant();
        await using var stream = new FileStream(Path.Combine(directory, fileName), FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/uploads/{folder}/{fileName}";
    }

    public void Delete(string? url)
    {
        if (string.IsNullOrEmpty(url) || !url.StartsWith("/uploads/"))
        {
            return;
        }

        var fullPath = Path.Combine(env.WebRootPath, url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }
}
