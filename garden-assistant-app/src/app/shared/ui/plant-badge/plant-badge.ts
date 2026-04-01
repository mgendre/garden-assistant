import { Component, inject, input } from '@angular/core';
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
  readonly plant = input<PlantDto | null>(null);
  readonly plantId = input<string | undefined>(undefined);
  readonly plantName = input<string | undefined>(undefined);
  readonly central = input(false);

  private readonly dialog = inject(MatDialog);
  private readonly plantStore = inject(PlantStore);

  get displayName(): string {
    return this.plant()?.name ?? this.plantName() ?? '';
  }

  openDetail(event: Event): void {
    event.stopPropagation();
    const plant = this.plant() ?? this.plantStore.findById(this.plantId());
    if (plant) {
      this.dialog.open<PlantDetailDialog, PlantDetailDialogData>(PlantDetailDialog, {
        data: { plant },
        maxWidth: '600px',
        width: '90vw',
      });
    }
  }
}
