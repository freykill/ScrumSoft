# ScrumSoft — SPA

Cliente web de ScrumSoft: gestion de proyectos agiles con tablero kanban en
tiempo real. Angular 17 con NgModules, PrimeNG 17 sobre la plantilla Sakai y
SCSS.

> Esta carpeta es solo el frontend. Para levantar la solucion completa (base de
> datos, API y SPA) con Docker, y para las decisiones arquitectonicas del
> ejercicio, ver el **[README de la raiz](../README.md)**.

---

## Levantar en local

Requiere Node 20 y la API corriendo en `https://localhost:7086` (ver
`scrumSoft-back`).

```bash
npm install
npm start          # http://localhost:4200
```

Usuarios precargados por la migracion semilla del backend:

| Correo | Contrasena | Rol |
|---|---|---|
| admin@scrumsoft.com | Admin123* | Administrador |
| miembro@scrumsoft.com | Miembro123* | Miembro |

En Docker no hace falta nada de esto: nginx sirve la SPA y hace de proxy hacia
la API, asi que las dos comparten origen.

## Comandos

| Comando | Que hace |
|---|---|
| `npm start` | Servidor de desarrollo en el puerto 4200 |
| `npm run build` | Compila; `-- --configuration=production` para el bundle de despliegue |
| `npm test` | Pruebas en modo watch, con el reporte de Jasmine en el navegador |
| `npm run test:ci` | Las 28 pruebas de una pasada, en Chrome headless |

## Configuracion por ambiente

Ningun componente ni servicio arma una url. La base del backend sale de
`src/environments/` y los endpoints estan todos en `common/services/url.services.ts`.

| Archivo | `urlBackend` | Cuando se usa |
|---|---|---|
| `environment.ts` | `https://localhost:7086` | desarrollo local |
| `environment.prod.ts` | *(vacio)* | build de produccion |

En produccion va vacio a proposito: las peticiones salen relativas
(`/api/v1/...`, `/hubs/tablero`) contra el mismo origen desde el que se
descargo la aplicacion. Eso elimina CORS y permite desplegar en cualquier host
o puerto sin recompilar.

---

## Como esta organizado

Separacion por capas: lo transversal en `common/`, el negocio en `services/`, y
las pantallas sin saber de HTTP.

```
src/app/
├── common/
│   ├── guards/          authGuard, invitadoGuard
│   ├── interceptors/    token en cada peticion y manejo del 401
│   └── services/        AuthService (sesion), GenericService (HTTP), UrlServices
├── config/              constantes y tema por defecto
├── enums/               enums del backend y opciones de los desplegables
├── models/              *Dto lo que llega, *Comando lo que se manda, *Filtros
├── services/            un servicio por area, sobre GenericService + UrlServices
├── layout/              plantilla Sakai
└── pages/
    ├── auth/            login
    └── business/        proyectos, columnas, miembros, tablero, usuarios
```

**Contenedor / presentacionales.** Cada pantalla tiene un contenedor con el
estado y las llamadas, y unos hijos que solo reciben por `@Input` y avisan por
`@Output`. Las listas van en `OnPush`, asi que nunca se muta un array: se
reemplaza. Referencia viva en `pages/business/usuarios/`.

**La sesion solo la toca `AuthService`.** Es el unico archivo que usa
`localStorage` / `sessionStorage`; el guard, el interceptor y los componentes
leen de el. Con "recordarme" la sesion va a `localStorage` y sin el a
`sessionStorage`, por eso nadie mas puede asumir donde esta.

**Un solo interceptor.** Adjunta el token y, ante un 401, cierra la sesion y
manda al login. El guard esta puesto sobre el layout y no sobre cada pantalla:
todo lo que cuelga de ahi es privado, asi que no hay forma de olvidarlo al
anadir una ruta.

---

## El tablero

Es la pantalla con mas cosas en juego, y donde mirar primero:

**Arrastre.** El CDK reporta un indice, pero el backend no posiciona por indice
sino por vecinos: entre que tarea queda arriba y cual abajo (`calcularVecinos`
en `utilities/orden.util.ts`). Asi el servidor asigna el orden con huecos sin
tocar el resto de la columna. `tablero-columna` es el unico que sabe del CDK y
traduce el evento; el contenedor solo entiende de tareas y columnas.

**Actualizacion optimista.** La tarjeta se mueve en pantalla y se guarda
despues. Si el servidor rechaza, vuelve visiblemente a su sitio y sale un aviso
con el motivo. Cuando acepta, se aplica el `orden` que devolvio el servidor y
no el que se calculo aqui.

**Tiempo real (SignalR).** `TableroRealtimeService` abre `/hubs/tablero` con el
mismo token de sesion, y se suscribe al grupo del proyecto; al cambiar de
tablero se sale del anterior, para no recibir eventos de tableros que no se
estan mirando. El componente cierra la conexion en `ngOnDestroy`.

Los eventos del hub se aplican con los mismos metodos que las respuestas del
REST, y eso hace que el eco sea inofensivo: quien mueve una tarjeta recibe
tambien su propio evento, pero aplicarlo dos veces da el mismo resultado. Con
filtros puestos no se aplica a ciegas: se recarga, porque el filtro vive en
este navegador y el servidor es quien sabe que tareas me tocan.

---

## Pruebas

Karma + Jasmine, **28 pruebas**. Cada `.spec.ts` esta junto a lo que prueba:

| Archivo | Que cubre |
|---|---|
| `pages/business/tablero/tablero.component.spec.ts` | el arrastre completo: vecinos que se mandan al servidor, pintado optimista, reversion al fallar y el orden que asigna el backend |
| `pages/business/columnas/columnas.component.spec.ts` | reordenar columnas: ids en su orden final, renumeracion local y recarga si el guardado falla |
| `utilities/orden.util.spec.ts` | los casos borde del calculo: columna vacia, indices fuera de rango, mover dentro de la propia columna |

El calculo de la nueva posicion al reordenar se prueba desde el componente y no
solo contra la funcion suelta: aislada siempre acierta, lo que se rompe de
verdad es la coordinacion entre calcular, pintar y deshacer.

```bash
npm run test:ci
```

---

## Convenciones

- **NgModules, no standalone.** Generar con `--standalone=false`.
- **Un modulo por area**, no por pantalla. Sin `SharedModule`: cada `@NgModule`
  importa lo que usa, uno por uno.
- **Codigo y comentarios en espanol.** Los comentarios explican *por que* se
  hizo asi, no *que* hace la linea.
- **Colores por variable de tema** (`surface-card`, `text-900`, …), nunca fijos:
  el engranaje del topbar cambia a tema oscuro y los colores fijos se rompen.

---

La plantilla base es [Sakai NG](https://github.com/primefaces/sakai-ng) de
PrimeTek, MIT (ver `LICENSE.md`).
