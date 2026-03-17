import { Component, Input, Output, EventEmitter } from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faMagnifyingGlass } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-search-input',
  standalone: true,
  imports: [FontAwesomeModule],
  templateUrl: './search-input.html',
  styleUrl: './search-input.scss'
})
export class SearchInput {
  @Input() placeholder = '';
  @Output() valueChange = new EventEmitter<string>();

  protected readonly faSearch = faMagnifyingGlass;
}
