import axios from 'axios';
import { authHeader } from './auth.service';
const API = import.meta.env.VITE_API_URL;

export const executePrompt = (prompt) => axios.post(`${API}/ai/execute`, { prompt }, authHeader());
