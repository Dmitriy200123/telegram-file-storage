using System;
using System.Threading.Tasks;
using FileStorageAPI.Models;
using FileStorageAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileStorageAPI.Controllers
{
    public class FilesController : Controller
    {
        private readonly IFilesService filesService;

        public FilesController(IFilesService filesService)
        {
            this.filesService = filesService;
        }

        [HttpGet("files")]
        public async Task<IActionResult> GetFiles()
        {
            var files = await filesService.GetFiles();
            return Json(files);
        }

        [HttpGet("files/{id:guid}")]
        public async Task<IActionResult> GetFileById(Guid id)
        {
            var file = await filesService.GetFileById(id);
            if (file is null)
            {
                return NotFound($"File with identifier {id} not found");
            }

            return Json(file);
        }

        [HttpPost("files")]
        public async Task<IActionResult> PostFile([FromForm] UploadFile uploadFile)
        {
            var file = await filesService.CreateFile(uploadFile);
            return Created(string.Empty, file);
        }

        [HttpPut("files/{id:guid}")]
        public async Task<IActionResult> PutFile(UpdateFile updateFile)
        {
            var file = await filesService.UpdateFile(updateFile);
            return Created(string.Empty, file);
        }

        [HttpDelete("files/{id:guid}")]
        public async Task<IActionResult> DeleteFile(Guid id)
        {
            await filesService.DeleteFile(id);
            return NoContent();
        }
    }
}