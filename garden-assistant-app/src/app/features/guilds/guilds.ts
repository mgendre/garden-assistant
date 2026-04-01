import { Component, inject, signal, computed } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faPlus } from '@fortawesome/free-solid-svg-icons';
import { GuildDto } from '../../api/garden-assistant-api';
import { GuildStore } from '../../shared/services/guild.store';
import { CompanionStore } from '../../shared/services/companion.store';
import { DialogService } from '../../shared/services/dialog.service';
import { PlantDialogService } from '../../shared/services/plant-dialog.service';
import { SearchInput } from '../../shared/ui/search-input/search-input';
import { GuildCard } from '../../shared/ui/guild-card/guild-card';
import { EmptyState } from '../../shared/ui/empty-state/empty-state';

@Component({
  selector: 'app-guilds',
  standalone: true,
  imports: [TranslateModule, FontAwesomeModule, SearchInput, GuildCard, EmptyState],
  templateUrl: './guilds.html',
  styleUrl: './guilds.scss'
})
export class Guilds {
  protected readonly store = inject(GuildStore);
  private readonly companionStore = inject(CompanionStore);
  private readonly router = inject(Router);
  private readonly dialogService = inject(DialogService);
  private readonly plantDialogService = inject(PlantDialogService);
  private readonly translate = inject(TranslateService);

  protected readonly faPlus = faPlus;

  readonly searchQuery = signal('');

  readonly filteredUserGuilds = computed(() => this.filterGuilds(this.store.userGuilds()));
  readonly filteredOfficialGuilds = computed(() => this.filterGuilds(this.store.officialGuilds()));

  private filterGuilds(guilds: GuildDto[]): GuildDto[] {
    const query = this.searchQuery().toLowerCase();
    if (!query) {
      return guilds;
    }
    return guilds.filter(g =>
      (g.name ?? '').toLowerCase().includes(query) ||
      (g.description ?? '').toLowerCase().includes(query) ||
      g.plants?.some(p => (p.name ?? '').toLowerCase().includes(query))
    );
  }

  openPlantDetail(plantId: string): void {
    this.plantDialogService.openDetail(plantId);
  }

  viewGuild(guild: GuildDto): void {
    if (guild.id) {
      this.router.navigate(['/companions'], { queryParams: { guild: guild.id } });
    }
  }

  editGuild(guild: GuildDto): void {
    if (guild.id) {
      this.router.navigate(['/companions'], { queryParams: { guild: guild.id, mode: 'edit' } });
    }
  }

  async deleteGuild(guild: GuildDto): Promise<void> {
    if (!guild.id) {
      return;
    }
    const confirmed = await this.dialogService.confirm(
      this.translate.instant('Guilds.ConfirmDeleteTitle'),
      this.translate.instant('Guilds.ConfirmDeleteMessage', { name: guild.name }),
      this.translate.instant('Guilds.Delete'),
      true
    );
    if (confirmed) {
      await this.store.deleteGuild(guild.id);
    }
  }

  createGuild(): void {
    this.companionStore.startNewGuild();
    this.router.navigate(['/companions'], { queryParams: { mode: 'create' } });
  }
}
