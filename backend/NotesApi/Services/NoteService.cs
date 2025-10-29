using Microsoft.EntityFrameworkCore;
using NotesApi.Data;
using NotesApi.Models;

namespace NotesApi.Services
{
    public class NoteService
    {
        private readonly AppDbContext _context;
        public NoteService(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Note>> GetAllAsync() =>
            await _context.Notes.AsNoTracking().ToListAsync();

        public async Task<Note?> GetByIdAsync(int id) =>
            await _context.Notes.FindAsync(id);

        public async Task<Note> CreateAsync(Note note)
        {
            note.CreatedAt = DateTime.UtcNow;
            _context.Notes.Add(note);
            await _context.SaveChangesAsync();
            return note;
        }

        public async Task<bool> UpdateAsync(Note note)
        {
            var existing = await _context.Notes.FindAsync(note.Id);
            if (existing == null) return false;

            existing.Title = note.Title;
            existing.Content = note.Content;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note == null) return false;

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
