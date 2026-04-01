import { Component, inject, signal, computed } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { CompanionStore } from '../../../shared/services/companion.store';
import { PlantDialogService } from '../../../shared/services/plant-dialog.service';
import { PlantStore } from '../../../shared/services/plant.store';
import { GuildService } from '../../../shared/services/guild.service';
import { GuildStore } from '../../../shared/services/guild.store';
import { GuildDto } from '../../../api/garden-assistant-api';
import { SearchInput } from '../../../shared/ui/search-input/search-input';
import { GuildCard } from '../../../shared/ui/guild-card/guild-card';
import { EmptyState } from '../../../shared/ui/empty-state/empty-state';

@Component({
  selector: 'app-guild-panel',
  standalone: true,
  imports: [TranslateModule, SearchInput, GuildCard, EmptyState],
  templateUrl: './guild-panel.html',
  styleUrl: './guild-panel.scss'
})
export class GuildPanel {
  protected readonly store = inject(CompanionStore);
  protected readonly guildStore = inject(GuildStore);
  private readonly plantStore = inject(PlantStore);
  private readonly guildService = inject(GuildService);
  private readonly plantDialogService = inject(PlantDialogService);

  readonly searchQuery = signal('');

  readonly filteredGuilds = computed(() => {
    const query = this.searchQuery().toLowerCase();
    const editingId = this.store.editingGuild()?.id;
    const selectedIds = this.store.selectedPlantIds();
    let guilds = this.guildStore.guilds()
      .filter(g => g.id !== editingId)
      .sort((a, b) => (a.name ?? '').localeCompare(b.name ?? '', 'fr'));
    if (selectedIds.size > 0) {
      guilds = guilds.filter(g => g.plants?.some(p => selectedIds.has(p.id)));
    }
    if (query) {
      guilds = guilds.filter(g =>
        (g.name ?? '').toLowerCase().includes(query) ||
        (g.description ?? '').toLowerCase().includes(query) ||
        g.plants?.some(p => (p.name ?? '').toLowerCase().includes(query))
      );
    }
    return guilds;
  });

  async viewGuild(guild: GuildDto): Promise<void> {
    if (!guild.id) {
      return;
    }
    const detail = await this.guildService.getById(guild.id);
    this.store.loadGuildForEditing(detail);
  }

  async editGuild(guild: GuildDto): Promise<void> {
    if (!guild.id) {
      return;
    }
    const detail = await this.guildService.getById(guild.id);
    this.store.loadGuildForEditing(detail);
    this.store.startGuildEditing();
  }

  showPlantDetail(plantId: string): void {
    this.plantDialogService.openDetail(plantId);
  }

  addPlant(plantId: string): void {
    const plant = this.plantStore.findById(plantId);
    if (plant) {
      this.store.addPlant(plant);
    }
  }

  addAllPlantsFromGuild(guild: GuildDto): void {
    for (const guildPlant of guild.plants ?? []) {
      if (guildPlant.id) {
        const plant = this.plantStore.findById(guildPlant.id);
        if (plant) {
          this.store.addPlant(plant);
        }
      }
    }
  }
}
