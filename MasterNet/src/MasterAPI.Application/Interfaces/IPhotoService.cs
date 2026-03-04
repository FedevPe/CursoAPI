using MasterAPI.Application.Imagenes;
using Microsoft.AspNetCore.Http;

namespace MasterAPI.Application.Interfaces
{
    public interface IPhotoService
    {
        Task<PhotoUploadResult> AddPhoto(IFormFile file);
        Task<string> DeletePhoto(string publicId);
    }
}