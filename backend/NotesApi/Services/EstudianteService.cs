using Microsoft.EntityFrameworkCore;
using NotesApi.Data;
using NotesApi.Models;

namespace NotesApi.Services
{
    public class EstudianteService
    {
        private readonly AppDbContext _context;
        public EstudianteService(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Estudiante>> GetAllAsync() =>
            await _context.Estudiantes.Include(e => e.Inscripciones).ToListAsync();

        public async Task<Estudiante?> GetByIdAsync(int id) =>
            await _context.Estudiantes.Include(e => e.Inscripciones!)
                                      .ThenInclude(i => i.Curso)
                                      .FirstOrDefaultAsync(e => e.Id == id);

        public async Task<Estudiante> CreateAsync(Estudiante estudiante)
        {
            _context.Estudiantes.Add(estudiante);
            await _context.SaveChangesAsync();
            return estudiante;
        }

        public async Task<bool> UpdateAsync(int id, Estudiante estudiante)
        {
            var existing = await _context.Estudiantes.FindAsync(id);
            if (existing == null) return false;

            _context.Entry(existing).CurrentValues.SetValues(estudiante);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var e = await _context.Estudiantes.FindAsync(id);
            if (e == null) return false;
            _context.Estudiantes.Remove(e);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
