import { Component, inject, OnInit, computed } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faHeart, faSeedling, faLayerGroup } from '@fortawesome/free-solid-svg-icons';
import { CalendarStore } from '../../shared/services/calendar.store';
import { PlantStore } from '../../shared/services/plant.store';
import { DialogService } from '../../shared/services/dialog.service';
import { PlantDialogService } from '../../shared/services/plant-dialog.service';
import { PlantCalendarGantt } from '../../shared/ui/plant-calendar-gantt/plant-calendar-gantt';
import { CalendarThisMonth } from './calendar-this-month';
import { PlantActionType, PlantDto } from '../../api/garden-assistant-api';
import { CalendarService } from '../../shared/services/calendar.service';
import { ACTION_TYPE_CONFIGS, FILTER_CONFIGS } from '../../shared/constants/plant-action.constants';

@Component({
  selector: 'app-calendar',
  standalone: true,
  imports: [TranslateModule, RouterLink, FontAwesomeModule, PlantCalendarGantt, CalendarThisMonth],
  templateUrl: './calendar.html',
  styleUrl: './calendar.scss'
})
export class Calendar implements OnInit {
  protected readonly store = inject(CalendarStore);
  protected readonly plantStore = inject(PlantStore);
  private readonly dialogService = inject(DialogService);
  private readonly plantDialogService = inject(PlantDialogService);
  private readonly calendarService = inject(CalendarService);

  protected readonly faHeart = faHeart;
  protected readonly faSeedling = faSeedling;
  protected readonly faLayerGroup = faLayerGroup;
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
    this.plantDialogService.openDetail(plant);
  }

  async openHarvestReadiness(plantId: string): Promise<void> {
    const plant = this.plantStore.findById(plantId);
    const plantName = plant?.name ?? '';
    await this.plantDialogService.openHarvestReadiness(plantId, plantName);
  }

  openActionInfo(type: PlantActionType): void {
    const key = ACTION_TYPE_CONFIGS.find(c => c.type === type)?.badgeKey;
    if (key) {
      this.dialogService.openBadgeInfo(
        `BadgeInfo.Action.${key}.Title`,
        `BadgeInfo.Action.${key}.Description`
      );
    }
  }
}
