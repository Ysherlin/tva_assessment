import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AccountService } from '../../services/account.service';
import { Account } from '../../models/account.model';

@Component({
  selector: 'app-account-details',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './account-details.html',
  styleUrl: './account-details.css'
})
export class AccountDetailsComponent {

  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private accountService = inject(AccountService);

  isNew = signal(false);
  isEditMode = signal(false);
  loading = signal(false);

  form = signal<Account>({
    personCode: 0,
    accountNumber: '',
    outstandingBalance: 0,
    isClosed: false
  });

  ngOnInit() {
    const codeParam = this.route.snapshot.paramMap.get('id');
    const personCodeParam = this.route.snapshot.queryParamMap.get('personCode');

    // NEW ACCOUNT
    if (!codeParam && personCodeParam) {
      this.isNew.set(true);
      this.isEditMode.set(true);
      this.form.update(f => ({
        ...f,
        personCode: Number(personCodeParam)
      }));
      return;
    }

    // EXISTING ACCOUNT
    const code = Number(codeParam);
    if (!code || Number.isNaN(code)) {
      alert('Invalid account');
      this.router.navigate(['/persons']);
      return;
    }

    this.loading.set(true);
    this.accountService.getOne(code).subscribe({
      next: (a) => {
        this.form.set(a);
        this.loading.set(false);
      },
      error: () => {
        alert('Failed to load account');
        this.router.navigate(['/persons']);
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

    const action = this.isNew()
      ? this.accountService.create(this.form())
      : this.accountService.update(this.form());

    action.subscribe({
      next: (saved) => {
        alert('Account saved successfully');
        this.form.set(saved);
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

  closeAccount() {
    if (!confirm('Close this account?')) return;

    this.accountService.close(this.form().code!).subscribe({
      next: (a) => {
        alert('Account closed');
        this.form.set(a);
      },
      error: (err: any) =>
        alert(err?.error?.message ?? 'Close failed')
    });
  }

  reopenAccount() {
    this.accountService.reopen(this.form().code!).subscribe({
      next: (a) => {
        alert('Account reopened');
        this.form.set(a);
      },
      error: (err: any) =>
        alert(err?.error?.message ?? 'Reopen failed')
    });
  }

  openTransaction(t: any) {
    this.router.navigate(['/transactions', t.code]);
  }

  addTransaction() {
    this.router.navigate(['/transactions/new'], {
      queryParams: { accountCode: this.form().code }
    });
  }
  
  backToPerson() {
  this.router.navigate(['/persons', this.form().personCode]);
 }
}
