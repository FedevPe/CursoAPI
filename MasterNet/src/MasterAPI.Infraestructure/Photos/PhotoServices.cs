using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MasterAPI.Application.Imagenes;
using MasterAPI.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace MasterAPI.Insfraestructure.Photos
{
    public class PhotoServices : IPhotoService
    {
        private readonly Cloudinary _cloud;

        public PhotoServices(IOptions<CloudinarySettings> config)
        {
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            _cloud = new Cloudinary(account);
        }

        public async Task<PhotoUploadResult> AddPhoto(IFormFile file)
        {
            if (file.Length > 0)
            {
                await using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill")
                };

                var result = await _cloud.UploadAsync(uploadParams);

                if(result.Error is not null)
                {
                    throw new Exception(result.Error.Message);
                }

                return new PhotoUploadResult
                {
                    PublicId = result.PublicId,
                    UrlImage = result.SecureUrl.ToString()
                };
            }

            return null!;
        }

        public async Task<string> DeletePhoto(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloud.DestroyAsync(deleteParams);
            return result.Result == "ok" ? result.Result! : null!;
        }
    }
}