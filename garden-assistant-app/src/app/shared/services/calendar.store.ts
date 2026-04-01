import { Injectable, inject, signal, computed } from '@angular/core';
import {
  CalendarDto,
  CalendarPlantDto,
  PlantActionType,
  HarvestReadinessDto,
  BedDto,
  GardenDto,
} from '../../api/garden-assistant-api';
import { CalendarService } from './calendar.service';
import { PlantStore } from './plant.store';
import { GardenService } from './garden.service';
import { GardenStore } from './garden.store';
import { MyPlantsStore } from './my-plants.store';
import { FILTER_CONFIGS, SOWING_ACTIONS, getEarliestHalfMonth } from '../constants/plant-action.constants';

export type PlantSourceFilter = 'all' | 'myPlants' | 'gardenPlants';
export type CalendarGrouping = 'flat' | 'byGarden';

export interface GardenCalendarGroup {
  gardenName: string;
  plants: CalendarPlantDto[];
}

interface GardenBedData {
  garden: GardenDto;
  beds: BedDto[];
}

@Injectable({ providedIn: 'root' })
export class CalendarStore {
  private readonly calendarService = inject(CalendarService);
  private readonly plantStore = inject(PlantStore);
  private readonly gardenService = inject(GardenService);
  private readonly gardenStore = inject(GardenStore);
  private readonly myPlantsStore = inject(MyPlantsStore);

  readonly calendarData = signal<CalendarDto | null>(null);
  readonly gardenCalendarPlants = signal<CalendarPlantDto[]>([]);
  readonly gardenBedData = signal<GardenBedData[]>([]);
  readonly loading = signal(false);
  readonly activeFilterKey = signal<string | null>(null);
  readonly sourceFilter = signal<PlantSourceFilter>('all');
  readonly grouping = signal<CalendarGrouping>('flat');

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

  private readonly gardenPlantIds = computed<Set<string>>(() => {
    const ids = this.gardenBedData()
      .flatMap(gbd => gbd.beds)
      .flatMap(bed => (bed.plantIds ?? []).map(String));
    return new Set(ids);
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

    const source = this.sourceFilter();
    if (source === 'myPlants') {
      const myIds = this.myPlantsStore.plantIds();
      plants = plants.filter(p => myIds.has(p.plantId));
    } else if (source === 'gardenPlants') {
      const gardenIds = this.gardenPlantIds();
      plants = plants.filter(p => gardenIds.has(p.plantId!));
    }

    return this.sortAndFilterByAction(plants);
  });

  readonly gardenGroups = computed<GardenCalendarGroup[]>(() => {
    const calendarMap = new Map<string, CalendarPlantDto>();
    for (const p of this.allCalendarPlants()) {
      if (p.plantId) {
        calendarMap.set(p.plantId, p);
      }
    }

    const filterTypes = this.activeActionTypes();
    const groups: GardenCalendarGroup[] = [];

    for (const gbd of this.gardenBedData()) {
      const plantIds = new Set<string>();
      for (const bed of gbd.beds) {
        for (const id of bed.plantIds ?? []) {
          plantIds.add(String(id));
        }
      }

      let plants: CalendarPlantDto[] = [];
      for (const id of plantIds) {
        const cp = calendarMap.get(id);
        if (cp) {
          plants.push(cp);
        }
      }

      plants = this.sortAndFilterByAction(plants);

      if (plants.length > 0) {
        groups.push({ gardenName: gbd.garden.name ?? '', plants });
      }
    }

    return groups.sort((a, b) => a.gardenName.localeCompare(b.gardenName, 'fr'));
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

  private sortAndFilterByAction(plants: CalendarPlantDto[]): CalendarPlantDto[] {
    const filterTypes = this.activeActionTypes();

    if (filterTypes.length === 0) {
      return [...plants].sort((a, b) => {
        const sowA = getEarliestHalfMonth(a.actions ?? [], SOWING_ACTIONS);
        const sowB = getEarliestHalfMonth(b.actions ?? [], SOWING_ACTIONS);
        if (sowA !== sowB) { return sowA - sowB; }
        const transA = getEarliestHalfMonth(a.actions ?? [], [PlantActionType.Transplanting]);
        const transB = getEarliestHalfMonth(b.actions ?? [], [PlantActionType.Transplanting]);
        if (transA !== transB) { return transA - transB; }
        const harvA = getEarliestHalfMonth(a.actions ?? [], [PlantActionType.Harvest]);
        const harvB = getEarliestHalfMonth(b.actions ?? [], [PlantActionType.Harvest]);
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
        const firstA = getEarliestHalfMonth(a.actions ?? [], filterTypes);
        const firstB = getEarliestHalfMonth(b.actions ?? [], filterTypes);
        return firstA - firstB;
      });
  }

  private async loadGardenPlants(): Promise<void> {
    const allGardens = this.gardenStore.gardens();

    if (allGardens.length === 0) {
      this.gardenCalendarPlants.set([]);
      this.gardenBedData.set([]);
      return;
    }

    const allPlantIds = new Set<string>();
    const bedData: GardenBedData[] = [];

    for (const garden of allGardens) {
      if (!garden.id) { continue; }
      const beds = await this.gardenService.getBeds(garden.id);
      bedData.push({ garden, beds });
      for (const bed of beds) {
        for (const id of bed.plantIds ?? []) {
          allPlantIds.add(String(id));
        }
      }
    }

    this.gardenBedData.set(bedData);

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

}
