import { Injectable, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { PlantDto } from '../../api/garden-assistant-api';
import { PlantStore } from './plant.store';
import { CalendarService } from './calendar.service';
import { DialogService } from './dialog.service';
import { PlantDetailDialog, PlantDetailDialogData } from '../ui/plant-detail-dialog/plant-detail-dialog';
import { HarvestReadinessDialog, HarvestReadinessDialogData } from '../ui/harvest-readiness/harvest-readiness-dialog';

@Injectable({ providedIn: 'root' })
export class PlantDialogService {
  private readonly dialog = inject(MatDialog);
  private readonly plantStore = inject(PlantStore);
  private readonly calendarService = inject(CalendarService);
  private readonly dialogService = inject(DialogService);

  openDetail(plantOrId: PlantDto | string): void {
    const plant = typeof plantOrId === 'string'
      ? this.plantStore.findById(plantOrId)
      : plantOrId;
    if (!plant) {
      return;
    }
    this.dialog.open<PlantDetailDialog, PlantDetailDialogData>(PlantDetailDialog, {
      data: { plant },
      maxWidth: '600px',
      width: '90vw',
    });
  }

  async openHarvestReadiness(plantId: string, plantName: string): Promise<void> {
    const readiness = await this.calendarService.getHarvestReadiness(plantId);
    if (readiness) {
      this.dialog.open<HarvestReadinessDialog, HarvestReadinessDialogData>(HarvestReadinessDialog, {
        data: { readiness, plantName },
        maxWidth: '600px',
        width: '90vw',
      });
    } else {
      this.dialogService.openBadgeInfo(
        'BadgeInfo.Action.Harvest.Title',
        'BadgeInfo.Action.Harvest.Description'
      );
    }
  }
}
