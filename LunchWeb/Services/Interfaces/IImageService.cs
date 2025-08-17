namespace LunchWeb.Services.Interfaces
{
    public interface IImageService
    {
        Task<string> SaveMealImageAsync(IFormFile file, CancellationToken ct = default);
        Task DeleteAsync(string? relativePath);
    }
}
