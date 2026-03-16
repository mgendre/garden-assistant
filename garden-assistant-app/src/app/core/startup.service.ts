import { Injectable, inject } from '@angular/core';
import { CompanionStore } from '../features/companions/companion.store';
import { MyPlantsStore } from '../features/my-plants/my-plants.store';
import { GuildStore } from '../features/guilds/guild.store';

@Injectable({ providedIn: 'root' })
export class StartupService {
  private readonly companionStore = inject(CompanionStore);
  private readonly myPlantsStore = inject(MyPlantsStore);
  private readonly guildStore = inject(GuildStore);

  async loadAll(): Promise<void> {
    await Promise.all([
      this.companionStore.loadPlants(),
      this.myPlantsStore.load(),
      this.guildStore.load(),
    ]);
  }
}
