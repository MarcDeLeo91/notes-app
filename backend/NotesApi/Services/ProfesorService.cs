using Microsoft.EntityFrameworkCore;
using NotesApi.Data;
using NotesApi.Models;

namespace NotesApi.Services
{
    public class ProfesorService
    {
        private readonly AppDbContext _context;
        public ProfesorService(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Profesor>> GetAllAsync() =>
            await _context.Profesores.Include(p => p.Cursos).ToListAsync();

        public async Task<Profesor?> GetByIdAsync(int id) =>
            await _context.Profesores.Include(p => p.Cursos)
                                     .FirstOrDefaultAsync(p => p.Id == id);

        public async Task<Profesor> CreateAsync(Profesor profesor)
        {
            _context.Profesores.Add(profesor);
            await _context.SaveChangesAsync();
            return profesor;
        }

        public async Task<bool> UpdateAsync(int id, Profesor profesor)
        {
            var existing = await _context.Profesores.FindAsync(id);
            if (existing == null) return false;

            _context.Entry(existing).CurrentValues.SetValues(profesor);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var p = await _context.Profesores.FindAsync(id);
            if (p == null) return false;
            _context.Profesores.Remove(p);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
