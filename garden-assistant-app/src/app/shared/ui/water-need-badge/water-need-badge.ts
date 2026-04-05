import { Component, input } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-water-need-badge',
  standalone: true,
  imports: [TranslateModule],
  templateUrl: './water-need-badge.html'
})
export class WaterNeedBadge {
  readonly waterNeeds = input.required<string | undefined>();
}
