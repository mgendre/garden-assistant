import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'companions', pathMatch: 'full' },
  {
    path: 'companions',
    loadComponent: () => import('./features/companions/companions').then(m => m.Companions)
  },
  {
    path: 'mes-plantes',
    loadComponent: () => import('./features/my-plants/my-plants').then(m => m.MyPlants)
  }
];
