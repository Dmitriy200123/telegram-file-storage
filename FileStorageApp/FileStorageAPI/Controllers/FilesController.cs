using System;
using System.Threading.Tasks;
using FileStorageAPI.Models;
using FileStorageAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileStorageAPI.Controllers
{
    public class FilesController : Controller
    {
        private readonly IFileService fileService;

        public FilesController(IFileService fileService)
        {
            this.fileService = fileService;
        }

        [HttpGet("files")]
        public async Task<IActionResult> GetFiles()
        {
            var files = await fileService.GetFiles();
            return Ok(files);
        }

        [HttpGet("files/{id:guid}")]
        public async Task<IActionResult> GetFileById(Guid id)
        {
            var file = await fileService.GetFileById(id);
            if (file is null)
            {
                return NotFound($"File with identifier {id} not found");
            }

            return Ok(file);
        }

        [HttpPost("files")]
        public async Task<IActionResult> PostFile([FromForm] UploadFile uploadFile)
        {
            var file = await fileService.CreateFile(uploadFile);
            return Created(string.Empty, file);
        }

        [HttpPut("files/{id:guid}")]
        public async Task<IActionResult> PutFile(UpdateFile updateFile)
        {
            var file = await fileService.UpdateFile(updateFile);
            return Created(string.Empty, file);
        }

        [HttpDelete("files/{id:guid}")]
        public async Task<IActionResult> DeleteFile(Guid id)
        {
            await fileService.DeleteFile(id);
            return NoContent();
        }
    }
}