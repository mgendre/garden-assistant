import { Component, inject, OnInit, computed, signal } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { faHeart, faSeedling, faLayerGroup, faTableCellsLarge, faList, faDroplet } from '@fortawesome/free-solid-svg-icons';
import { CalendarStore, CalendarGrouping, PlantSourceFilter } from '../../shared/services/calendar.store';
import { PlantStore } from '../../shared/services/plant.store';
import { DialogService } from '../../shared/services/dialog.service';
import { PlantDialogService } from '../../shared/services/plant-dialog.service';
import { PlantCalendarGantt } from '../../shared/ui/plant-calendar-gantt/plant-calendar-gantt';
import { CalendarThisMonth } from './calendar-this-month';
import { PlantActionType, PlantDto } from '../../api/garden-assistant-api';
import { CalendarService } from '../../shared/services/calendar.service';
import { ACTION_TYPE_CONFIGS, FILTER_CONFIGS } from '../../shared/constants/plant-action.constants';
import { EmptyState } from '../../shared/ui/empty-state/empty-state';
import { ToggleGroup, ToggleOption } from '../../shared/ui/toggle-group/toggle-group';
import { CalendarWateringToday } from './calendar-watering-today/calendar-watering-today';
import { CalendarWatering } from './calendar-watering/calendar-watering';

@Component({
  selector: 'app-calendar',
  standalone: true,
  imports: [TranslateModule, PlantCalendarGantt, CalendarThisMonth, EmptyState, ToggleGroup, CalendarWateringToday, CalendarWatering],
  templateUrl: './calendar.html',
  styleUrl: './calendar.scss'
})
export class Calendar implements OnInit {
  protected readonly store = inject(CalendarStore);
  protected readonly plantStore = inject(PlantStore);
  private readonly dialogService = inject(DialogService);
  private readonly plantDialogService = inject(PlantDialogService);
  private readonly calendarService = inject(CalendarService);

  protected readonly filters = FILTER_CONFIGS;
  protected readonly activeCalendarTab = signal<'actions' | 'watering'>('actions');
  protected readonly calendarTabOptions: ToggleOption[] = [
    { value: 'actions', labelKey: 'Calendar.TabActions', icon: faSeedling },
    { value: 'watering', labelKey: 'Calendar.TabWatering', icon: faDroplet },
  ];
  protected readonly sourceOptions: ToggleOption[] = [
    { value: 'all', labelKey: 'Calendar.AllPlants' },
    { value: 'myPlants', labelKey: 'Calendar.MyPlantsOnly', icon: faHeart },
    { value: 'gardenPlants', labelKey: 'Calendar.GardenPlantsOnly', icon: faSeedling },
  ];
  protected readonly groupingOptions: ToggleOption[] = [
    { value: 'flat', labelKey: 'Calendar.FlatView', icon: faList },
    { value: 'byGarden', labelKey: 'Calendar.ByGarden', icon: faLayerGroup },
    { value: 'byBed', labelKey: 'Calendar.ByBed', icon: faTableCellsLarge },
  ];
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

  onSourceChange(value: string): void {
    this.store.sourceFilter.set(value as PlantSourceFilter);
    if (value !== 'gardenPlants') {
      this.store.grouping.set('flat');
    }
  }

  onGroupingChange(value: string): void {
    this.store.grouping.set(value as CalendarGrouping);
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
