import { Component, inject, computed } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faPlus } from '@fortawesome/free-solid-svg-icons';
import { GardenDto, CreateGardenRequest } from '../../../api/garden-assistant-api';
import { GardenStore } from '../../../shared/services/garden.store';
import { DialogService } from '../../../shared/services/dialog.service';
import { GardenDialogService } from '../../../shared/services/garden-dialog.service';
import { EmptyState } from '../../../shared/ui/empty-state/empty-state';

@Component({
  selector: 'app-garden-list',
  standalone: true,
  imports: [TranslateModule, FontAwesomeModule, EmptyState],
  templateUrl: './garden-list.html',
})
export class GardenList {
  protected readonly store = inject(GardenStore);
  private readonly router = inject(Router);
  private readonly dialogService = inject(DialogService);
  private readonly gardenDialogService = inject(GardenDialogService);
  private readonly translate = inject(TranslateService);
  protected readonly faPlus = faPlus;

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
    const result = await this.gardenDialogService.openCreateGarden({ mode: 'create' });
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
    const confirmed = await this.dialogService.confirm(
      this.translate.instant('Garden.ConfirmDeleteTitle'),
      this.translate.instant('Garden.ConfirmDeleteMessage', { name: garden.name }),
      this.translate.instant('Garden.Delete'),
      true
    );
    if (confirmed) {
      await this.store.deleteGarden(garden.id);
    }
  }
}
