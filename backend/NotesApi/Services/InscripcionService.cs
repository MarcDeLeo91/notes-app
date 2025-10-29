using Microsoft.EntityFrameworkCore;
using NotesApi.Data;
using NotesApi.Models;

namespace NotesApi.Services
{
    public class InscripcionService
    {
        private readonly AppDbContext _context;
        public InscripcionService(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Inscripcion>> GetAllAsync() =>
            await _context.Inscripciones
                .Include(i => i.Curso)
                .Include(i => i.Estudiante)
                .ToListAsync();

        public async Task<Inscripcion?> GetByIdAsync(int id) =>
            await _context.Inscripciones
                .Include(i => i.Curso)
                .Include(i => i.Estudiante)
                .FirstOrDefaultAsync(i => i.Id == id);

        public async Task<Inscripcion> CreateAsync(Inscripcion inscripcion)
        {
            _context.Inscripciones.Add(inscripcion);
            await _context.SaveChangesAsync();
            return inscripcion;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var inscripcion = await _context.Inscripciones.FindAsync(id);
            if (inscripcion == null) return false;
            _context.Inscripciones.Remove(inscripcion);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Inscripcion>> GetByCursoAsync(int cursoId)
        {
            return await _context.Inscripciones
                .Where(i => i.CursoId == cursoId)
                .Include(i => i.Curso)
                .Include(i => i.Estudiante)
                .ToListAsync();
        }

        public async Task<IEnumerable<Inscripcion>> GetByEstudianteAsync(int estudianteId)
        {
            return await _context.Inscripciones
                .Where(i => i.EstudianteId == estudianteId)
                .Include(i => i.Curso)
                .Include(i => i.Estudiante)
                .ToListAsync();
        }
    }
}
