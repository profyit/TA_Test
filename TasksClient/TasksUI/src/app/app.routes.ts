import { Routes } from '@angular/router';

export const appRoutes: Routes = [

  { 
   // canActivate: [AuthGuard]
    path: 'tasks',
    loadComponent: () => import('./core/tasks/task-list/task-list.component').then(m => m.TaskListComponent),
  },

  {
    path: '',
    redirectTo: '/tasks', 
    pathMatch: 'full'
  },
  {
    path: '**', 
    redirectTo: '/tasks' 
  }
];
export { Routes };

