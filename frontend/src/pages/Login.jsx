import { login } from '../services/auth.service';
import { useNavigate } from 'react-router-dom';
import { useState } from 'react';

const Login = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const navigate = useNavigate();

    const submit = async (e) => {
        e.preventDefault();
        setError(''); // Limpiar errores previos

        // Validaciones adicionales
        if (!email.trim() || !password.trim()) {
            setError('El correo electrónico y la contraseña son obligatorios.');
            return;
        }
        if (!/\S+@\S+\.\S+/.test(email)) {
            setError('El correo electrónico no tiene un formato válido.');
            return;
        }
        if (password.length < 6) {
            setError('La contraseña debe tener al menos 6 caracteres.');
            return;
        }

        try {
            const response = await login(email, password);
            console.log('Respuesta del servidor:', response); // Depuración
            if (response && response.token) {
                localStorage.setItem('token', response.token); // Guarda el token en localStorage
                navigate('/dashboard'); // Redirige al dashboard
            } else {
                throw new Error('Respuesta inesperada del servidor');
            }
        } catch (err) {
            console.error('Error en login:', err);
            setError(
                err.response?.data?.message || 
                'Error al iniciar sesión. Verifica tus credenciales o intenta nuevamente.'
            );
        }
    };

    return (
        <div style={{ maxWidth: '400px', margin: '0 auto', padding: '20px' }}>
            <h2>Iniciar Sesión</h2>
            <form onSubmit={submit}>
                <div style={{ marginBottom: '10px' }}>
                    <label htmlFor="email">Email</label>
                    <input
                        id="email"
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        placeholder="Email"
                        required
                        style={{ width: '100%', padding: '8px', marginTop: '5px' }}
                    />
                </div>
                <div style={{ marginBottom: '10px' }}>
                    <label htmlFor="password">Contraseña</label>
                    <input
                        id="password"
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        placeholder="Contraseña"
                        required
                        style={{ width: '100%', padding: '8px', marginTop: '5px' }}
                    />
                </div>
                <button type="submit" style={{ width: '150px', padding: '10px', backgroundColor: '#007BFF', color: '#FFF', border: 'none', borderRadius: '5px' }}>
                    Entrar
                </button>
                {error && <p style={{ color: 'red', marginTop: '10px' }}>{error}</p>}
            </form>
        </div>
    );
};

export default Login;