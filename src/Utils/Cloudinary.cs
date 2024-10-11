using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DotNetEnv;

namespace BlogApp.Utils;

public class Uploader
{
    private readonly Cloudinary _cloudinary;

    public Uploader()
    {
        var cloudName = Env.GetString("CLOUD_NAME");
        var apiKey = Env.GetString("API_KEY");
        var apiSecret = Env.GetString("API_SECRET");

        var account = new Account(cloudName, apiKey, apiSecret);

        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> UploadImageAsync(IFormFile file)
    {
        string folderName = Env.GetString("FOLDER_NAME");

        if (file == null || file.Length == 0)
            throw new ArgumentException("File cannot be null or empty");

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, file.OpenReadStream()),
            Folder = folderName,
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        return uploadResult.SecureUrl.ToString();
    }
}
