import axios from "axios";
import { authHeader } from "./auth.service";

const API_URL = import.meta.env.VITE_API_URL + "/cursos";

export const getCursos = () => axios.get(API_URL, { headers: authHeader() });
export const createCurso = (data) => axios.post(API_URL, data, { headers: authHeader() });
export const updateCurso = (id, data) =>
  axios.put(`${API_URL}/${id}`, data, { headers: authHeader() });
export const deleteCurso = (id) =>
  axios.delete(`${API_URL}/${id}`, { headers: authHeader() });

const cursoService = {
  getCursos,
  createCurso,
  updateCurso,
  deleteCurso,
};

export default cursoService;
