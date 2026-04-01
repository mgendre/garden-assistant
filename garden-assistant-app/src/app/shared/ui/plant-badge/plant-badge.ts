import { Component, inject, input, output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { PlantDto } from '../../../api/garden-assistant-api';
import { PlantStore } from '../../services/plant.store';
import { PlantDetailDialog, PlantDetailDialogData } from '../plant-detail-dialog/plant-detail-dialog';

@Component({
  selector: 'app-plant-badge',
  standalone: true,
  imports: [],
  templateUrl: './plant-badge.html',
  host: { style: 'display: inline-block' },
})
export class PlantBadge {
  readonly plant = input.required<PlantDto>();
  readonly central = input(false);

  private readonly dialog = inject(MatDialog);

  openDetail(event: Event): void {
    event.stopPropagation();
    this.dialog.open<PlantDetailDialog, PlantDetailDialogData>(PlantDetailDialog, {
      data: { plant: this.plant() },
      maxWidth: '600px',
      width: '90vw',
    });
  }
}
