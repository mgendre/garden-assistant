import { Component, input, output, signal, computed, effect } from '@angular/core';

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
  readonly open = input<boolean | null>(null);
  readonly toggled = output<boolean>();

  readonly expanded = signal(false);

  readonly isExpanded = computed(() => {
    const ext = this.open();
    if (ext !== null) {
      return ext;
    }
    return this.forceExpanded() || this.expanded();
  });

  constructor() {
    effect(() => {
      if (this.initialExpanded()) {
        this.expanded.set(true);
      }
    });
  }

  toggle(): void {
    const ext = this.open();
    if (ext !== null) {
      this.toggled.emit(!ext);
      return;
    }
    if (!this.forceExpanded()) {
      this.expanded.update(v => !v);
      this.toggled.emit(this.expanded());
    }
  }
}
