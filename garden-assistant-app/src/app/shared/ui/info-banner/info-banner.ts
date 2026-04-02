import { Component, input, output } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-info-banner',
  standalone: true,
  imports: [TranslateModule],
  templateUrl: './info-banner.html',
  host: { '[class]': '"info-banner info-banner--" + variant()' },
})
export class InfoBanner {
  readonly emoji = input<string | undefined>(undefined);
  readonly messageKey = input.required<string>();
  readonly messageParams = input<Record<string, unknown>>({});
  readonly actionLabel = input<string | undefined>(undefined);
  readonly variant = input<'info' | 'warning'>('info');
  readonly actionClick = output<void>();
}
