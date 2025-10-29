import axios from "axios";
import { authHeader } from "./auth.service";

const API_URL = import.meta.env.VITE_API_URL + "/notas";

export const getNotas = () => axios.get(API_URL, { headers: authHeader() });
export const createNota = (data) => axios.post(API_URL, data, { headers: authHeader() });
export const updateNota = (id, data) =>
  axios.put(`${API_URL}/${id}`, data, { headers: authHeader() });
export const deleteNota = (id) =>
  axios.delete(`${API_URL}/${id}`, { headers: authHeader() });

const notaService = {
  getNotas,
  createNota,
  updateNota,
  deleteNota,
};

export default notaService;
