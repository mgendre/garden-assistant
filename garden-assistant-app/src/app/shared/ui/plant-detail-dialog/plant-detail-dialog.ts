import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';
import { PlantDto } from '../../../api/garden-assistant-api';
import { PlantCard } from '../plant-card/plant-card';

export interface PlantDetailDialogData {
  plant: PlantDto;
}

@Component({
  selector: 'app-plant-detail-dialog',
  standalone: true,
  imports: [MatDialogModule, TranslateModule, PlantCard],
  templateUrl: './plant-detail-dialog.html',
  styleUrl: './plant-detail-dialog.scss'
})
export class PlantDetailDialog {
  readonly data = inject<PlantDetailDialogData>(MAT_DIALOG_DATA);
}
