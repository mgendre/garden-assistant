import { Component, input, signal, effect } from '@angular/core';

@Component({
  selector: 'app-collapsible',
  standalone: true,
  imports: [],
  templateUrl: './collapsible.html',
  styleUrl: './collapsible.scss'
})
export class Collapsible {
  readonly initialExpanded = input(false);
  readonly expanded = signal(false);

  constructor() {
    effect(() => {
      if (this.initialExpanded()) {
        this.expanded.set(true);
      }
    });
  }

  toggle(): void {
    this.expanded.update(v => !v);
  }
}
