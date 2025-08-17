using LunchWeb.Services.Interfaces;

namespace LunchWeb.Services.Implementations
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _env;
        private string[] allowed = [".jpg", ".jpeg", ".png", ".webp"];

        public ImageService(IWebHostEnvironment env) => _env = env;
        public async Task<string> SaveMealImageAsync(IFormFile file, CancellationToken ct = default)
        {
            if (file == null || file.Length == 0) throw new ArgumentException("Soubor je prázdný!");
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowed.Contains(ext)) throw new InvalidOperationException("Nepodporovaný formát!");

            var fileName = $"{Guid.NewGuid():N}{ext}";
            var relPath = Path.Combine("images", "meals", fileName).Replace("\\", "/");
            var absPath = Path.Combine(_env.WebRootPath, "images", "meals", fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(absPath)!);
            using var stream = File.Create(absPath);
            await file.CopyToAsync(stream, ct);

            return "/" + relPath;
        }

        public Task DeleteAsync(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return Task.CompletedTask;
            var abs = Path.Combine(_env.WebRootPath, relativePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
            if (File.Exists(abs)) File.Delete(abs);
            return Task.CompletedTask;
        }
    }
}

