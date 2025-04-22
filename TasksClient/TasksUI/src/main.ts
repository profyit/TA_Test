import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideNativeDateAdapter } from '@angular/material/core'; 

import { AppComponent } from './app/app.component';
import { appRoutes } from './app/app.routes'; 
import { authInterceptor } from './app/core/interceptors/auth.interceptor'; 

bootstrapApplication(AppComponent, {
  providers: [
    provideRouter(appRoutes),
    provideHttpClient(withInterceptors([authInterceptor])), 
    provideAnimations(), 
    provideNativeDateAdapter() 
  ]
}).catch(err => console.error(err));
