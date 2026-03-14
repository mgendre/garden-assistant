import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  {
    path: '',
    loadComponent: () => import('./layout/shell/shell').then(m => m.ShellComponent),
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./features/dashboard/dashboard').then(m => m.DashboardComponent)
      },
      {
        path: 'garden',
        loadComponent: () => import('./features/garden/garden').then(m => m.GardenComponent)
      },
      {
        path: 'tasks',
        loadComponent: () => import('./features/tasks/tasks').then(m => m.TasksComponent)
      },
      {
        path: 'companions',
        loadComponent: () => import('./features/companions/companions').then(m => m.CompanionsComponent)
      }
    ]
  },
  { path: '**', redirectTo: 'dashboard' }
];
