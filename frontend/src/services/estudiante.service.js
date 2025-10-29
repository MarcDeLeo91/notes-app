import axios from "axios";
import { authHeader } from "./auth.service";

const API_URL = import.meta.env.VITE_API_URL + "/estudiantes";

export const getEstudiantes = () => axios.get(API_URL, { headers: authHeader() });
export const createEstudiante = (data) => axios.post(API_URL, data, { headers: authHeader() });
export const updateEstudiante = (id, data) =>
  axios.put(`${API_URL}/${id}`, data, { headers: authHeader() });
export const deleteEstudiante = (id) =>
  axios.delete(`${API_URL}/${id}`, { headers: authHeader() });

const estudianteService = {
  getEstudiantes,
  createEstudiante,
  updateEstudiante,
  deleteEstudiante,
};

export default estudianteService;
