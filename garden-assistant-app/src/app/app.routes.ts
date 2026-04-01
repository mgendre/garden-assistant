import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'garden', pathMatch: 'full' },
  {
    path: 'companions',
    loadComponent: () => import('./features/companions/companions').then(m => m.Companions)
  },
  {
    path: 'my-plants',
    loadComponent: () => import('./features/my-plants/my-plants').then(m => m.MyPlants)
  },
  {
    path: 'guilds',
    loadComponent: () => import('./features/guilds/guilds').then(m => m.Guilds)
  },
  {
    path: 'calendar',
    loadComponent: () => import('./features/calendar/calendar').then(m => m.Calendar)
  },
  {
    path: 'garden',
    loadComponent: () => import('./features/garden/garden-list/garden-list').then(m => m.GardenList)
  },
  {
    path: 'garden/:id',
    loadComponent: () => import('./features/garden/garden-view/garden-view').then(m => m.GardenView)
  },
  {
    path: 'whats-new',
    loadComponent: () => import('./features/whats-new/whats-new').then(m => m.WhatsNew)
  }
];
