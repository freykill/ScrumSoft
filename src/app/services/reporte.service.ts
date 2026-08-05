import { inject, Injectable } from '@angular/core';
import { ArchivoDescargado, GenericService, UrlServices } from '../common/services';
import { FormatoDeReporte } from '../enums';
import { TableroFiltros } from '../models';

/**
 * Reporte del proyecto en PDF o Excel.
 *
 * Acepta los MISMOS filtros que el tablero a proposito: si el usuario esta
 * mirando la pantalla filtrada, el archivo tiene que salir con lo mismo que
 * ve. Bajarse el proyecto entero cuando en pantalla hay tres tareas seria
 * desconcertante.
 *
 * Respuestas que documenta el backend:
 *   GET  200 el binario   400 falta el formato   403 no eres del proyecto
 */
@Injectable({ providedIn: 'root' })
export class ReporteService {

    private readonly genericService = inject(GenericService);
    private readonly urlService = inject(UrlServices);

    descargar(
        idProyecto: string,
        formato: FormatoDeReporte,
        filtros?: TableroFiltros
    ): Promise<ArchivoDescargado> {
        return this.genericService.descargarArchivo(
            this.urlService.urlProyectoReporte(idProyecto),
            {
                // `formato` es obligatorio; los filtros solo si tienen valor.
                formato,
                idResponsable: filtros?.idResponsable || undefined,
                prioridad: filtros?.prioridad || undefined,
                texto: filtros?.texto?.trim() || undefined
            }
        );
    }
}
