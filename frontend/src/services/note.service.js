// src/services/note.service.js
import axios from 'axios';
import { authHeader } from './auth.service';
const API = import.meta.env.VITE_API_URL;

// Usamos axios.defaults (configurado en login / main.jsx). 
// authHeader() queda disponible para casos específicos (fetch directo).

export const getNotes = () => axios.get(`${API}/notes`, { headers: authHeader() });
export const getNote = (id) => axios.get(`${API}/notes/${id}`, { headers: authHeader() });
export const createNote = (note) => axios.post(`${API}/notes`, note, { headers: authHeader() });
export const updateNote = (id, note) => axios.put(`${API}/notes/${id}`, note, { headers: authHeader() });
export const deleteNote = (id) => axios.delete(`${API}/notes/${id}`, { headers: authHeader() });
