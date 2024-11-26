import React from 'react';
import ReactDOM from 'react-dom';
import Dashboard from './dashboard'; // Asegúrate de que la ruta sea correcta

ReactDOM.render(
    <React.StrictMode>
        <Dashboard />
    </React.StrictMode>,
    document.getElementById('dashboard-root')
);