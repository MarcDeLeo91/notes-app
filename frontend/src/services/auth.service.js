import axios from 'axios';

const API = import.meta.env.VITE_API_URL;

// Registro de usuario
export const register = async (data) => {
  try {
    const response = await axios.post(`${API}/auth/register`, data);
    return response.data;
  } catch (error) {
    console.error('Error durante el registro:', error.response?.data?.message || error.message);
    throw new Error(error.response?.data?.message || 'Error en el registro');
  }
};

// Inicio de sesión
export const login = async (email, password) => {
  try {
    const response = await axios.post(`${API}/auth/login`, { email, password });
    if (response.data?.token) {
      // Guardar token y configurar axios globalmente
      saveToken(response.data.token);
    } else {
      throw new Error('Token no recibido en la respuesta del servidor');
    }
    return response.data;
  } catch (error) {
    console.error('Error durante el inicio de sesión:', error.response?.data?.message || error.message);
    throw new Error(error.response?.data?.message || 'Error en el inicio de sesión');
  }
};

// Cerrar sesión
export const logout = () => {
  try {
    localStorage.removeItem('token');
    delete axios.defaults.headers.common['Authorization'];
    console.log('Sesión cerrada correctamente.');
  } catch (error) {
    console.error('Error al cerrar sesión:', error.message);
  }
};

// Guardar el token en localStorage y configurar axios
const saveToken = (token) => {
  try {
    localStorage.setItem('token', token);
    axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
    console.log('Token guardado y configurado en axios.');
  } catch (error) {
    console.error('Error al guardar el token:', error.message);
  }
};

// Obtener el token almacenado
export const getToken = () => {
  try {
    return localStorage.getItem('token');
  } catch (error) {
    console.error('Error al obtener el token:', error.message);
    return null;
  }
};

// Generar encabezado de autorización (opcional, para fetch directos)
export const authHeader = () => {
  const token = getToken();
  return token ? { Authorization: `Bearer ${token}` } : {};
};