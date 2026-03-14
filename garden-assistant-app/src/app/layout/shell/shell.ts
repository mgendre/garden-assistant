import { Component, computed, inject, signal } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive, Router } from '@angular/router';

interface NavItem {
  label: string;
  route: string;
  emoji: string;
}

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './shell.html',
  styleUrl: './shell.scss'
})
export class ShellComponent {
  private readonly router = inject(Router);

  readonly navItems: NavItem[] = [
    { label: 'Tableau de bord', route: '/dashboard', emoji: '🏡' },
    { label: 'Mon jardin',      route: '/garden',    emoji: '🌱' },
    { label: 'Tâches',          route: '/tasks',     emoji: '📋' },
    { label: 'Associations',    route: '/companions',emoji: '🤝' },
  ];

  readonly sidebarOpen = signal(false);

  readonly activePageLabel = computed(() => {
    const url = this.router.url;
    return this.navItems.find(item => url.startsWith(item.route))?.label ?? '';
  });

  toggleSidebar(): void {
    this.sidebarOpen.update(v => !v);
  }

  closeSidebar(): void {
    this.sidebarOpen.set(false);
  }
}
