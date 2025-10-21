// src/services/auth.service.js
import axios from 'axios';
const API = import.meta.env.VITE_API_URL;

// Registro de usuario
export const register = async (data) => {
  try {
    const response = await axios.post(`${API}/auth/register`, data);
    return response.data;
  } catch (error) {
    console.error('Error during registration:', error.response?.data || error.message);
    throw error;
  }
};

// Inicio de sesión
export const login = async (email, password) => {
  try {
    const response = await axios.post(`${API}/auth/login`, { email, password });
    if (response.data?.token) {
      // Guardar token y configurar axios globalmente
      localStorage.setItem('token', response.data.token);
      axios.defaults.headers.common['Authorization'] = `Bearer ${response.data.token}`;
    }
    return response.data;
  } catch (error) {
    console.error('Error during login:', error.response?.data || error.message);
    throw error;
  }
};

// Cerrar sesión
export const logout = () => {
  localStorage.removeItem('token');
  delete axios.defaults.headers.common['Authorization'];
};

// Obtener el token almacenado
export const getToken = () => {
  return localStorage.getItem('token');
};

// Generar encabezado de autorización (opcional, para fetch directos)
export const authHeader = () => {
  const token = getToken();
  return token ? { Authorization: `Bearer ${token}` } : {};
};
