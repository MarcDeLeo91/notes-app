using Microsoft.EntityFrameworkCore;
using NotesApi.Data;
using NotesApi.Models;

namespace NotesApi.Services
{
    public class CursoService
    {
        private readonly AppDbContext _context;
        public CursoService(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Curso>> GetAllAsync() =>
            await _context.Cursos.Include(c => c.Profesor).ToListAsync();

        public async Task<Curso?> GetByIdAsync(int id) =>
            await _context.Cursos.Include(c => c.Profesor)
                                 .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<Curso> CreateAsync(Curso curso)
        {
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();
            return curso;
        }

        public async Task<bool> UpdateAsync(int id, Curso curso)
        {
            var existing = await _context.Cursos.FindAsync(id);
            if (existing == null) return false;

            _context.Entry(existing).CurrentValues.SetValues(curso);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var c = await _context.Cursos.FindAsync(id);
            if (c == null) return false;
            _context.Cursos.Remove(c);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
