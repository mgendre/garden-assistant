import { Injectable, inject } from '@angular/core';
import { PlantStore } from './plant.store';
import { MyPlantsStore } from './my-plants.store';
import { GuildStore } from './guild.store';
import { GardenStore } from './garden.store';

@Injectable({ providedIn: 'root' })
export class StartupService {
  private readonly plantStore = inject(PlantStore);
  private readonly myPlantsStore = inject(MyPlantsStore);
  private readonly guildStore = inject(GuildStore);
  private readonly gardenStore = inject(GardenStore);

  async loadAll(): Promise<void> {
    await Promise.all([
      this.plantStore.loadAll(),
      this.myPlantsStore.load(),
      this.guildStore.load(),
      this.gardenStore.loadGardens(),
    ]);
  }
}
