import React, { useEffect, useState } from "react";
import cursoService from "../services/curso.service";
import CourseCard from "../components/CourseCard";

export default function Courses() {
  const [courses, setCourses] = useState([]);

  useEffect(() => {
    cursoService.getCursos().then((response) => {
      if (response.data && Array.isArray(response.data.$values)) {
        setCourses(response.data.$values);
      } else {
        console.error("La respuesta de la API no contiene un arreglo válido:", response.data);
      }
    }).catch((error) => {
      console.error("Error al obtener los cursos:", error);
    });
  }, []);

  return (
    <div className="p-6">
      <h1 className="text-xl font-bold mb-4">Cursos</h1>
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {Array.isArray(courses) && courses.map((c) => (
          <CourseCard key={c.id} course={c} />
        ))}
      </div>
    </div>
  );
}
