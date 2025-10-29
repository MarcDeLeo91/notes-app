import axios from "axios";
import { authHeader } from "./auth.service";

const API_URL = import.meta.env.VITE_API_URL + "/inscripciones";

export const getInscripciones = () => axios.get(API_URL, { headers: authHeader() });
export const createInscripcion = (data) =>
  axios.post(API_URL, data, { headers: authHeader() });
export const deleteInscripcion = (id) =>
  axios.delete(`${API_URL}/${id}`, { headers: authHeader() });

const inscripcionService = {
  getInscripciones,
  createInscripcion,
  deleteInscripcion,
};

export default inscripcionService;
