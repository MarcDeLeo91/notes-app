import React from "react";

export default function StudentCard({ student }) {
  return (
    <div className="bg-white border rounded-lg p-4 shadow-sm hover:shadow-md transition">
      <h3 className="text-lg font-semibold text-indigo-700">{student.nombre}</h3>
      <p className="text-gray-600 text-sm">Email: {student.email}</p>
      <p className="text-gray-500 text-xs mt-2">ID: {student.id}</p>
    </div>
  );
}
