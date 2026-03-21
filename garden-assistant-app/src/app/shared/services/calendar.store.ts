import { Injectable, inject, signal, computed } from '@angular/core';
import {
  CalendarDto,
  CalendarPlantDto,
  PlantActionType,
  HarvestReadinessDto,
} from '../../api/garden-assistant-api';
import { CalendarService } from './calendar.service';
import { PlantStore } from './plant.store';
import { FILTER_CONFIGS, SOWING_ACTIONS } from '../constants/plant-action.constants';

@Injectable({ providedIn: 'root' })
export class CalendarStore {
  private readonly calendarService = inject(CalendarService);
  private readonly plantStore = inject(PlantStore);

  readonly calendarData = signal<CalendarDto | null>(null);
  readonly loading = signal(false);
  readonly activeFilterKey = signal<string | null>(null);

  readonly activeActionTypes = computed<PlantActionType[]>(() => {
    const key = this.activeFilterKey();
    if (key === null) {
      return [];
    }
    return FILTER_CONFIGS.find(f => f.key === key)?.actionTypes ?? [];
  });

  readonly filteredPlants = computed<CalendarPlantDto[]>(() => {
    const data = this.calendarData();
    if (!data?.plants) {
      return [];
    }
    const filterTypes = this.activeActionTypes();

    if (filterTypes.length === 0) {
      return [...data.plants].sort((a, b) => {
        const sowA = this.getEarliestHalfMonth(a, SOWING_ACTIONS);
        const sowB = this.getEarliestHalfMonth(b, SOWING_ACTIONS);
        if (sowA !== sowB) { return sowA - sowB; }
        const transA = this.getEarliestHalfMonth(a, [PlantActionType.Transplanting]);
        const transB = this.getEarliestHalfMonth(b, [PlantActionType.Transplanting]);
        if (transA !== transB) { return transA - transB; }
        const harvA = this.getEarliestHalfMonth(a, [PlantActionType.Harvest]);
        const harvB = this.getEarliestHalfMonth(b, [PlantActionType.Harvest]);
        if (harvA !== harvB) { return harvA - harvB; }
        const nameA = this.plantStore.findById(a.plantId)?.name ?? '';
        const nameB = this.plantStore.findById(b.plantId)?.name ?? '';
        return nameA.localeCompare(nameB);
      });
    }

    return data.plants
      .filter(plant =>
        plant.actions?.some(action =>
          action.actionType !== undefined && filterTypes.includes(action.actionType)
        )
      )
      .sort((a, b) => {
        const firstA = this.getEarliestHalfMonth(a, filterTypes);
        const firstB = this.getEarliestHalfMonth(b, filterTypes);
        return firstA - firstB;
      });
  });

  async loadCalendar(): Promise<void> {
    this.loading.set(true);
    try {
      const data = await this.calendarService.getMyPlantsCalendar();
      this.calendarData.set(data);
    } finally {
      this.loading.set(false);
    }
  }

  async loadHarvestReadiness(plantId: string): Promise<HarvestReadinessDto | null> {
    return this.calendarService.getHarvestReadiness(plantId);
  }

  toggleFilter(filterKey: string): void {
    this.activeFilterKey.update(current => current === filterKey ? null : filterKey);
  }

  isFilterActive(filterKey: string): boolean {
    return this.activeFilterKey() === filterKey;
  }

  private getEarliestHalfMonth(plant: CalendarPlantDto, actionTypes: PlantActionType[]): number {
    const actions = plant.actions?.filter(a =>
      a.actionType !== undefined && actionTypes.includes(a.actionType)
    ) ?? [];
    if (actions.length === 0) {
      return 99;
    }
    return Math.min(...actions.map(a => a.halfMonthStart ?? 99));
  }
}
