using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace DownloadProxy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileProxyController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public FileProxyController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("download")]
        public async Task<IActionResult> DownloadFile(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
            {
                return BadRequest();
            }

            try
            {
                // Create an HttpClient instance
                var httpClient = _httpClientFactory.CreateClient();

                // Make an asynchronous request to the specified file URL
                var response = await httpClient.GetAsync(fileUrl);
                var ext = Path.GetExtension(fileUrl);
                // Check if the response is successful
                if (response.IsSuccessStatusCode)
                {
                    // Stream the file content to the client
                    var stream = await response.Content.ReadAsStreamAsync();

                    var provider = new FileExtensionContentTypeProvider();


                    return new FileStreamResult(stream, "application/octet-stream")
                    {
                        //Change or update file name here.
                        FileDownloadName = "downloaded_file." + ext
                    };
                }
                else
                {
                    return BadRequest("Error downloading the file.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
