// Ambiente de produccion: la SPA se publica detras de nginx, que sirve los
// estaticos y ademas hace de proxy hacia la api (ver nginx.conf).
//
// urlBackend va vacio a proposito: asi las peticiones salen relativas
// (/api/v1/..., /hubs/tablero) contra el mismo origen desde el que se descargo
// la aplicacion. Eso evita CORS por completo y hace que el despliegue funcione
// en cualquier host o puerto sin volver a compilar.
export const environment = {
    production: true,
    name: 'prod',
    urlBackend: ''
};
