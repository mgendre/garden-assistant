import { Component, input, signal, computed, effect } from '@angular/core';

@Component({
  selector: 'app-collapsible',
  standalone: true,
  imports: [],
  templateUrl: './collapsible.html',
  styleUrl: './collapsible.scss'
})
export class Collapsible {
  readonly initialExpanded = input(false);
  readonly forceExpanded = input(false);
  readonly expanded = signal(false);

  readonly isExpanded = computed(() => this.forceExpanded() || this.expanded());

  constructor() {
    effect(() => {
      if (this.initialExpanded()) {
        this.expanded.set(true);
      }
    });
  }

  toggle(): void {
    if (!this.forceExpanded()) {
      this.expanded.update(v => !v);
    }
  }
}
