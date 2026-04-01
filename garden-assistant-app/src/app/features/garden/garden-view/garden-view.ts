import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faPlus, faPen, faArrowLeft, faTrash } from '@fortawesome/free-solid-svg-icons';
import { UpdateGardenRequest, CreateBedRequest, UpdateBedRequest } from '../../../api/garden-assistant-api';
import { GardenStore } from '../../../shared/services/garden.store';
import { DialogService } from '../../../shared/services/dialog.service';
import { GardenDialogService } from '../../../shared/services/garden-dialog.service';
import { BedPanel } from '../bed-panel/bed-panel';
import { GardenCalendar } from '../garden-calendar/garden-calendar';

@Component({
  selector: 'app-garden-view',
  standalone: true,
  imports: [TranslateModule, FontAwesomeModule, BedPanel, GardenCalendar],
  templateUrl: './garden-view.html',
})
export class GardenView implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  protected readonly store = inject(GardenStore);
  private readonly dialogService = inject(DialogService);
  private readonly gardenDialogService = inject(GardenDialogService);
  private readonly translate = inject(TranslateService);

  protected readonly faPlus = faPlus;
  protected readonly faPen = faPen;
  protected readonly faArrowLeft = faArrowLeft;
  protected readonly faTrash = faTrash;

  readonly gardenId = signal('');

  readonly garden = computed(() =>
    this.store.gardens().find(g => g.id === this.gardenId())
  );

  readonly beds = computed(() =>
    [...this.store.beds()].sort((a, b) => (a.name ?? '').localeCompare(b.name ?? '', 'fr'))
  );
  readonly hasBeds = computed(() => this.beds().length > 0);

  async ngOnInit(): Promise<void> {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.router.navigate(['/garden']);
      return;
    }
    this.gardenId.set(id);
    await this.store.loadBeds(id);
  }

  goBack(): void {
    this.router.navigate(['/garden']);
  }

  async editGarden(): Promise<void> {
    const g = this.garden();
    if (!g) {
      return;
    }
    const result = await this.gardenDialogService.openCreateGarden({ mode: 'edit', name: g.name, description: g.description });
    if (result) {
      await this.store.updateGarden(g.id!, { name: result.name, description: result.description } as UpdateGardenRequest);
    }
  }

  async deleteGarden(): Promise<void> {
    const g = this.garden();
    if (!g?.id) {
      return;
    }
    const confirmed = await this.dialogService.confirm(
      this.translate.instant('Garden.ConfirmDeleteTitle'),
      this.translate.instant('Garden.ConfirmDeleteMessage', { name: g.name }),
      this.translate.instant('Garden.Delete'),
      true
    );
    if (confirmed) {
      await this.store.deleteGarden(g.id);
      this.router.navigate(['/garden']);
    }
  }

  async addBed(): Promise<void> {
    const result = await this.gardenDialogService.openCreateBed({ mode: 'create' });
    if (result !== undefined) {
      await this.store.createBed(this.gardenId(), { name: result.name } as CreateBedRequest);
    }
  }

  async onBedRenamed(bedId: string, newName: string | undefined): Promise<void> {
    await this.store.updateBed(this.gardenId(), bedId, { name: newName } as UpdateBedRequest);
  }

  async onBedDeleted(bedId: string): Promise<void> {
    await this.store.deleteBed(this.gardenId(), bedId);
  }
}
