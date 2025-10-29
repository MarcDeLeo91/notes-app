import React from "react";

export default function NoteCard({ note }) {
  return (
    <div className="bg-white shadow-md rounded-lg p-4 border hover:shadow-lg transition">
      <h3 className="text-lg font-semibold mb-1">{note.title}</h3>
      <p className="text-sm text-gray-700">{note.content}</p>
      <div className="text-xs text-gray-400 mt-2">
        Creado el {new Date(note.createdAt).toLocaleDateString()}
      </div>
    </div>
  );
}
