import { Component, inject } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { MatDialog } from '@angular/material/dialog';
import { CompanionStore } from '../../../shared/services/companion.store';
import { PlantCard } from '../../../shared/ui/plant-card/plant-card';
import { BadgeInfoDialog, BadgeInfoDialogData } from '../../../shared/ui/badge-info-dialog/badge-info-dialog';

@Component({
  selector: 'app-plant-detail-panel',
  standalone: true,
  imports: [TranslateModule, PlantCard],
  templateUrl: './plant-detail-panel.html',
  styleUrl: './plant-detail-panel.scss'
})
export class PlantDetailPanel {
  protected readonly store = inject(CompanionStore);
  private readonly dialog = inject(MatDialog);

  openMechanismInfo(mechanism: number): void {
    const key = this.store.getMechanismKey(mechanism);
    if (key) {
      this.dialog.open<BadgeInfoDialog, BadgeInfoDialogData>(BadgeInfoDialog, {
        data: {
          titleKey: `BadgeInfo.Mechanism.${key}.Title`,
          descriptionKey: `BadgeInfo.Mechanism.${key}.Description`,
        },
        maxWidth: '400px',
      });
    }
  }
}
