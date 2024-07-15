using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

public class VideoController : Controller
{
    private readonly IAmazonS3 _s3Client;

    public VideoController(IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
    }

    [HttpGet]
    public IActionResult Upload()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile videoFile)
    {
        if (videoFile == null || videoFile.Length == 0)
        {
            ModelState.AddModelError("videoFile", "Please select a file to upload.");
            return View();
        }

        try
        {
            // Prepare S3 bucket information
            string bucketName = "jeniawsbucket16";
            string keyName = "videos/" + Guid.NewGuid() + Path.GetExtension(videoFile.FileName);

            // Upload the video file to S3
            using (var stream = videoFile.OpenReadStream())
            {
                var fileTransferUtility = new TransferUtility(_s3Client);
                await fileTransferUtility.UploadAsync(stream, bucketName, keyName);
            }

            // Optionally, save metadata or database record about the uploaded video

            ViewBag.Message = "File uploaded successfully.";
            return View();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
            return View();
        }
    }
}
