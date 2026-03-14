import { ApplicationConfig, APP_INITIALIZER, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { routes } from './app.routes';
import { AuthClient, GardensClient, PlantAssociationsClient, PlantsClient } from './api/garden-assistant-api';
import { environment } from '../environments/environment';
import { AuthService } from './core/auth/auth.service';
import { authInterceptor } from './core/auth/auth.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideAnimationsAsync(),
    provideHttpClient(withInterceptors([authInterceptor])),
    {
      provide: APP_INITIALIZER,
      useFactory: (authService: AuthService) => () => authService.initialize(),
      deps: [AuthService],
      multi: true
    },
    {
      provide: AuthClient,
      useFactory: () => new AuthClient(environment.apiBaseUrl)
    },
    {
      provide: GardensClient,
      useFactory: (authService: AuthService) => new GardensClient(environment.apiBaseUrl, authService.createAuthFetch()),
      deps: [AuthService]
    },
    {
      provide: PlantsClient,
      useFactory: (authService: AuthService) => new PlantsClient(environment.apiBaseUrl, authService.createAuthFetch()),
      deps: [AuthService]
    },
    {
      provide: PlantAssociationsClient,
      useFactory: (authService: AuthService) => new PlantAssociationsClient(environment.apiBaseUrl, authService.createAuthFetch()),
      deps: [AuthService]
    }
  ]
};
