import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';

export interface BadgeInfoDialogData {
  titleKey: string;
  descriptionKey: string;
  timesPerWeek?: number;
}

@Component({
  selector: 'app-badge-info-dialog',
  standalone: true,
  imports: [MatDialogModule, TranslateModule],
  templateUrl: './badge-info-dialog.html',
  styleUrl: './badge-info-dialog.scss'
})
export class BadgeInfoDialog {
  readonly data = inject<BadgeInfoDialogData>(MAT_DIALOG_DATA);
}
