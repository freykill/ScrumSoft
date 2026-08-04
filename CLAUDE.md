# ScrumSoft — convenciones del frontend

Angular 17.0.5 con **NgModules** (no standalone), PrimeNG 17.2, PrimeFlex 3.3, plantilla Sakai NG.
Backend .NET 8 aparte, en `https://localhost:7086`.

---

## Reglas duras

1. **NgModules siempre.** Angular 17 genera standalone por defecto, asi que:
   `ng g c pages/business/x --standalone=false`
2. **Un modulo por area, no por pantalla.** `BusinessModule` declara todas las pantallas
   de negocio y sus hijos. Prohibido crear un modulo por componente o anidar modulos
   dentro de una carpeta de pantalla.
3. **Prohibido `SharedModule`.** Nada de modulos que reexporten una bolsa de otros
   modulos. Cada `@NgModule` importa lo que usa, uno por uno, en orden alfabetico.
   Es mas largo de leer pero se sabe de que depende cada cosa.
4. **Sin breadcrumb y sin componente de cabecera.** La cabecera de cada pantalla va
   escrita en su propio HTML (ver plantilla mas abajo).
5. **`localStorage` / `sessionStorage` solo dentro de `AuthService`.** Ningun otro
   archivo toca storage.
6. **Nada de URLs literales.** Todos los endpoints viven en `UrlServices`, y lo que
   cambia por ambiente en `src/environments/`. Ningun componente arma una URL.
7. **Codigo y comentarios en espanol, sin tildes.** Los comentarios explican *por que*
   se hizo asi, no *que* hace la linea.

---

## Donde va cada cosa

| Carpeta | Contenido |
|---|---|
| `common/guards` | Guards funcionales (`authGuard`) |
| `common/interceptors` | Un unico `HttpRequestInterceptor`: token + manejo de 401/403 |
| `common/services` | `AuthService` (sesion), `GenericService` (facade HTTP), `UrlServices` (endpoints), `LoaderService` |
| `config` | `app.constants.ts` (valores fijos), `theme.config.ts` (tema por defecto) |
| `enums` | Enums del backend, como strings. Aqui tambien las listas `OPCIONES_*` para dropdowns |
| `models` | Interfaces: `*Dto` lo que llega, `*Comando` lo que se manda, `*Filtros` el estado de los filtros |
| `services` | Servicios de negocio (`LoginService`, …). Usan `GenericService` + `UrlServices` |
| `pages/auth` | Login y pantallas publicas |
| `pages/business` | Pantallas de gestion, todas bajo el layout y el guard |
| `layout` | Plantilla Sakai. Tocar solo `app.menu.component.ts` para el menu |

Peticiones HTTP: siempre por `GenericService.genericCallServices<T>(METHODS.X, url, body?)`,
que devuelve `Promise<T>`. Existe `genericCallServices$` para los casos que necesitan
cancelacion (autocompletes con `switchMap`).

---

## Como se arma una pantalla de gestion

Patron **contenedor / presentacionales**. Referencia viva: `pages/business/usuarios/`.

```
pages/business/<entidad>/
├── <entidad>.component.ts|html          CONTENEDOR
├── <entidad>-list/
│   └── <entidad>-list.component.ts|html PRESENTACIONAL — la tabla
└── <entidad>-form/
    └── <entidad>-form.component.ts|html PRESENTACIONAL — el dialogo
```

Carpeta plana. Nada de un nivel `components/` intermedio.

| | Contenedor | Lista y formulario |
|---|---|---|
| Inyecta servicios | si | **no, nunca** |
| Guarda el estado | si | no |
| Sabe de la API | si | no |
| Recibe datos | de los servicios | por `@Input` |
| Avisa de acciones | — | por `@Output` |
| `ChangeDetection` | por defecto | `OnPush` en la lista |

### Contenedor

Tiene el estado y todas las decisiones:

```ts
@Component({
    selector: 'app-usuarios',
    templateUrl: './usuarios.component.html',
    // Instancia propia de la pantalla, no se cruza con otras.
    providers: [ConfirmationService]
})
export class UsuariosComponent implements OnInit {

    private usuarios: UsuarioDto[] = [];   // fuente de verdad
    usuariosVisibles: UsuarioDto[] = [];   // lo que ve la tabla, ya filtrado
    cargando = false;
    guardando = false;

    filtros: UsuarioFiltros = { busqueda: '', rol: null, estado: null };

    mostrarFormulario = false;
    usuarioEnEdicion: UsuarioDto | null = null;   // null = alta
```

Metodos: `aplicarFiltros()`, `limpiarFiltros()`, `nuevo()`, `editar()`,
`guardar()`, `confirmarEliminacion()` y un `eliminar()` privado.

### Lista

```ts
@Input() usuarios: UsuarioDto[] = [];
@Input() cargando = false;
@Output() editar = new EventEmitter<UsuarioDto>();
@Output() eliminar = new EventEmitter<UsuarioDto>();
```

Solo el `<p-table>`. Los helpers de presentacion (`severidadRol`, `esActivo`) van aqui,
no en el contenedor, porque son cosa de como se pinta.

### Formulario

```ts
@Input() usuario: UsuarioDto | null = null;   // null = alta
@Input() visible = false;
@Input() guardando = false;
@Output() visibleChange = new EventEmitter<boolean>();   // habilita [(visible)]
@Output() guardar = new EventEmitter<GuardarUsuarioComando>();
```

Tiene su `FormGroup` con `fb.nonNullable.group()` — la validacion si es asunto del
formulario — pero no persiste nada: emite el comando y el contenedor decide.

---

## Plantilla HTML del contenedor

```html
<!-- 1. Cabecera: titulo, descripcion y accion principal -->
<div class="surface-card border-round-xl border-1 surface-border p-4 mb-3
            flex flex-column md:flex-row md:align-items-center justify-content-between gap-3">
    <div class="flex-1">
        <h2 class="text-900 text-2xl font-bold m-0">Usuarios</h2>
        <p class="text-600 mt-2 mb-0">Administra quien tiene acceso a ScrumSoft.</p>
    </div>
    <div class="flex-shrink-0">
        <button pButton pRipple icon="pi pi-plus" label="Nuevo usuario" (click)="nuevo()"></button>
    </div>
</div>

<!-- 2. Tarjeta de contenido: barra de filtros + tabla -->
<div class="surface-card border-1 surface-border border-round-xl overflow-hidden">
    <div class="p-3 border-bottom-1 surface-border flex flex-column lg:flex-row gap-2
                lg:align-items-center justify-content-between">
        <span class="p-input-icon-left w-full lg:w-30rem">
            <i class="pi pi-search"></i>
            <input pInputText type="text" class="w-full" placeholder="Buscar..."
                   [(ngModel)]="filtros.busqueda" (ngModelChange)="aplicarFiltros()">
        </span>
        <div class="flex flex-column sm:flex-row gap-2">
            <!-- p-dropdown por cada filtro, siempre con [showClear]="true" -->
        </div>
    </div>

    <app-usuarios-list [usuarios]="usuariosVisibles" [cargando]="cargando"
                       (editar)="editar($event)"
                       (eliminar)="confirmarEliminacion($event)"></app-usuarios-list>
</div>

<!-- 3. Dialogos, siempre al final -->
<app-usuarios-form [(visible)]="mostrarFormulario" [usuario]="usuarioEnEdicion"
                   [guardando]="guardando" (guardar)="guardar($event)"></app-usuarios-form>

<p-confirmDialog [style]="{ width: '28rem' }"></p-confirmDialog>
```

**Clases de color:** siempre `surface-card`, `surface-border`, `surface-ground`,
`text-900`, `text-600`. Nunca `border-gray-200` ni colores fijos: el engranaje del
topbar cambia a tema oscuro y los colores fijos se rompen.

---

## Cosas que se rompen facil

**La lista es `OnPush`, asi que nunca se muta un array.** Al editar o eliminar se
devuelve un array nuevo con objetos nuevos, si no el hijo no repinta:

```ts
this.usuarios = this.usuarios.map(u => u.id === id ? { ...u, estadoRegistro: 'E' } : u);
```

**Los filtros escriben en un campo, no en un getter.** Un getter en la plantilla se
reevalua en cada ciclo de deteccion de cambios y devuelve un array nuevo cada vez, que
es justo lo que rompe el `OnPush` del hijo.

**El formulario se reinicia en `(onShow)` del `p-dialog`, no en un setter de `@Input`.**
Al abrir dos veces seguidas en modo alta, `usuario` sigue siendo `null` y Angular no
vuelve a disparar el setter: el formulario quedaria con lo de la vez anterior.

**`ConfirmationService` va en los `providers` del componente**, no del modulo, para que
el `<p-confirmDialog>` de esa plantilla resuelva la misma instancia.

**`MessageService` NO se provee por pantalla.** Ya esta a nivel raiz en `app.module.ts`
y el `<p-toast>` de `app.component.html` escucha esa instancia. Proveerlo otra vez hace
que los toast no salgan.

**Eliminar es borrado logico** (`estadoRegistro: 'A' | 'E'`), igual que en el backend.
El registro no se pierde, se marca.

---

## Al terminar una pantalla

- `npx ng build` limpio. Los diagnosticos del IDE mientras se edita suelen quedar
  desfasados; el build manda. Si sale un `Can't resolve` raro despues de mover
  archivos, `rm -rf .angular/cache` y compilar de nuevo.
- Menu en `layout/app.menu.component.ts`, ruta en `pages/business/business-routing.module.ts`.
- Commit propio, atomico y con mensaje descriptivo. No acumular varias pantallas
  en un commit.
