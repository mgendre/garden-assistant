import { Component, inject, input } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { DialogService } from '../../services/dialog.service';

@Component({
  selector: 'app-water-need-badge',
  standalone: true,
  imports: [TranslateModule],
  templateUrl: './water-need-badge.html',
  host: { style: 'display: contents' }
})
export class WaterNeedBadge {
  readonly waterNeeds = input.required<string | undefined>();

  private readonly dialogService = inject(DialogService);

  openInfo(event: Event): void {
    event.stopPropagation();
    const level = this.waterNeeds();
    if (!level) { return; }
    this.dialogService.openBadgeInfo(
      `BadgeInfo.Water.${level}.Title`,
      `BadgeInfo.Water.${level}.Description`
    );
  }
}
