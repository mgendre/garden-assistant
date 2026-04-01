import { Component, input, output } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-empty-state',
  standalone: true,
  imports: [TranslateModule, RouterLink],
  templateUrl: './empty-state.html',
})
export class EmptyState {
  readonly icon = input<string | undefined>(undefined);
  readonly titleKey = input<string | undefined>(undefined);
  readonly messageKey = input<string | undefined>(undefined);
  readonly actionKey = input<string | undefined>(undefined);
  readonly linkRoute = input<string | undefined>(undefined);
  readonly minHeight = input<string | undefined>(undefined);
  readonly actionClick = output<void>();
}
