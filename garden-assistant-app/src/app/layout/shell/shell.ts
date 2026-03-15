import { Component, HostListener, signal } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';

interface NavItem {
  labelKey: string;
  route: string;
}

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, TranslateModule],
  templateUrl: './shell.html',
  styleUrl: './shell.scss'
})
export class ShellComponent {
  readonly navItems: NavItem[] = [
    { labelKey: 'Nav.Dashboard',  route: '/dashboard' },
    { labelKey: 'Nav.Garden',     route: '/garden' },
    { labelKey: 'Nav.Companions', route: '/companions' },
    { labelKey: 'Nav.Guilds',     route: '/guilds' },
    { labelKey: 'Nav.Tasks',      route: '/tasks' },
  ];

  readonly menuOpen = signal(false);

  toggleMenu(): void {
    this.menuOpen.update(v => !v);
  }

  closeMenu(): void {
    this.menuOpen.set(false);
  }

  @HostListener('document:keydown.escape')
  onEscapeKey(): void {
    if (this.menuOpen()) {
      this.closeMenu();
    }
  }
}
