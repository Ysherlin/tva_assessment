import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

import { PersonService } from '../../services/person.service';
import { AccountService } from '../../services/account.service';
import { Person } from '../../models/person.model';
import { Account } from '../../models/account.model';

interface PersonForm {
  code?: number;
  idNumber: string;
  name?: string | null;
  surname?: string | null;
}

@Component({
  selector: 'app-person-details',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './person-details.html',
  styleUrl: './person-details.css'
})
export class PersonDetailsComponent {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private personService = inject(PersonService);
  private accountService = inject(AccountService);

  isNew = signal(false);
  isEditMode = signal(false);
  loading = signal(false);

  form = signal<PersonForm>({
    idNumber: '',
    name: '',
    surname: ''
  });

  private accountsState = signal<Account[]>([]);
  accounts = computed(() => this.accountsState());

  ngOnInit() {
    const idParam = this.route.snapshot.paramMap.get('id');

    // NEW PERSON
    if (!idParam) {
      this.isNew.set(true);
      this.isEditMode.set(true);
      return;
    }

    const code = Number(idParam);
    if (!code) {
      alert('Invalid person');
      this.router.navigate(['/persons']);
      return;
    }

    this.loading.set(true);

    this.personService.getOne(code).subscribe({
      next: (p) => {
        this.form.set({
          code: p.code,
          idNumber: p.idNumber,
          name: p.name,
          surname: p.surname
        });

        this.loadAccounts(code);
      },
      error: () => {
        alert('Failed to load person');
        this.router.navigate(['/persons']);
      }
    });
  }

  private loadAccounts(personCode: number) {
    this.accountService.getByPerson(personCode).subscribe({
      next: (accounts) => {
        this.accountsState.set(accounts);
        this.loading.set(false);
      },
      error: () => {
        alert('Failed to load accounts');
        this.loading.set(false);
      }
    });
  }

  edit() {
    this.isEditMode.set(true);
  }

  cancel() {
    if (this.isNew()) {
      this.router.navigate(['/persons']);
    } else {
      this.isEditMode.set(false);
    }
  }

  save() {
    this.loading.set(true);

    const payload: Person = { ...this.form() };

    const action = this.isNew()
      ? this.personService.create(payload)
      : this.personService.update(payload);

    action.subscribe({
      next: (saved) => {
        alert('Person saved successfully');
        this.form.set({
          code: saved.code,
          idNumber: saved.idNumber,
          name: saved.name,
          surname: saved.surname
        });
        this.isNew.set(false);
        this.isEditMode.set(false);
        this.loading.set(false);
      },
      error: (err: any) => {
        alert(err?.error?.message ?? 'Save failed');
        this.loading.set(false);
      }
    });
  }

  delete() {
    if (!confirm('Are you sure you want to delete this person?')) return;

    this.personService.delete(this.form().code!).subscribe({
      next: () => {
        alert('Person deleted');
        this.router.navigate(['/persons']);
      },
      error: (err: any) =>
        alert(err?.error?.message ?? 'Delete failed')
    });
  }

  openAccount(a: Account) {
    this.router.navigate(['/accounts', a.code]);
  }

  createAccount() {
    this.router.navigate(['/accounts/new'], {
      queryParams: { personCode: this.form().code }
    });
  }
}
