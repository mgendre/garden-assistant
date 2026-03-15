import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'companions', pathMatch: 'full' },
  {
    path: 'companions',
    loadComponent: () => import('./features/companions/companions').then(m => m.Companions)
  }
];
