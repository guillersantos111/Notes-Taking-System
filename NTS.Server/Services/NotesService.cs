﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NTS.Server.Data;
using NTS.Server.Entities;
using NTS.Server.Entities.DTOs;
using NTS.Server.Services.Contracts;

namespace NTS.Server.Services
{
    public class NotesService : INotesService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IFavoriteNoteService favoriteNoteService;
        private readonly IImpotantNotesService impotantNotesService;
        private readonly ISharedNotesService sharedNotesService;
        private readonly IStarredNotesService starredNotesService;

        public NotesService(ApplicationDbContext dbContext, IFavoriteNoteService favoriteNoteService, IImpotantNotesService impotantNotesService, ISharedNotesService sharedNotesService, IStarredNotesService starredNotesService)
        {
            this.dbContext = dbContext;
            this.favoriteNoteService = favoriteNoteService;
            this.impotantNotesService = impotantNotesService;
            this.sharedNotesService = sharedNotesService;
            this.starredNotesService = starredNotesService;
        }


        public async Task<IEnumerable<Notes>> GetAllNotesAsync(Guid userId)
        {
            try
            {
                return await dbContext.Notes
                    .Where(note => note.UserId == userId)
                    .ToListAsync();
            }
            catch (Exception error)
            {
                throw new Exception($"Error Fetching All Notes: {error.Message}");
            }
        }


        public async Task<Notes> GetNoteByIdAsync(Guid noteId)
        {
            try
            {
                var fetchedNote = await dbContext.Notes.FindAsync(noteId);
                return fetchedNote!;
            }
            catch (Exception error)
            {
                throw new Exception($"Error Fetching Note By ID: {error.Message}");
            }
        }


        public async Task<IEnumerable<Notes>> SearchNotesAsync(string searchTerm, Guid userId)
        {
            try
            {
                return await dbContext.Notes
                    .Where(note => note.UserId == userId &&
                         (note.Title.Contains(searchTerm) || note.Content.Contains(searchTerm)))
                    .ToListAsync();
            }
            catch (Exception error)
            {
                throw new Exception($"Error Searching Notes: {error.Message}");
            }
        }


        public async Task<Notes?> CreateNoteAsync([FromBody] CreateNotesDto noteDetails, Guid userId)
        {
            try
            {
                var newNote = new Notes
                {
                    UserId = userId,
                    Title = noteDetails.Title,
                    Content = noteDetails.Content,
                    Priority = noteDetails.Priority,
                    CreatedAt = DateTime.UtcNow
                };

                dbContext.Notes.Add(newNote);
                await dbContext.SaveChangesAsync();
                return newNote;
            }
            catch (Exception error)
            {
                throw new Exception($"Error Creating Note: {error.Message}");
            }
        }


        public async Task<Notes> UpdateNotesAsync(UpdateNotesDto updatedNoteDetails, Guid noteId, Guid userId)
        {
            try
            {
                var existingNote = await dbContext.Notes.FindAsync(noteId);
                if (existingNote == null || existingNote.UserId != userId) return null;

                existingNote.Title = updatedNoteDetails.Title;
                existingNote.Content = updatedNoteDetails.Content;
                existingNote.Priority = updatedNoteDetails.Priority;

                dbContext.Notes.Update(existingNote);
                await dbContext.SaveChangesAsync();
                return existingNote;
            }
            catch (Exception error)
            {
                throw new Exception($"Error Editing Note: {error.Message}");
            }
        }


        public async Task<bool> RemoveNoteAsync(Guid noteId)
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var noteToRemove = await dbContext.Notes.FindAsync(noteId);
                if (noteToRemove == null)
                    return false;

                await favoriteNoteService.RemoveByNoteIdAsync(noteId);
                await impotantNotesService.RemoveByNoteIdAsync(noteId);
                await sharedNotesService.RemoveByNoteIdAsync(noteId);
                await starredNotesService.RemoveByNoteIdAsync(noteId);

                dbContext.Notes.Remove(noteToRemove);

                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception error)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error Removing Note: {error.Message}");
            }
        }
    }
}
