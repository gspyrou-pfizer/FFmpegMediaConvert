using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Xabe.FFmpeg;

namespace FFmpegMediaConvert
{
    public static class FFmpegMediaConvert
    {
        [FunctionName("FFmpegMediaConvert")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

                string name = req.Query["name"];
                string extension = req.Query["extension"];
                var fileToConvert = new FileInfo(name);
                log.LogInformation($"Input file : {fileToConvert.FullName}");
                string outputFileName = Path.ChangeExtension(Path.GetTempFileName(), extension);

                var conversion = await FFmpeg.Conversions.FromSnippet.Convert(fileToConvert.FullName, outputFileName);
                await conversion.Start();

                log.LogInformation($"Converted file : {outputFileName}");

                return new OkObjectResult(outputFileName);

            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }

        }
    }
}
