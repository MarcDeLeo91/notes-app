import axios from "axios";
import { authHeader } from "./auth.service";

const API_URL = import.meta.env.VITE_API_URL + "/ai";

export const executePrompt = async (prompt) => {
  const res = await axios.post(`${API_URL}/execute`, { prompt }, { headers: authHeader() });
  return res.data;
};
