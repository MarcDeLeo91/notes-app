import React, { useEffect, useState } from "react";
import notaService from "../services/nota.service";

export default function Grades() {
  const [grades, setGrades] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const fetchGrades = async () => {
      setLoading(true);
      setError("");
      try {
        const response = await notaService.getNotas();
        const data = response.data?.$values || [];
        setGrades(data);
      } catch (err) {
        console.error("Error fetching grades:", err);
        setError("No se pudieron cargar las notas. Intenta nuevamente más tarde.");
      } finally {
        setLoading(false);
      }
    };

    fetchGrades();
  }, []);

  if (loading) {
    return <div className="p-6">Cargando notas...</div>;
  }

  if (error) {
    return <div className="p-6 text-red-500">{error}</div>;
  }

  return (
    <div className="p-6">
      <h1 className="text-xl font-bold mb-4">Notas Académicas</h1>
      <table className="table-auto w-full border">
        <thead>
          <tr className="bg-gray-200">
            <th>Estudiante</th>
            <th>Curso</th>
            <th>Nota</th>
          </tr>
        </thead>
        <tbody>
          {grades.map((n, index) => (
            <tr key={`${n.id}-${index}`} className="border-b">
              <td className="p-2">{n.estudiante?.nombre || "N/A"}</td>
              <td className="p-2">{n.curso?.nombre || "N/A"}</td>
              <td className="p-2">{n.valor || "N/A"}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
