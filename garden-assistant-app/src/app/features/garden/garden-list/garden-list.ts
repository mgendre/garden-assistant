import { Component, inject, computed } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faPlus, faChevronRight, faLayerGroup } from '@fortawesome/free-solid-svg-icons';
import { MatDialog } from '@angular/material/dialog';
import { firstValueFrom } from 'rxjs';
import { GardenDto, CreateGardenRequest } from '../../../api/garden-assistant-api';
import { GardenStore } from '../../../shared/services/garden.store';
import { ConfirmDialogComponent, ConfirmDialogData } from '../../../shared/confirm-dialog/confirm-dialog';
import { CreateGardenDialog, CreateGardenDialogData, CreateGardenDialogResult } from '../create-garden-dialog/create-garden-dialog';

@Component({
  selector: 'app-garden-list',
  standalone: true,
  imports: [TranslateModule, FontAwesomeModule],
  templateUrl: './garden-list.html',
})
export class GardenList {
  protected readonly store = inject(GardenStore);
  private readonly router = inject(Router);
  private readonly dialog = inject(MatDialog);
  private readonly translate = inject(TranslateService);
  protected readonly faPlus = faPlus;
  protected readonly faChevronRight = faChevronRight;
  protected readonly faLayerGroup = faLayerGroup;

  readonly gardens = computed(() =>
    [...this.store.gardens()].sort((a, b) => (a.name ?? '').localeCompare(b.name ?? '', 'fr'))
  );
  readonly isEmpty = computed(() => this.gardens().length === 0 && !this.store.loading());

  openGarden(garden: GardenDto): void {
    if (garden.id) {
      this.router.navigate(['/garden', garden.id]);
    }
  }

  async createGarden(): Promise<void> {
    const result = await firstValueFrom(
      this.dialog.open<CreateGardenDialog, CreateGardenDialogData, CreateGardenDialogResult>(CreateGardenDialog, {
        data: { mode: 'create' },
        maxWidth: '500px',
        width: '90vw',
      }).afterClosed()
    );
    if (result) {
      const garden = await this.store.createGarden({ name: result.name, description: result.description } as CreateGardenRequest);
      this.router.navigate(['/garden', garden.id]);
    }
  }

  async deleteGarden(event: Event, garden: GardenDto): Promise<void> {
    event.stopPropagation();
    if (!garden.id) {
      return;
    }
    const confirmed = await firstValueFrom(
      this.dialog.open<ConfirmDialogComponent, ConfirmDialogData, boolean>(ConfirmDialogComponent, {
        data: {
          title: this.translate.instant('Garden.ConfirmDeleteTitle'),
          message: this.translate.instant('Garden.ConfirmDeleteMessage', { name: garden.name }),
          confirmLabel: this.translate.instant('Garden.Delete'),
        },
      }).afterClosed()
    );
    if (confirmed) {
      await this.store.deleteGarden(garden.id);
    }
  }
}
