import React from "react";
import { Link, useNavigate } from "react-router-dom";

export default function NavBar() {
  const navigate = useNavigate();

  const token = localStorage.getItem("token");

  const handleLogout = () => {
    localStorage.removeItem("token");
    navigate("/login");
  };

  return (
    <nav className="bg-indigo-700 text-white px-6 py-3 flex justify-start items-center">
      {token ? (
        <>
          <Link to="/students" className="hover:text-gray-300">
            Estudiantes
          </Link>
          <Link to="/courses" className="hover:text-gray-300">
            Cursos
          </Link>
          <Link to="/enrollments" className="hover:text-gray-300">
            Inscripciones
          </Link>
          <Link to="/grades" className="hover:text-gray-300">
            Notas
          </Link>
          <Link to="/ai" className="hover:text-gray-300">
            Asistente IA
          </Link>
          <button onClick={handleLogout} className="bg-red-500 px-3 py-1 rounded hover:bg-red-600" style={{ width: '150px' }}>
            Cerrar sesión
          </button>
        </>
      ) : (
        <div className="flex gap-2">
          <button
            onClick={() => navigate("/login")}
            className="bg-blue-500 px-3 py-1 rounded hover:bg-blue-600"
            style={{ width: '150px' }}
          >
            Login
          </button>
          <button
            onClick={() => navigate("/register")}
            className="bg-green-500 px-3 py-1 rounded hover:bg-green-600"
            style={{ width: '150px' }}
          >
            Registrar
          </button>
        </div>
      )}
    </nav>
  );
}
