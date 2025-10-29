using Microsoft.EntityFrameworkCore;
using NotesApi.Data;
using NotesApi.Models;

namespace NotesApi.Services
{
    public class NotaService
    {
        private readonly AppDbContext _context;
        public NotaService(AppDbContext context) => _context = context;

        public async Task<List<NotaAcademica>> GetAllAsync()
        {
            try
            {
                var notas = await _context.Notas.Include(n => n.Inscripcion)
                                        .ToListAsync();

                foreach (var nota in notas)
                {
                    if (nota.Inscripcion != null)
                    {
                        await _context.Entry(nota.Inscripcion).Reference(i => i.Estudiante).LoadAsync();
                        await _context.Entry(nota.Inscripcion).Reference(i => i.Curso).LoadAsync();
                    }
                }

                return notas;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching all notes: {ex.Message}");
                throw new Exception("Error al obtener las notas. Verifica la base de datos y las relaciones.");
            }
        }

        public async Task<NotaAcademica?> GetByIdAsync(int id) =>
            await _context.Notas.Include(n => n.Inscripcion)
                                .FirstOrDefaultAsync(n => n.Id == id);

        public async Task<NotaAcademica> CreateAsync(NotaAcademica nota)
        {
            _context.Notas.Add(nota);
            await _context.SaveChangesAsync();
            return nota;
        }

        public async Task<List<NotaAcademica>> GetNotasByEstudianteAsync(int estudianteId)
        {
            return await _context.Notas
                .Include(n => n.Inscripcion!)
                .ThenInclude(i => i.Estudiante!)
                .Where(n => n.Inscripcion!.EstudianteId == estudianteId)
                .ToListAsync();
        }
    }
}
