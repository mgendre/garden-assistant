import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faPlus, faPen, faArrowLeft } from '@fortawesome/free-solid-svg-icons';
import { MatDialog } from '@angular/material/dialog';
import { firstValueFrom } from 'rxjs';
import { UpdateGardenRequest, CreateBedRequest, UpdateBedRequest } from '../../../api/garden-assistant-api';
import { GardenStore } from '../../../shared/services/garden.store';
import { BedPanel } from '../bed-panel/bed-panel';
import { CreateGardenDialog, CreateGardenDialogData, CreateGardenDialogResult } from '../create-garden-dialog/create-garden-dialog';
import { CreateBedDialog, CreateBedDialogData, CreateBedDialogResult } from '../create-bed-dialog/create-bed-dialog';
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
  private readonly dialog = inject(MatDialog);

  protected readonly faPlus = faPlus;
  protected readonly faPen = faPen;
  protected readonly faArrowLeft = faArrowLeft;

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
    const result = await firstValueFrom(
      this.dialog.open<CreateGardenDialog, CreateGardenDialogData, CreateGardenDialogResult>(CreateGardenDialog, {
        data: { mode: 'edit', name: g.name, description: g.description },
        maxWidth: '500px',
        width: '90vw',
      }).afterClosed()
    );
    if (result) {
      await this.store.updateGarden(g.id!, { name: result.name, description: result.description } as UpdateGardenRequest);
    }
  }

  async addBed(): Promise<void> {
    const result = await firstValueFrom(
      this.dialog.open<CreateBedDialog, CreateBedDialogData, CreateBedDialogResult>(CreateBedDialog, {
        data: { mode: 'create' },
        maxWidth: '500px',
        width: '90vw',
      }).afterClosed()
    );
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
