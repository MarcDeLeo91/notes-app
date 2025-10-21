// src/pages/Dashboard.jsx
import React, { useEffect, useState } from 'react';
import { getNotes, createNote, deleteNote } from '../services/note.service';
import NoteCard from '../components/NoteCard';
import { logout } from '../services/auth.service';
import { useNavigate } from 'react-router-dom';

export default function Dashboard() {
  const [notes, setNotes] = useState([]);
  const [title, setTitle] = useState('');
  const [content, setContent] = useState('');
  const [error, setError] = useState(null); // Estado para manejar errores
  const navigate = useNavigate();

  const load = async () => {
    try {
      const r = await getNotes(); // r es la respuesta axios
      setNotes(r.data);
      setError(null); // Limpiar errores si la solicitud es exitosa
    } catch (err) {
      console.error('Error al cargar las notas:', err.response?.data || err.message);
      if (err.response?.status === 401) {
        // Token inválido/expirado: limpiar y redirigir a login
        logout();
        navigate('/login');
        return;
      }
      setError('No se pudieron cargar las notas. Verifica tu conexión o inicia sesión nuevamente.');
    }
  };

  useEffect(() => {
    load();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const add = async (e) => {
    e.preventDefault();
    try {
      // No busques jwtToken — usamos 'token' y authHeader en servicios
      await createNote({ title, content });
      setTitle('');
      setContent('');
      await load();
    } catch (err) {
      console.error('Error al crear la nota:', err.response?.data || err.message);
      if (err.response?.status === 401) {
        logout();
        navigate('/login');
        return;
      }
      setError('No se pudo crear la nota. Verifica tu conexión o inicia sesión nuevamente.');
    }
  };

  const remove = async (id) => {
    try {
      await deleteNote(id);
      await load();
    } catch (err) {
      console.error('Error al eliminar la nota:', err.response?.data || err.message);
      if (err.response?.status === 401) {
        logout();
        navigate('/login');
        return;
      }
      setError('No se pudo eliminar la nota. Verifica tu conexión o inicia sesión nuevamente.');
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
        />
        <input
          placeholder="Contenido"
          value={content}
          onChange={(e) => setContent(e.target.value)}
          required
        />
        <button type="submit">Crear</button>
      </form>

      <div style={{ display: 'grid', gap: 10 }}>
        {notes.map((n) => (
          <NoteCard key={n.id} note={n} onDelete={() => remove(n.id)} />
        ))}
      </div>
    </div>
  );
}
