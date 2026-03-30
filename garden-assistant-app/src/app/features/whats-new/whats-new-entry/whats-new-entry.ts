import { Component, input } from '@angular/core';
import { MarkdownComponent } from 'ngx-markdown';

@Component({
  selector: 'app-whats-new-entry',
  standalone: true,
  imports: [MarkdownComponent],
  templateUrl: './whats-new-entry.html',
  styleUrl: './whats-new-entry.scss'
})
export class WhatsNewEntry {
  readonly title = input.required<string>();
  readonly date = input.required<string>();
  readonly markdownContent = input.required<string>();
}
