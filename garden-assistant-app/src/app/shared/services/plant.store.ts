import { Injectable, inject, signal } from '@angular/core';
import { PlantDto, PlantsClient } from '../../api/garden-assistant-api';

@Injectable({ providedIn: 'root' })
export class PlantStore {
  private readonly plantsClient = inject(PlantsClient);

  readonly allPlants = signal<PlantDto[]>([]);
  readonly loading = signal(false);

  async loadAll(): Promise<void> {
    this.loading.set(true);
    try {
      const plants = await this.plantsClient.getAll();
      this.allPlants.set(plants);
    } finally {
      this.loading.set(false);
    }
  }

  findById(id: string | undefined): PlantDto | undefined {
    if (!id) {
      return undefined;
    }
    return this.allPlants().find(p => p.id === id);
  }
}
