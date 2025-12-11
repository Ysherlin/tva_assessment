import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PersonService } from '../../services/person.service';
import { Person, PagedResult } from '../../models/person.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-persons',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './persons.html',
  styleUrl: './persons.css'
})
export class PersonsComponent {

  private personService = inject(PersonService);
  private router = inject(Router);

  // ✔ Input fields (bound with ngModel)
  idNumber = signal<string>('');
  surname = signal<string>('');

  // ✔ State
  loading = signal<boolean>(false);
  page = signal<number>(1);
  pageSize = 10;

  resultsState = signal<PagedResult<Person>>({
    items: [],
    pageNumber: 1,
    pageSize: 10,
    totalCount: 0,
    totalPages: 0
  });

  // ------ COMPUTED VALUES BOUND TO TEMPLATE ------

  results = computed(() => this.resultsState().items);
  totalPages = computed(() => this.resultsState().totalPages);
  currentPage = computed(() => this.resultsState().pageNumber);

  pageRange = computed(() => {
    const total = this.totalPages();
    if (total <= 1) return [];
    return Array.from({ length: total }, (_, i) => i + 1);
  });

  // ------ MAIN SEARCH FUNCTION (ALWAYS RESETS TO PAGE 1) ------

  search() {
    this.loadPage(1);
  }

  // ------ PAGING ------

  loadPage(page: number) {
    this.page.set(page);
    this.loading.set(true);

    this.personService
      .search(
        this.idNumber() || null,
        this.surname() || null,
        null,          // accountNumber is NOT used in this spec
        page,
        this.pageSize
      )
      .subscribe({
        next: (res) => {
          this.resultsState.set(res);
          this.loading.set(false);
        },
        error: (err) => {
          console.error(err);
          alert(err?.error?.message ?? 'Failed to load persons');
          this.loading.set(false);
        }
      });
  }

  // ------ OPEN DETAILS ------

  openDetails(person: Person) {
    this.router.navigate(['/persons', person.code]);
  }
}
