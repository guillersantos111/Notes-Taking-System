﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NTS.Server.Services.Contracts;
using System.Security.Claims;

namespace NTS.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavoriteNotesController : ControllerBase
    {
        private readonly IFavoriteNoteService favoriteNoteService;

        public FavoriteNotesController(IFavoriteNoteService favoriteNoteService)
        {
            this.favoriteNoteService = favoriteNoteService;
        }

        [HttpPost("mark-favorite{noteId}"), Authorize(Roles = "DefaultUser")]
        public async Task<IActionResult> MarkNoteAsFavoriteAsync(Guid noteId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                var userId = Guid.Parse(userIdClaim!.Value);

                var markedNote = await favoriteNoteService.MarkNotesAsFavoriteAsync(noteId, userId);

                if (!markedNote)
                    return NotFound("Note Not Found Or Your Are Not Authorized To Mark This Note As Favorite");

                return Ok(markedNote);
            }
            catch (Exception error)
            {
                return BadRequest(error.Message);
            }
        }
    }
}
