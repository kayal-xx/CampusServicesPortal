import { Routes } from '@angular/router';
import { FeeList } from './features/fees/fee-list/fee-list';
import { FeeManagement } from './features/fees/fee-management/fee-management';


export const routes: Routes = [
  {
    path: 'fees',
    component: FeeList
  },
  {
  path: 'admin/fees',
  component: FeeManagement
},
  {
    path: '',
    redirectTo: 'fees',
    pathMatch: 'full'
  }
];