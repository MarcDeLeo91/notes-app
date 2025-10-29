import React from "react";

export default function CourseCard({ course }) {
  return (
    <div className="bg-white border rounded-lg p-4 shadow-sm hover:shadow-md transition">
      <h3 className="text-lg font-semibold text-indigo-700">{course.nombre}</h3>
      <p className="text-sm text-gray-600">Código: {course.codigo}</p>
      <p className="text-xs text-gray-500 mt-2">
        Créditos: {course.creditos || "N/A"}
      </p>
    </div>
  );
}
