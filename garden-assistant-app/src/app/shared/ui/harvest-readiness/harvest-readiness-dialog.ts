import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';
import { HarvestReadinessDto } from '../../../api/garden-assistant-api';
import { HarvestReadiness } from './harvest-readiness';

export interface HarvestReadinessDialogData {
  readiness: HarvestReadinessDto;
  plantName: string;
}

@Component({
  selector: 'app-harvest-readiness-dialog',
  standalone: true,
  imports: [MatDialogModule, TranslateModule, HarvestReadiness],
  templateUrl: './harvest-readiness-dialog.html',
  styleUrl: './harvest-readiness-dialog.scss'
})
export class HarvestReadinessDialog {
  readonly data = inject<HarvestReadinessDialogData>(MAT_DIALOG_DATA);
}
