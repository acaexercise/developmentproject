using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ACA.Data;
using ACA.Domain;
using CsvHelper;
using CsvHelper.TypeConversion;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace ACA.Classes.Blazor.Controllers
{
    [Route("api/[controller]/[action]")]
    public class UploadController : Controller
    {
        private readonly ICsvDataFileService _csvDataFileService;
        private readonly ILogger<UploadController> _logger;
        public IWebHostEnvironment HostingEnvironment { get; set; }

        public UploadController(IWebHostEnvironment hostingEnvironment,ICsvDataFileService csvDataFileService,
            ILogger<UploadController> logger)
        {
            _csvDataFileService = csvDataFileService;
            _logger = logger;
            HostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        public async Task<IActionResult> Save(IEnumerable<IFormFile> files) // the default field name. See SaveField
        {
            if (files != null)
            {
                try
                {
                    foreach (var file in files)
                    {
                        var fileContent = ContentDispositionHeaderValue.Parse(file.ContentDisposition);

                        // Some browsers send file names with full path.
                        // We are only interested in the file name.
                        var fileName = Path.GetFileName(fileContent.FileName.ToString().Trim('"'));
                
                        // Implement security mechanisms here - prevent path traversals,
                        // check for allowed extensions, types, size, content, viruses, etc.
                        // this sample always saves the file to the root and is not sufficient for a real application

                        var memoryStream = new MemoryStream();
                        await file.CopyToAsync(memoryStream);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        //Before saving file to data folder, verify we can parse it
                        var reader = new StreamReader(memoryStream);
                        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                        var records = csv.GetRecords<Student>().ToList();

                        var results = await _csvDataFileService.SaveStreamToFile(memoryStream,"DataFiles", fileName);
                        return new OkObjectResult(results);
                    }
                }
                catch (HeaderValidationException ex)
                {
                    var message = $"Invalid Header(s) found on file - {string.Join(',',ex.HeaderNames)} could not be found";
                    _logger.LogError("UploadController - Invalid Header found on file - {exception}", ex);
                    await HandleFailure(ex, message);
                }
                catch (BadDataException ex)
                {
                    var message = $"Invalid Data found on file ";
                    _logger.LogError("UploadController - Invalid Data found on file {exception}", ex);
                    await HandleFailure(ex, message);
                }
                catch (TypeConverterException ex)
                {
                    var message = $"Invalid Data (could not convert type) found on file";
                    _logger.LogError("UploadController - Invalid Data (could not convert type) found on file - {exception}", ex);
                    await HandleFailure(ex, message);
                }
                //TODO: Account for other types of invalid data
                catch (Exception ex)
                {
                    await HandleFailure(ex);
                }
            }

            // Return an empty string message in this case
            return new EmptyResult();
        }

        private async Task HandleFailure(Exception ex,string message=null)
        {
            // implement error handling here, this merely indicates a failure to the upload
            Response.StatusCode = 500;
            await Response.WriteAsync(message ?? ex.ToString()); // custom error message
        }

    }
}