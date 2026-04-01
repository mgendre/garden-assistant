import { Injectable, inject, signal, computed } from '@angular/core';
import {
  CalendarDto,
  CalendarPlantDto,
  PlantActionType,
  HarvestReadinessDto,
} from '../../api/garden-assistant-api';
import { CalendarService } from './calendar.service';
import { PlantStore } from './plant.store';
import { GardenService } from './garden.service';
import { GardenStore } from './garden.store';
import { MyPlantsStore } from './my-plants.store';
import { FILTER_CONFIGS, SOWING_ACTIONS } from '../constants/plant-action.constants';

@Injectable({ providedIn: 'root' })
export class CalendarStore {
  private readonly calendarService = inject(CalendarService);
  private readonly plantStore = inject(PlantStore);
  private readonly gardenService = inject(GardenService);
  private readonly gardenStore = inject(GardenStore);
  private readonly myPlantsStore = inject(MyPlantsStore);

  readonly calendarData = signal<CalendarDto | null>(null);
  readonly gardenCalendarPlants = signal<CalendarPlantDto[]>([]);
  readonly loading = signal(false);
  readonly activeFilterKey = signal<string | null>(null);
  readonly myPlantsOnly = signal(false);

  readonly allCalendarPlants = computed<CalendarPlantDto[]>(() => {
    const myPlants = this.calendarData()?.plants ?? [];
    const gardenPlants = this.gardenCalendarPlants();

    const seen = new Set<string>();
    const merged: CalendarPlantDto[] = [];

    for (const p of myPlants) {
      if (p.plantId && !seen.has(p.plantId)) {
        seen.add(p.plantId);
        merged.push(p);
      }
    }
    for (const p of gardenPlants) {
      if (p.plantId && !seen.has(p.plantId)) {
        seen.add(p.plantId);
        merged.push(p);
      }
    }

    return merged;
  });

  readonly activeActionTypes = computed<PlantActionType[]>(() => {
    const key = this.activeFilterKey();
    if (key === null) {
      return [];
    }
    return FILTER_CONFIGS.find(f => f.key === key)?.actionTypes ?? [];
  });

  readonly filteredPlants = computed<CalendarPlantDto[]>(() => {
    let plants = this.allCalendarPlants();

    if (this.myPlantsOnly()) {
      const myIds = this.myPlantsStore.plantIds();
      plants = plants.filter(p => myIds.has(p.plantId));
    }

    const filterTypes = this.activeActionTypes();

    if (filterTypes.length === 0) {
      return [...plants].sort((a, b) => {
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

    return plants
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
      const [myPlantsData] = await Promise.all([
        this.calendarService.getMyPlantsCalendar(),
        this.loadGardenPlants(),
      ]);
      this.calendarData.set(myPlantsData);
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

  toggleMyPlantsOnly(): void {
    this.myPlantsOnly.update(v => !v);
  }

  private async loadGardenPlants(): Promise<void> {
    const beds = this.gardenStore.beds();
    const allGardens = this.gardenStore.gardens();

    if (allGardens.length === 0) {
      this.gardenCalendarPlants.set([]);
      return;
    }

    const allPlantIds = new Set<string>();
    if (beds.length > 0) {
      for (const bed of beds) {
        for (const id of bed.plantIds ?? []) {
          allPlantIds.add(String(id));
        }
      }
    }

    if (allPlantIds.size === 0) {
      for (const garden of allGardens) {
        if (!garden.id) { continue; }
        const gardenBeds = await this.gardenService.getBeds(garden.id);
        for (const bed of gardenBeds) {
          for (const id of bed.plantIds ?? []) {
            allPlantIds.add(String(id));
          }
        }
      }
    }

    if (allPlantIds.size === 0) {
      this.gardenCalendarPlants.set([]);
      return;
    }

    const uniqueIds = [...allPlantIds];
    const allActions = await Promise.all(
      uniqueIds.map(id => this.calendarService.getPlantActions(id))
    );

    const entries: CalendarPlantDto[] = [];
    for (let i = 0; i < uniqueIds.length; i++) {
      entries.push({ plantId: uniqueIds[i], actions: allActions[i] } as CalendarPlantDto);
    }

    this.gardenCalendarPlants.set(entries);
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
