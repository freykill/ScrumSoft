import { NgModule } from '@angular/core';
import { HashLocationStrategy, LocationStrategy } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { httpInterceptorProviders } from './common/interceptors';
import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { AppLayoutModule } from './layout/app.layout.module';
import { NotfoundComponent } from './pages/notfound/notfound.component';

@NgModule({
    declarations: [
        AppComponent, NotfoundComponent
    ],
    imports: [
        AppRoutingModule,
        AppLayoutModule,
        HttpClientModule,
        ToastModule
    ],
    providers: [
        // Rutas con # : el servidor solo recibe "/", asi el F5 y los links directos
        // funcionan en cualquier hosting sin configurar fallback a index.html
        { provide: LocationStrategy, useClass: HashLocationStrategy },
        // Instancia unica para toda la app: el <p-toast> de app.component.html
        // escucha esta y por eso los toast funcionan desde cualquier componente
        MessageService,
        httpInterceptorProviders
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
