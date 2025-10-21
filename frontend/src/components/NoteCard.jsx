import React from 'react';

export default function NoteCard({ note, onDelete }) {
  return (
    <div style={{ border: '1px solid #ccc', padding: 10, borderRadius: 6 }}>
      <h4>{note.title}</h4>
      <p>{note.content}</p>
      <div style={{ fontSize: 12, color: '#666' }}>Creada: {new Date(note.createdAt).toLocaleString()}</div>
      <button onClick={onDelete} style={{ marginTop: 8 }}>Eliminar</button>
    </div>
  );
}
