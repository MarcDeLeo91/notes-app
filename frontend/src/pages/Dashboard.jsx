import React, { useEffect, useState } from 'react';
import { getNotes, createNote, deleteNote } from '../services/note.service';
import NoteCard from '../components/NoteCard';
import { logout } from '../services/auth.service';
import { useNavigate } from 'react-router-dom';

export default function Dashboard() {
  const [notes, setNotes] = useState([]); // Inicializa como un array vacío
  const [title, setTitle] = useState('');
  const [content, setContent] = useState('');
  const [error, setError] = useState(null); // Estado para manejar errores
  const navigate = useNavigate();

  // Cargar notas desde el backend
  const load = async () => {
    try {
      const response = await getNotes();
      setNotes(response.data || []); // Si no hay datos, establece un array vacío
      setError(null); // Limpiar errores si la solicitud es exitosa
    } catch (err) {
      console.error('Error al cargar las notas:', err.response?.data || err.message);
      if (err.response?.status === 401) {
        // Token inválido/expirado: limpiar y redirigir a login
        logout();
        navigate('/login');
        return;
      }
      setError(
        err.response?.data?.message ||
        'No se pudieron cargar las notas. Verifica tu conexión o inicia sesión nuevamente.'
      );
      setNotes([]); // Establece un array vacío en caso de error
    }
  };

  // Cargar notas al montar el componente
  useEffect(() => {
    load();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  // Crear una nueva nota
  const add = async (e) => {
    e.preventDefault();
    if (!title.trim() || !content.trim()) {
      setError('El título y el contenido no pueden estar vacíos.');
      return;
    }
    try {
      await createNote({ title, content });
      setTitle('');
      setContent('');
      await load(); // Recargar las notas después de crear una
    } catch (err) {
      console.error('Error al crear la nota:', err.response?.data || err.message);
      if (err.response?.status === 401) {
        logout();
        navigate('/login');
        return;
      }
      setError(
        err.response?.data?.message ||
        'No se pudo crear la nota. Verifica tu conexión o intenta nuevamente.'
      );
    }
  };

  // Eliminar una nota
  const remove = async (id) => {
    try {
      await deleteNote(id);
      await load(); // Recargar las notas después de eliminar una
    } catch (err) {
      console.error('Error al eliminar la nota:', err.response?.data || err.message);
      if (err.response?.status === 401) {
        logout();
        navigate('/login');
        return;
      }
      setError(
        err.response?.data?.message ||
        'No se pudo eliminar la nota. Verifica tu conexión o intenta nuevamente.'
      );
    }
  };

  return (
    <div>
      <h2>Dashboard</h2>
      {error && <p style={{ color: 'red' }}>{error}</p>}
      <form onSubmit={add} style={{ marginBottom: 16 }}>
        <input
          placeholder="Título"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          required
          maxLength={50} // Límite de longitud
        />
        <input
          placeholder="Contenido"
          value={content}
          onChange={(e) => setContent(e.target.value)}
          required
          maxLength={200} // Límite de longitud
        />
        <button type="submit">Crear</button>
      </form>

      <div style={{ display: 'grid', gap: 10 }}>
        {notes && notes.length > 0 ? (
          notes.map((note) => (
            <NoteCard key={note.id} note={note} onDelete={() => remove(note.id)} />
          ))
        ) : (
          <p>No hay notas disponibles.</p>
        )}
      </div>
    </div>
  );
}