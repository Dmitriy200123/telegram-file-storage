using System;
using System.Threading.Tasks;
using FileStorageAPI.Models;
using FileStorageAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileStorageAPI.Controllers
{
    public class SendersController : Controller
    {
        private readonly ISenderService senderService;

        public SendersController(ISenderService senderService)
        {
            this.senderService = senderService;
        }

        [HttpGet("senders/{id:guid}")]
        public async Task<IActionResult> GetSenderById(Guid id)
        {
            var sender = await senderService.GetSenderById(id);
            if (sender is null)
            {
                return NotFound($"User with identifier {id} not found");
            }

            return Json(sender);
        }

        [HttpPost("senders")]
        public async Task<IActionResult> PostSender(Sender sender)
        {
            var result = await senderService.CreateSender(sender);
            return Created(string.Empty, result);
        }

        [HttpPut("senders/{id:guid}")]
        public async Task<IActionResult> PutSender(Sender sender, Guid id)
        {
            var result = await senderService.UpdateSender(id, sender);
            return Created(string.Empty, result);
        }
    }
}