import React, { useEffect, useState } from "react";
import estudianteService from "../services/estudiante.service";
import StudentCard from "../components/StudentCard";

export default function Students() {
  const [students, setStudents] = useState([]);

  useEffect(() => {
    estudianteService.getEstudiantes().then((response) => {
      if (response.data && Array.isArray(response.data.$values)) {
        setStudents(response.data.$values);
      } else {
        console.error("La respuesta de la API no contiene un arreglo válido:", response.data);
      }
    }).catch((error) => {
      console.error("Error al obtener los estudiantes:", error);
    });
  }, []);

  return (
    <div className="p-6">
      <h1 className="text-xl font-bold mb-4">Estudiantes</h1>
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {Array.isArray(students) && students.map((s) => (
          <StudentCard key={s.id} student={s} />
        ))}
      </div>
    </div>
  );
}
