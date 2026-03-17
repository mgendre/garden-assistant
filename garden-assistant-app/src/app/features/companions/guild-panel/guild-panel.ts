import { Component, inject } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { CompanionStore } from '../../../shared/services/companion.store';
import { PlantStore } from '../../../shared/services/plant.store';
import { GuildDetailDto } from '../../../api/garden-assistant-api';

@Component({
  selector: 'app-guild-panel',
  standalone: true,
  imports: [TranslateModule],
  templateUrl: './guild-panel.html',
  styleUrl: './guild-panel.scss'
})
export class GuildPanel {
  protected readonly store = inject(CompanionStore);
  private readonly plantStore = inject(PlantStore);

  onGuildPlantClick(plantId: string | undefined): void {
    if (!plantId) {
      return;
    }
    const plant = this.plantStore.allPlants().find(p => p.id === plantId);
    if (plant) {
      this.store.addPlant(plant);
    }
  }

  async onAddGuild(guild: GuildDetailDto): Promise<void> {
    await this.store.addGuild(guild);
  }
}
