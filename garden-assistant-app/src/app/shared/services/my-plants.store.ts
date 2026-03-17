import { Injectable, inject, signal, computed } from '@angular/core';
import { PlantDto } from '../../api/garden-assistant-api';
import { MyPlantsService } from './my-plants.service';

@Injectable({ providedIn: 'root' })
export class MyPlantsStore {
  private readonly service = inject(MyPlantsService);

  readonly plants = signal<PlantDto[]>([]);
  readonly loading = signal(false);

  readonly plantIds = computed(() => new Set(this.plants().map(p => p.id)));

  readonly sortedPlants = computed(() =>
    [...this.plants()].sort((a, b) => (a.name ?? '').localeCompare(b.name ?? '', 'fr'))
  );

  async load(): Promise<void> {
    this.loading.set(true);
    try {
      const plants = await this.service.getAll();
      this.plants.set(plants);
    } finally {
      this.loading.set(false);
    }
  }

  async toggle(plant: PlantDto): Promise<void> {
    if (this.plantIds().has(plant.id)) {
      this.plants.update(list => list.filter(p => p.id !== plant.id));
      try {
        await this.service.remove(plant.id!);
      } catch {
        this.plants.update(list => [...list, plant]);
      }
    } else {
      this.plants.update(list => [plant, ...list]);
      try {
        await this.service.add(plant.id!);
      } catch {
        this.plants.update(list => list.filter(p => p.id !== plant.id));
      }
    }
  }

  isSaved(plantId: string | undefined): boolean {
    if (!plantId) {
      return false;
    }
    return this.plantIds().has(plantId);
  }
}
