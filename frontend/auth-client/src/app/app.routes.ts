import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/presentation/home.component';
import { LoginComponent } from './features/auth/login/presentation/login.component';

export const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'home', component: HomeComponent },
  { path: 'auth/login', component: LoginComponent },
  { path: '**', redirectTo: '/home' }
];
