﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NTS.Server.Database.DatabaseContext;
using NTS.Server.Entities;
using NTS.Server.Entities.DTOs;
using NTS.Server.Services.Contracts;

namespace NTS.Server.Services
{
    public class NotesService : INotesService
    {
        private readonly ApplicationDbContext dbContext;

        public NotesService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Notes?> CreateNoteAsync(NotesDto request, Guid userId)
        {
            try
            {
                var note = new Notes 
                {
                    UserId = userId,
                    Title = request.Title,
                    Content = request.Content,
                    Priority = request.Priority,
                    CreatedAt = DateTime.UtcNow
                };

                dbContext.Notes.Add(note);
                await dbContext.SaveChangesAsync();
                return note;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Creating Note: {ex.Message}");
            }
        }


        public async Task<Notes> EditNotesAsync( EditNotesDto editNotesDto, Guid noteId, Guid userId)
        {
            try
            {
                var existingNote = await dbContext.Notes.FindAsync(editNotesDto.NoteId);
                if (existingNote == null || existingNote.UserId != userId) return null;

                existingNote.Title = editNotesDto.Title;
                existingNote.Content = editNotesDto.Content;
                existingNote.Priority = editNotesDto.Priority;

                dbContext.Notes.Update(existingNote);
                await dbContext.SaveChangesAsync();
                return existingNote;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Editing Note: {ex.Message}");
            }
        }


        public async Task<bool> RemoveNoteAsync(Guid noteId)
        {
            try
            {
                var note = await dbContext.Notes.FindAsync(noteId);
                if (note == null) return false;

                dbContext.Remove(note);
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Removing Note: {ex.Message}");
            }
        }


        public async Task<List<Notes>> GetAllNotesAsync(Guid userId)
        {
            return await dbContext.Notes.Where(n => n.UserId == userId).ToListAsync();
        }

        public async Task<Notes> GetNoteByIdAsync(Guid noteId, Guid userId)
        {
            var note = await dbContext.Notes.FindAsync(noteId);
            if (note == null || (note.UserId != userId && !dbContext.SharedNotes.Any(sn => sn.NoteId == noteId && sn.UserId == userId)))
                return null;

            return note;
        }


        public async Task<IQueryable<Notes>> SearchNotesAsync(string searchTerm)
        {
            var notes = dbContext.Notes.Where(n => n.Title.Contains(searchTerm) || n.Content.Contains(searchTerm));
            return notes;
        }


        public async Task<bool> MarkNoteAsFavoriteAsync(Guid noteId, Guid userId)
        {
            var note = await dbContext.Notes.FindAsync(noteId);
            if (note == null || note.UserId != userId) return false;

            var favoriteNote = new FavoriteNotes
            {
                FavoriteNoteId = Guid.NewGuid(),
                NoteId = noteId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            dbContext.FavoriteNotes.Add(favoriteNote);
            await dbContext.SaveChangesAsync();
            return true;
        }


        public async Task<bool> MarkNoteAsImportantAsync(Guid noteId, Guid userId)
        {
            try
            {
                var note = await dbContext.Notes.FindAsync(noteId);
                if (note == null || note.UserId != userId) return false;

                var importantNote = new ImportantNotes
                {
                    ImportantNoteId = Guid.NewGuid(),
                    NoteId = noteId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };

                dbContext.ImportantNotes.Add(importantNote);
                await dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Marking The Note: {ex.Message}");
            }
        }


        public async Task<bool> MarkNoteAsSharedAsync(Guid noteId, Guid userId, Guid sharedWithUserId)
        {
            try
            {
                var note = await dbContext.Notes.FindAsync(noteId);
                if (note == null || note.UserId != userId) return false;

                var sharedNote = new SharedNotes
                {
                    SharedNoteId = Guid.NewGuid(),
                    NoteId = noteId,
                    UserId = userId,
                    SharedWithUserId = sharedWithUserId,
                    CreatedAt = DateTime.UtcNow
                };

                dbContext.SharedNotes.Add(sharedNote);
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Marking Note: {ex.Message}");
            }
        }


        public async Task<bool> MarkNoteAsStarredAsync(Guid noteId, Guid userId)
        {
            try
            {
                var note = await dbContext.Notes.FindAsync(noteId);
                if (note == null || note.UserId != userId) return false;

                var starredNote = new StarredNotes
                {
                    StarredNotesId = Guid.NewGuid(),
                    NoteId = noteId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };

                dbContext.StarredNotes.Add(starredNote);
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Marking Note: {ex.Message}");
            }
        }
    }
}