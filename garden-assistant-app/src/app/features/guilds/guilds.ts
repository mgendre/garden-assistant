import { Component, inject, signal, computed } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faPlus } from '@fortawesome/free-solid-svg-icons';
import { firstValueFrom } from 'rxjs';
import { GuildDto } from '../../api/garden-assistant-api';
import { GuildStore } from '../../shared/services/guild.store';
import { CompanionStore } from '../../shared/services/companion.store';
import { PlantStore } from '../../shared/services/plant.store';
import { ConfirmDialogComponent, ConfirmDialogData } from '../../shared/confirm-dialog/confirm-dialog';
import { PlantDetailDialog, PlantDetailDialogData } from '../../shared/ui/plant-detail-dialog/plant-detail-dialog';
import { SearchInput } from '../../shared/ui/search-input/search-input';
import { GuildCard } from '../../shared/ui/guild-card/guild-card';

@Component({
  selector: 'app-guilds',
  standalone: true,
  imports: [TranslateModule, FontAwesomeModule, SearchInput, GuildCard],
  templateUrl: './guilds.html',
  styleUrl: './guilds.scss'
})
export class Guilds {
  protected readonly store = inject(GuildStore);
  private readonly companionStore = inject(CompanionStore);
  private readonly plantStore = inject(PlantStore);
  private readonly router = inject(Router);
  private readonly dialog = inject(MatDialog);
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
    const plant = this.plantStore.findById(plantId);
    if (!plant) {
      return;
    }
    this.dialog.open<PlantDetailDialog, PlantDetailDialogData>(PlantDetailDialog, {
      data: { plant },
      maxWidth: '600px',
      width: '90vw',
    });
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
    this.companionStore.startGuildCreation();
    this.router.navigate(['/companions']);
  }
}
