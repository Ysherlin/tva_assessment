import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatInputModule } from '@angular/material/input';
import { MatNativeDateModule } from '@angular/material/core';
import { TransactionService } from '../../services/transaction.service';
import { Transaction } from '../../models/transaction.model';

@Component({
  selector: 'app-transaction-details',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatDatepickerModule,
    MatInputModule,
    MatNativeDateModule
  ],
  templateUrl: './transaction-details.html',
  styleUrl: './transaction-details.css'
})
export class TransactionDetailsComponent {

  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private transactionService = inject(TransactionService);

  isNew = signal(false);
  loading = signal(false);

  form = signal<Transaction>({
    accountCode: 0,
    transactionDate: new Date(),
    amount: 0,
    description: ''
  });

  ngOnInit() {
    const codeParam = this.route.snapshot.paramMap.get('id');
    const accountCodeParam =
      this.route.snapshot.queryParamMap.get('accountCode');

    if (!codeParam && accountCodeParam) {
      this.isNew.set(true);
      this.form.update(f => ({
        ...f,
        accountCode: Number(accountCodeParam)
      }));
      return;
    }

    const code = Number(codeParam);
    if (!code || Number.isNaN(code)) {
      alert('Invalid transaction');
      this.router.navigate(['/persons']);
      return;
    }

    this.loading.set(true);
    this.transactionService.getOne(code).subscribe({
      next: (t) => {
        this.form.set({
          ...t,
          transactionDate: new Date(t.transactionDate),
          captureDate: t.captureDate ? new Date(t.captureDate) : undefined
        });
        this.loading.set(false);
      },
      error: () => {
        alert('Failed to load transaction');
        this.router.navigate(['/persons']);
      }
    });
  }

  save() {
    this.loading.set(true);

    const action = this.isNew()
      ? this.transactionService.create(this.form())
      : this.transactionService.update(this.form());

    action.subscribe({
      next: (saved) => {
        alert('Transaction saved successfully');
        this.form.set(saved);
        this.isNew.set(false);
        this.loading.set(false);
      },
      error: (err: any) => {
        alert(err?.error?.message ?? 'Save failed');
        this.loading.set(false);
      }
    });
  }

  cancel() {
    this.router.navigate(['/accounts', this.form().accountCode]);
  }
}
