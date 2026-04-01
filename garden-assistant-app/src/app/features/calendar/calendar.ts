import { Component, inject, OnInit, computed } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';
import { CalendarStore } from '../../shared/services/calendar.store';
import { PlantStore } from '../../shared/services/plant.store';
import { PlantCalendarGantt } from '../../shared/ui/plant-calendar-gantt/plant-calendar-gantt';
import { BadgeInfoDialog, BadgeInfoDialogData } from '../../shared/ui/badge-info-dialog/badge-info-dialog';
import { CalendarThisMonth } from './calendar-this-month';
import { PlantActionType, PlantDto } from '../../api/garden-assistant-api';
import { PlantDetailDialog, PlantDetailDialogData } from '../../shared/ui/plant-detail-dialog/plant-detail-dialog';
import { HarvestReadinessDialog, HarvestReadinessDialogData } from '../../shared/ui/harvest-readiness/harvest-readiness-dialog';
import { CalendarService } from '../../shared/services/calendar.service';
import { ACTION_TYPE_CONFIGS, FILTER_CONFIGS } from '../../shared/constants/plant-action.constants';

@Component({
  selector: 'app-calendar',
  standalone: true,
  imports: [TranslateModule, RouterLink, PlantCalendarGantt, CalendarThisMonth],
  templateUrl: './calendar.html',
  styleUrl: './calendar.scss'
})
export class Calendar implements OnInit {
  protected readonly store = inject(CalendarStore);
  protected readonly plantStore = inject(PlantStore);
  private readonly dialog = inject(MatDialog);
  private readonly calendarService = inject(CalendarService);

  protected readonly filters = FILTER_CONFIGS;
  protected readonly monthLabels = ['J', 'F', 'M', 'A', 'M', 'J', 'J', 'A', 'S', 'O', 'N', 'D'];
  protected readonly currentMonthIndex = new Date().getMonth();

  protected readonly availableFilterKeys = computed(() => {
    const plants = this.store.allCalendarPlants();
    const allTypes = new Set<PlantActionType>();
    for (const plant of plants) {
      for (const action of plant.actions ?? []) {
        if (action.actionType !== undefined) {
          allTypes.add(action.actionType);
        }
      }
    }
    const keys = new Set<string>();
    for (const filter of FILTER_CONFIGS) {
      if (filter.actionTypes.some(t => allTypes.has(t))) {
        keys.add(filter.key);
      }
    }
    return keys;
  });

  protected readonly hasActiveFilter = computed(() => this.store.activeFilterKey() !== null);

  protected readonly ganttFilters = computed<PlantActionType[]>(() => {
    return this.store.activeActionTypes();
  });

  async ngOnInit(): Promise<void> {
    await this.store.loadCalendar();
  }

  isFilterActive(key: string): boolean {
    return this.store.isFilterActive(key);
  }

  toggleFilter(key: string): void {
    this.store.toggleFilter(key);
  }

  openPlantDetail(plant: PlantDto): void {
    this.dialog.open<PlantDetailDialog, PlantDetailDialogData>(PlantDetailDialog, {
      data: { plant },
      maxWidth: '700px',
      width: '95vw',
    });
  }

  async openHarvestReadiness(plantId: string): Promise<void> {
    const plant = this.plantStore.findById(plantId);
    const readiness = await this.calendarService.getHarvestReadiness(plantId);
    if (readiness && plant) {
      this.dialog.open<HarvestReadinessDialog, HarvestReadinessDialogData>(HarvestReadinessDialog, {
        data: { readiness, plantName: plant.name! },
        maxWidth: '600px',
        width: '90vw',
      });
    } else {
      this.openActionInfo(PlantActionType.Harvest);
    }
  }

  openActionInfo(type: PlantActionType): void {
    const key = ACTION_TYPE_CONFIGS.find(c => c.type === type)?.badgeKey;
    if (key) {
      this.dialog.open<BadgeInfoDialog, BadgeInfoDialogData>(BadgeInfoDialog, {
        data: {
          titleKey: `BadgeInfo.Action.${key}.Title`,
          descriptionKey: `BadgeInfo.Action.${key}.Description`,
        },
        maxWidth: '400px',
      });
    }
  }
}
