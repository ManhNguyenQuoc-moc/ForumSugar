namespace ForumSugar.Services.Interfaces
{
    public interface IPhotoService
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
}
