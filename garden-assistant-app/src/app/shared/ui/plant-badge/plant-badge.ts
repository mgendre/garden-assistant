import { Component, inject, input } from '@angular/core';
import { PlantDto } from '../../../api/garden-assistant-api';
import { PlantDialogService } from '../../services/plant-dialog.service';

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

  private readonly plantDialogService = inject(PlantDialogService);

  get displayName(): string {
    return this.plant()?.name ?? this.plantName() ?? '';
  }

  openDetail(event: Event): void {
    event.stopPropagation();
    const plant = this.plant();
    if (plant) {
      this.plantDialogService.openDetail(plant);
    } else {
      const id = this.plantId();
      if (id) {
        this.plantDialogService.openDetail(id);
      }
    }
  }
}
