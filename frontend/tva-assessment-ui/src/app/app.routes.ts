import { Routes } from '@angular/router';

import { HomeComponent } from './pages/home/home';
import { PersonsComponent } from './pages/persons/persons';
import { PersonDetailsComponent } from './pages/person-details/person-details';
import { AccountDetailsComponent } from './pages/account-details/account-details';
import { TransactionDetailsComponent } from './pages/transaction-details/transaction-details';
import { AboutComponent } from './pages/about/about';
import { ContactComponent } from './pages/contact/contact';

export const routes: Routes = [
  { path: '', pathMatch: 'full', component: HomeComponent },
  { path: 'persons', component: PersonsComponent },
  { path: 'persons/new', component: PersonDetailsComponent },
  { path: 'persons/:id', component: PersonDetailsComponent },
  { path: 'accounts/new', component: AccountDetailsComponent },
  { path: 'accounts/:id', component: AccountDetailsComponent },
  { path: 'transactions/new', component: TransactionDetailsComponent },
  { path: 'transactions/:id', component: TransactionDetailsComponent },
  { path: 'about', component: AboutComponent },
  { path: 'contact', component: ContactComponent },
  { path: '**', redirectTo: '' },
];
