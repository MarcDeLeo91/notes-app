import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { logout, getToken } from '../services/auth.service';

export default function NavBar(){
  const nav = useNavigate();
  const token = getToken();

  const doLogout = () => {
    logout();
    nav('/login');
  };

  return (
    <nav style={{ padding: 12, borderBottom: '1px solid #ddd' }}>
      <Link to="/dashboard" style={{ marginRight: 12 }}>Dashboard</Link>
      <Link to="/ai" style={{ marginRight: 12 }}>AI</Link>
      {!token ? (
        <>
          <Link to="/login" style={{ marginRight: 12 }}>Login</Link>
          <Link to="/register">Register</Link>
        </>
      ) : (
        <button onClick={doLogout}>Cerrar sesión</button>
      )}
    </nav>
  );
}
