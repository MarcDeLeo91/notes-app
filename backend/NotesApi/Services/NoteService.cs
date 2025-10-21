using Microsoft.EntityFrameworkCore;
using NotesApi.Data;
using NotesApi.Models;

namespace NotesApi.Services
{
    public class NoteService
    {
        private readonly AppDbContext _context;

        public NoteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Note>> GetNotes(int userId)
        {
            return await _context.Notes.Where(n => n.UserId == userId).ToListAsync();
        }

        public async Task<Note?> GetNote(int userId, int noteId)
        {
            return await _context.Notes.FirstOrDefaultAsync(n => n.Id == noteId && n.UserId == userId);
        }

        public async Task<Note> CreateNote(int userId, Note note)
        {
            note.UserId = userId;
            _context.Notes.Add(note);
            await _context.SaveChangesAsync();
            return note;
        }

        public async Task<bool> UpdateNote(int userId, int noteId, Note updatedNote)
        {
            var note = await _context.Notes.FirstOrDefaultAsync(n => n.Id == noteId && n.UserId == userId);
            if (note == null) return false;

            note.Title = updatedNote.Title;
            note.Content = updatedNote.Content;
            note.IsArchived = updatedNote.IsArchived;
            note.IsFavorite = updatedNote.IsFavorite;
            note.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteNote(int userId, int noteId)
        {
            var note = await _context.Notes.FirstOrDefaultAsync(n => n.Id == noteId && n.UserId == userId);
            if (note == null) return false;

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();
            return true;
        }

        // métodos auxiliares que puedes implementar si deseas:
        // ArchiveNote, ToggleFavorite, AddTag, SearchNotes, etc.
    }
}
