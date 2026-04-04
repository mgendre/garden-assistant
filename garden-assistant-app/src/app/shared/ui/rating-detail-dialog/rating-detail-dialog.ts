import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';
import { DialogService } from '../../services/dialog.service';

export interface RatingDetailData {
  plantName: string;
  rating: number;
  score: number;
  mechanisms: string[];
  mechanismBadgeKeys: string[];
  harmfulMechanisms: string[];
  harmfulMechanismBadgeKeys: string[];
  hasRootDepthBonus: boolean;
  hasSameFamilyMalus: boolean;
  hasWaterIncompatibility: boolean;
  isCentralCompanion: boolean;
}

@Component({
  selector: 'app-rating-detail-dialog',
  standalone: true,
  imports: [TranslateModule],
  templateUrl: './rating-detail-dialog.html',
})
export class RatingDetailDialog {
  readonly data = inject<RatingDetailData>(MAT_DIALOG_DATA);
  private readonly dialogRef = inject(MatDialogRef<RatingDetailDialog>);
  private readonly dialogService = inject(DialogService);

  close(): void {
    this.dialogRef.close();
  }

  getStars(): number[] {
    return [1, 2, 3, 4, 5];
  }

  openMechanismInfo(index: number): void {
    const key = this.data.mechanismBadgeKeys[index];
    if (key) {
      this.dialogService.openBadgeInfo(
        `BadgeInfo.Mechanism.${key}.Title`,
        `BadgeInfo.Mechanism.${key}.Description`
      );
    }
  }

  openHarmfulMechanismInfo(index: number): void {
    const key = this.data.harmfulMechanismBadgeKeys[index];
    if (key) {
      this.dialogService.openBadgeInfo(
        `BadgeInfo.Mechanism.${key}.Title`,
        `BadgeInfo.Mechanism.${key}.Description`
      );
    }
  }
}
