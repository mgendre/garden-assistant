import { Component, inject, signal, computed } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faPen, faTrash, faWandMagicSparkles, faPlus } from '@fortawesome/free-solid-svg-icons';
import { firstValueFrom } from 'rxjs';
import { GuildSummaryDto } from '../../api/garden-assistant-api';
import { GuildStore } from '../../shared/services/guild.store';
import { GuildService } from '../../shared/services/guild.service';
import { CompanionStore } from '../../shared/services/companion.store';
import { PlantStore } from '../../shared/services/plant.store';
import { ConfirmDialogComponent, ConfirmDialogData } from '../../shared/confirm-dialog/confirm-dialog';
import { PlantDetailDialog, PlantDetailDialogData } from '../../shared/ui/plant-detail-dialog/plant-detail-dialog';
import { SearchInput } from '../../shared/ui/search-input/search-input';

@Component({
  selector: 'app-guilds',
  standalone: true,
  imports: [TranslateModule, FontAwesomeModule, SearchInput],
  templateUrl: './guilds.html',
  styleUrl: './guilds.scss'
})
export class Guilds {
  protected readonly store = inject(GuildStore);
  private readonly guildService = inject(GuildService);
  private readonly companionStore = inject(CompanionStore);
  private readonly plantStore = inject(PlantStore);
  private readonly router = inject(Router);
  private readonly dialog = inject(MatDialog);
  private readonly translate = inject(TranslateService);

  protected readonly faPen = faPen;
  protected readonly faTrash = faTrash;
  protected readonly faCustomize = faWandMagicSparkles;
  protected readonly faPlus = faPlus;

  readonly searchQuery = signal('');

  readonly filteredUserGuilds = computed(() => this.filterGuilds(this.store.userGuilds()));
  readonly filteredOfficialGuilds = computed(() => this.filterGuilds(this.store.officialGuilds()));

  private filterGuilds(guilds: GuildSummaryDto[]): GuildSummaryDto[] {
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

  openPlantDetail(plantId: string | undefined, event: Event): void {
    event.stopPropagation();
    if (!plantId) {
      return;
    }
    const plant = this.plantStore.findById(plantId);
    if (!plant) {
      return;
    }
    this.dialog.open<PlantDetailDialog, PlantDetailDialogData>(PlantDetailDialog, {
      data: { plant },
      maxWidth: '500px',
      width: '90vw',
    });
  }

  async customizeGuild(guild: GuildSummaryDto, event: Event): Promise<void> {
    event.stopPropagation();
    if (!guild.id) {
      return;
    }
    const detail = await this.guildService.getById(guild.id);
    this.companionStore.loadGuildForEditing(detail);
    this.router.navigate(['/companions']);
  }

  async editGuild(guild: GuildSummaryDto): Promise<void> {
    if (!guild.id) {
      return;
    }
    const detail = await this.guildService.getById(guild.id);
    this.companionStore.loadGuildForEditing(detail);
    this.router.navigate(['/companions']);
  }

  async deleteGuild(guild: GuildSummaryDto, event: Event): Promise<void> {
    event.stopPropagation();
    if (!guild.id) {
      return;
    }
    const confirmed = await firstValueFrom(
      this.dialog.open<ConfirmDialogComponent, ConfirmDialogData, boolean>(ConfirmDialogComponent, {
        data: {
          title: this.translate.instant('Guilds.ConfirmDeleteTitle'),
          message: this.translate.instant('Guilds.ConfirmDeleteMessage', { name: guild.name }),
          confirmLabel: this.translate.instant('Guilds.Delete'),
        },
      }).afterClosed()
    );
    if (confirmed) {
      await this.store.deleteGuild(guild.id);
    }
  }

  createGuild(): void {
    this.companionStore.startNewGuild();
    this.router.navigate(['/companions']);
  }
}
