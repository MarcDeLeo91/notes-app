import React, { useState } from 'react';
import { register } from '../services/auth.service';
import { useNavigate } from 'react-router-dom';

export default function Register(){
  const [email,setEmail]=useState('');
  const [password,setPassword]=useState('');
  const [error,setError]=useState('');
  const nav = useNavigate();

  const submit = async (e) => {
    e.preventDefault();
    setError('');
    try {
      await register({ email, password });
      nav('/login');
    } catch (err) {
      setError(err.response?.data?.message || err.message || 'Error');
    }
  };

  return (
    <div style={{ maxWidth: 480, margin: '0 auto' }}>
      <h2>Registro</h2>
      <form onSubmit={submit}>
        <div>
          <label>Email</label><br/>
          <input value={email} onChange={e=>setEmail(e.target.value)} required placeholder="Email"/>
        </div>
        <div>
          <label>Contraseña</label><br/>
          <input type="password" value={password} onChange={e=>setPassword(e.target.value)} required placeholder="Contraseña"/>
        </div>
        {error && <div style={{ color: 'red', marginTop: 8 }}>{error}</div>}
      </form>
    </div>
  );
}
