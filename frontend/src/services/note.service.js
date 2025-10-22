// src/services/note.service.js
import axios from 'axios';
const API = import.meta.env.VITE_API_URL;

// Recuperar el token desde localStorage y configurar el encabezado Authorization
const getAuthHeaders = () => {
  const token = localStorage.getItem('token');
  return token ? { Authorization: `Bearer ${token}` } : {};
};

// Obtener todas las notas
export const getNotes = async () => {
  try {
    const response = await axios.get(`${API}/notes`, { headers: getAuthHeaders() });
    return response.data;
  } catch (error) {
    console.error('Error al obtener las notas:', error.response?.data || error.message);
    throw error;
  }
};

// Obtener una nota por ID
export const getNote = async (id) => {
  try {
    const response = await axios.get(`${API}/notes/${id}`, { headers: getAuthHeaders() });
    return response.data;
  } catch (error) {
    console.error(`Error al obtener la nota con ID ${id}:`, error.response?.data || error.message);
    throw error;
  }
};

// Crear una nueva nota
export const createNote = async (note) => {
  try {
    const response = await axios.post(`${API}/notes`, note, { headers: getAuthHeaders() });
    return response.data;
  } catch (error) {
    console.error('Error al crear la nota:', error.response?.data || error.message);
    throw error;
  }
};

// Actualizar una nota por ID
export const updateNote = async (id, note) => {
  try {
    const response = await axios.put(`${API}/notes/${id}`, note, { headers: getAuthHeaders() });
    return response.data;
  } catch (error) {
    console.error(`Error al actualizar la nota con ID ${id}:`, error.response?.data || error.message);
    throw error;
  }
};

// Eliminar una nota por ID
export const deleteNote = async (id) => {
  try {
    const response = await axios.delete(`${API}/notes/${id}`, { headers: getAuthHeaders() });
    return response.data;
  } catch (error) {
    console.error(`Error al eliminar la nota con ID ${id}:`, error.response?.data || error.message);
    throw error;
  }
};