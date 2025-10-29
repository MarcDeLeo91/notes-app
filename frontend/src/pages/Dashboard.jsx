import React, { useEffect, useState } from "react";
import axios from "axios";

/**
 * Dashboard.jsx
 * - Muestra métricas resumidas (students, courses, enrollments, notes)
 * - Botones rápidos para navegar a páginas importantes
 * - Mini-widget para probar un prompt corto al agente IA
 *
 * Requisitos:
 * - axios.defaults.baseURL debe apuntar al backend (VITE_API_URL)
 * - Endpoints esperados:
 *     GET /estudiantes
 *     GET /cursos
 *     GET /inscripciones
 *     GET /notas
 *     POST /ai/execute   (body: { prompt, userId? })
 */

export default function Dashboard() {
  const [counts, setCounts] = useState({
    estudiantes: 0,
    cursos: 0,
    inscripciones: 0,
    notas: 0,
  });
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [quickPrompt, setQuickPrompt] = useState("");
  const [aiResponse, setAiResponse] = useState(null);

  const token = localStorage.getItem("token");

  const loadCounts = async () => {
    setLoading(true);
    setError("");
    try {
      const [estRes, curRes, insRes, notaRes] = await Promise.all([
        axios.get("/estudiantes"),
        axios.get("/cursos"),
        axios.get("/inscripciones"),
        axios.get("/notas"),
      ]);

      setCounts({
        estudiantes: Array.isArray(estRes.data) ? estRes.data.length : 0,
        cursos: Array.isArray(curRes.data) ? curRes.data.length : 0,
        inscripciones: Array.isArray(insRes.data) ? insRes.data.length : 0,
        notas: Array.isArray(notaRes.data) ? notaRes.data.length : 0,
      });
    } catch (err) {
      console.error("Error cargando métricas:", err);
      setError("No se pudieron cargar las métricas. Verifica que el backend esté corriendo.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (token) {
      loadCounts();
    }
  }, [token]);

  const handleQuickPrompt = async () => {
    if (!quickPrompt.trim()) {
      setAiResponse({ error: "Escribe un prompt primero." });
      return;
    }
    setAiResponse({ loading: true });
    try {
      const res = await axios.post("/ai/execute", { prompt: quickPrompt });
      setAiResponse({ ok: true, data: res.data });
      setTimeout(loadCounts, 800);
    } catch (err) {
      console.error("Error ejecutando prompt:", err);
      const message = err.response?.data?.message || err.message || "Error desconocido";
      setAiResponse({ error: message });
    }
  };

  if (!token) {
    return <div className="p-6">Por favor inicia sesión para acceder al Dashboard.</div>;
  }

  return (
    <div className="p-6">
      <h1 className="text-2xl font-bold mb-4">Dashboard</h1>

      {error && (
        <div className="mb-4 p-3 bg-red-100 text-red-800 rounded">
          {error}
        </div>
      )}

      <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
        <div className="card p-4">
          <div className="text-sm text-gray-500">Estudiantes</div>
          <div className="text-3xl font-semibold">{loading ? "..." : counts.estudiantes}</div>
        </div>

        <div className="card p-4">
          <div className="text-sm text-gray-500">Cursos</div>
          <div className="text-3xl font-semibold">{loading ? "..." : counts.cursos}</div>
        </div>

        <div className="card p-4">
          <div className="text-sm text-gray-500">Inscripciones</div>
          <div className="text-3xl font-semibold">{loading ? "..." : counts.inscripciones}</div>
        </div>

        <div className="card p-4">
          <div className="text-sm text-gray-500">Notas</div>
          <div className="text-3xl font-semibold">{loading ? "..." : counts.notas}</div>
        </div>
      </div>

      <div className="flex flex-col h-[500px] border rounded p-4 bg-gray-50">
        <div className="flex-1 overflow-y-auto mb-4">
          {aiResponse?.ok && (
            <div className="p-2 bg-green-100 rounded mb-2">
              <p className="text-sm">{aiResponse.data}</p>
            </div>
          )}
          {aiResponse?.error && (
            <div className="p-2 bg-red-100 rounded mb-2">
              <p className="text-sm">Error: {aiResponse.error}</p>
            </div>
          )}
        </div>
        <div className="flex gap-2">
          <input
            value={quickPrompt}
            onChange={(e) => setQuickPrompt(e.target.value)}
            placeholder="Escribe aquí un prompt..."
            className="flex-1 p-2 border rounded"
          />
          <button onClick={handleQuickPrompt} className="px-3 py-1 rounded bg-blue-600 text-white text-sm" style={{ width: '150px' }}>
            Enviar
          </button>
        </div>
      </div>
    </div>
  );
}
