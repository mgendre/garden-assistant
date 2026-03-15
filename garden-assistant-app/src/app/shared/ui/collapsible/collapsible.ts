import { Component, signal } from '@angular/core';

@Component({
  selector: 'app-collapsible',
  standalone: true,
  imports: [],
  templateUrl: './collapsible.html',
  styleUrl: './collapsible.scss'
})
export class Collapsible {
  readonly expanded = signal(false);

  toggle(): void {
    this.expanded.update(v => !v);
  }
}
