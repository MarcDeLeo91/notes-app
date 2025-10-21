import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { login } from '../services/auth.service';

export default function Login() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const nav = useNavigate();

  const submit = async (e) => {
    e.preventDefault();
    setError('');
    try {
      const response = await login(email, password); // Asegúrate de que `login` devuelva el token
      const token = response.data.token; // Ajusta según la estructura de la respuesta
      localStorage.setItem('token', token); // Guarda el token en localStorage
      nav('/dashboard'); // Redirige al dashboard
    } catch (err) {
      setError(err.response?.data?.message || err.message || 'Error');
    }
  };

  return (
    <div style={{ maxWidth: 480, margin: '0 auto' }}>
      <h2>Login</h2>
      <form onSubmit={submit}>
        <div>
          <label>Email</label><br/>
          <input value={email} onChange={e=>setEmail(e.target.value)} required />
        </div>
        <div>
          <label>Contraseña</label><br/>
          <input type="password" value={password} onChange={e=>setPassword(e.target.value)} required />
        </div>
        <button type="submit">Ingresar</button>
        {error && <div style={{ color: 'red', marginTop: 8 }}>{error}</div>}
      </form>
    </div>
  );
}