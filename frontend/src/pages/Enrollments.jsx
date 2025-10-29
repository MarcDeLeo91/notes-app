import React, { useEffect, useState } from "react";
import inscripcionService from "../services/inscripcion.service";

export default function Enrollments() {
  const [enrollments, setEnrollments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const fetchEnrollments = async () => {
      setLoading(true);
      setError("");
      try {
        const response = await inscripcionService.getInscripciones();
        const data = response.data?.$values || [];
        setEnrollments(data);
      } catch (err) {
        console.error("Error fetching enrollments:", err);
        setError("No se pudieron cargar las inscripciones. Intenta nuevamente más tarde.");
      } finally {
        setLoading(false);
      }
    };

    fetchEnrollments();
  }, []);

  if (loading) {
    return <div className="p-6">Cargando inscripciones...</div>;
  }

  if (error) {
    return <div className="p-6 text-red-500">{error}</div>;
  }

  return (
    <div className="p-6">
      <h1 className="text-xl font-bold mb-4">Inscripciones</h1>
      <table className="table-auto w-full border">
        <thead>
          <tr className="bg-gray-200">
            <th>Estudiante</th>
            <th>Curso</th>
          </tr>
        </thead>
        <tbody>
          {enrollments.map((e, index) => (
            <tr key={`${e.id}-${index}`} className="border-b">
              <td className="p-2">{e.estudiante?.nombre || "N/A"}</td>
              <td className="p-2">{e.curso?.nombre || "N/A"}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
