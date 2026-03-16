import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faPen, faTrash, faArrowUpRightFromSquare, faPlus } from '@fortawesome/free-solid-svg-icons';
import { firstValueFrom } from 'rxjs';
import { GuildSummaryDto } from '../../api/garden-assistant-api';
import { GuildStore } from './guild.store';
import { GuildService } from './guild.service';
import { CompanionStore } from '../companions/companion.store';
import { ConfirmDialogComponent, ConfirmDialogData } from '../../shared/confirm-dialog/confirm-dialog';

@Component({
  selector: 'app-guilds',
  standalone: true,
  imports: [TranslateModule, FontAwesomeModule],
  templateUrl: './guilds.html',
  styleUrl: './guilds.scss'
})
export class Guilds {
  protected readonly store = inject(GuildStore);
  private readonly guildService = inject(GuildService);
  private readonly companionStore = inject(CompanionStore);
  private readonly router = inject(Router);
  private readonly dialog = inject(MatDialog);
  private readonly translate = inject(TranslateService);

  protected readonly faPen = faPen;
  protected readonly faTrash = faTrash;
  protected readonly faOpen = faArrowUpRightFromSquare;
  protected readonly faPlus = faPlus;

  async openInCompanions(guild: GuildSummaryDto): Promise<void> {
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
