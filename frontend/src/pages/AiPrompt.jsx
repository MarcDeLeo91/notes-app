import React, { useState } from 'react';
import { executePrompt } from '../services/ai.service';

const PROMPTS = [
  'crear nota rápida',
  'crear nota: {titulo} | {contenido}',
  'modificar nota {id}: {titulo} | {contenido}',
  'eliminar nota {id}',
  'marcar favorita {id}',
  'archivar nota {id}',
  'desarchivar nota {id}',
  'agregar etiqueta {id}: {etiqueta}',
  'buscar notas: {texto}',
  'borrar todas las notas archivadas'
];

export default function AiPrompt(){
  const [prompt,setPrompt] = useState('');
  const [result,setResult] = useState(null);
  const [loading,setLoading] = useState(false);

  const send = async () => {
    if(!prompt) return;
    setLoading(true);
    setResult(null);
    try {
      const r = await executePrompt(prompt);
      setResult(r.data);
    } catch (err) {
      setResult({ error: err.response?.data ?? err.message });
    } finally { setLoading(false); }
  };

  return (
    <div style={{ maxWidth: 800 }}>
      <h2>AI Prompts</h2>
      <select onChange={e=>setPrompt(e.target.value)} value={prompt}>
        <option value="">-- seleccionar --</option>
        {PROMPTS.map(p => <option key={p} value={p}>{p}</option>)}
      </select>
      <div>
        <textarea rows={4} style={{ width: '100%' }} value={prompt} onChange={e=>setPrompt(e.target.value)} />
      </div>
      <button onClick={send} disabled={!prompt || loading}>Ejecutar prompt</button>
      <pre style={{ whiteSpace:'pre-wrap', marginTop:12 }}>{JSON.stringify(result, null, 2)}</pre>
    </div>
  );
}
