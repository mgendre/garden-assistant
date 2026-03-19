import { Component, inject, signal, computed } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';
import { CompanionStore } from '../../../shared/services/companion.store';
import { PlantDetailDialog, PlantDetailDialogData } from '../../../shared/ui/plant-detail-dialog/plant-detail-dialog';
import { PlantStore } from '../../../shared/services/plant.store';
import { GuildService } from '../../../shared/services/guild.service';
import { GuildStore } from '../../../shared/services/guild.store';
import { GuildDto } from '../../../api/garden-assistant-api';
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
  protected readonly guildStore = inject(GuildStore);
  private readonly plantStore = inject(PlantStore);
  private readonly guildService = inject(GuildService);
  private readonly dialog = inject(MatDialog);

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
