import { Component, inject, signal, computed } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';
import { CompanionStore } from '../../../shared/services/companion.store';
import { PlantDetailDialog, PlantDetailDialogData } from '../../../shared/ui/plant-detail-dialog/plant-detail-dialog';
import { PlantStore } from '../../../shared/services/plant.store';
import { GuildService } from '../../../shared/services/guild.service';
import { GuildSummaryDto } from '../../../api/garden-assistant-api';
import { SearchInput } from '../../../shared/ui/search-input/search-input';
import { GuildCard } from '../../../shared/ui/guild-card/guild-card';

@Component({
  selector: 'app-guild-panel',
  standalone: true,
  imports: [TranslateModule, SearchInput, GuildCard],
  templateUrl: './guild-panel.html',
  styleUrl: './guild-panel.scss'
})
export class GuildPanel {
  protected readonly store = inject(CompanionStore);
  private readonly plantStore = inject(PlantStore);
  private readonly guildService = inject(GuildService);
  private readonly dialog = inject(MatDialog);

  readonly searchQuery = signal('');

  readonly filteredGuilds = computed(() => {
    const query = this.searchQuery().toLowerCase();
    const guilds = this.store.guildsForSelectedPlants();
    if (!query) {
      return guilds;
    }
    return guilds.filter(g =>
      (g.name ?? '').toLowerCase().includes(query) ||
      (g.description ?? '').toLowerCase().includes(query) ||
      g.plants?.some(p => (p.name ?? '').toLowerCase().includes(query))
    );
  });

  async viewGuild(guild: GuildSummaryDto): Promise<void> {
    if (!guild.id) {
      return;
    }
    const detail = await this.guildService.getById(guild.id);
    this.store.loadGuildForEditing(detail);
  }

  async editGuild(guild: GuildSummaryDto): Promise<void> {
    if (!guild.id) {
      return;
    }
    const detail = await this.guildService.getById(guild.id);
    this.store.loadGuildForEditing(detail);
    this.store.startGuildEditing();
  }

  showPlantDetail(plantId: string): void {
    const plant = this.plantStore.findById(plantId);
    if (plant) {
      this.dialog.open<PlantDetailDialog, PlantDetailDialogData>(PlantDetailDialog, {
        data: { plant },
        maxWidth: '500px',
        width: '90vw',
      });
    }
  }

  addPlant(plantId: string): void {
    const plant = this.plantStore.findById(plantId);
    if (plant) {
      this.store.addPlant(plant);
    }
  }
}
