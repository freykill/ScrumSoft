/**
 * Configuracion visual por defecto de la app.
 * Es el estado inicial del LayoutService; el usuario puede cambiarlo en caliente
 * desde el panel de configuracion de Sakai (el engranaje del topbar).
 *
 * Los COLORES no estan aqui. El primario vive en el tema:
 *   src/assets/layout/styles/theme/lara-light-scrum/theme.css
 * y los secundarios de marca en src/brand.scss. Ver brand.scss para el porque.
 */
export const TEMA_DEFECTO = {
    /**
     * Tema base de PrimeNG. Tiene que coincidir con el href del link #theme-css
     * de index.html, si no el cambio de tema en caliente se rompe.
     */
    theme: 'lara-light-scrum',
    colorScheme: 'light',
    inputStyle: 'outlined',
    menuMode: 'static',
    ripple: true,
    /** Tamano base en px del html. Escala toda la interfaz. */
    scale: 14
};
