import { Component, inject, input, output, signal, computed } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faPen, faTrash } from '@fortawesome/free-solid-svg-icons';
import { MatDialog } from '@angular/material/dialog';
import { firstValueFrom } from 'rxjs';
import {
  BedDto,
  PlantDto,
  GuildDto,
  CompanionRecommendationRequest,
  GuildPlantRole,
  PropagationMethod,
  PlantActionType,
} from '../../../api/garden-assistant-api';
import { PlantStore } from '../../../shared/services/plant.store';
import { GuildService } from '../../../shared/services/guild.service';
import { CompanionService } from '../../../shared/services/companion.service';
import { CalendarService } from '../../../shared/services/calendar.service';
import { Collapsible } from '../../../shared/ui/collapsible/collapsible';
import { PlantAssociationPanel, PlantCalendarEntry } from '../../../shared/ui/plant-association-panel/plant-association-panel';
import { PlantDetailDialog, PlantDetailDialogData } from '../../../shared/ui/plant-detail-dialog/plant-detail-dialog';
import { HarvestReadinessDialog, HarvestReadinessDialogData } from '../../../shared/ui/harvest-readiness/harvest-readiness-dialog';
import { BadgeInfoDialog, BadgeInfoDialogData } from '../../../shared/ui/badge-info-dialog/badge-info-dialog';
import { ConfirmDialogComponent, ConfirmDialogData } from '../../../shared/confirm-dialog/confirm-dialog';
import { CreateBedDialog, CreateBedDialogData, CreateBedDialogResult } from '../create-bed-dialog/create-bed-dialog';
import { SOWING_ACTIONS } from '../../../shared/constants/plant-action.constants';

@Component({
  selector: 'app-bed-panel',
  standalone: true,
  imports: [TranslateModule, FontAwesomeModule, Collapsible, PlantAssociationPanel],
  templateUrl: './bed-panel.html',
})
export class BedPanel {
  readonly bed = input.required<BedDto>();
  readonly gardenId = input.required<string>();
  readonly bedNameChanged = output<string | undefined>();
  readonly bedDeleted = output<void>();

  private readonly plantStore = inject(PlantStore);
  private readonly guildService = inject(GuildService);
  private readonly companionService = inject(CompanionService);
  private readonly calendarService = inject(CalendarService);
  private readonly router = inject(Router);
  private readonly dialog = inject(MatDialog);
  private readonly translate = inject(TranslateService);

  protected readonly faPen = faPen;
  protected readonly faTrash = faTrash;

  readonly guild = signal<GuildDto | null>(null);
  readonly recommendations = signal<any>(null);
  readonly calendarEntries = signal<PlantCalendarEntry[]>([]);
  readonly loading = signal(false);
  readonly loaded = signal(false);

  readonly plants = computed<PlantDto[]>(() => {
    const g = this.guild();
    if (!g) {
      return [];
    }
    const plantIds = new Set((g.plants ?? []).map(p => p.id!).filter(Boolean));
    return this.plantStore.allPlants().filter(p => plantIds.has(p.id!));
  });

  readonly centralPlantIds = computed<Set<string>>(() => {
    const g = this.guild();
    if (!g) {
      return new Set();
    }
    return new Set(
      (g.plants ?? [])
        .filter(p => p.role === GuildPlantRole.Central && p.id)
        .map(p => p.id!)
    );
  });

  readonly associations = computed(() => {
    return this.recommendations()?.selectedPlantAssociations ?? [];
  });

  readonly plantCount = computed(() => this.bed().plantIds?.length ?? 0);

  onHeaderClick(): void {
    this.loadDetail();
  }

  async loadDetail(): Promise<void> {
    const guildId = this.bed().guildId;
    if (!guildId) {
      this.loaded.set(true);
      return;
    }

    this.loading.set(true);
    try {
      const guild = await this.guildService.getById(guildId);
      this.guild.set(guild);

      const plantIds = (guild.plants ?? []).map(p => p.id!).filter(Boolean);
      if (plantIds.length < 2) {
        return;
      }

      const [recs, ...allActions] = await Promise.all([
        this.companionService.getRecommendations({ plantIds, minScore: 0 } as CompanionRecommendationRequest),
        ...plantIds.map(id => this.calendarService.getPlantActions(id)),
      ]);

      this.recommendations.set(recs);

      const entries: PlantCalendarEntry[] = [];
      for (let i = 0; i < plantIds.length; i++) {
        const plant = this.plantStore.findById(plantIds[i]);
        if (plant) {
          entries.push({
            plantId: plantIds[i],
            name: plant.name!,
            propagationMethod: plant.propagationMethod ?? PropagationMethod.Seed,
            frostSensitive: plant.frostSensitive ?? false,
            actions: allActions[i],
          });
        }
      }
      entries.sort((a, b) => {
        const sowA = this.getEarliestHalfMonth(a.actions, SOWING_ACTIONS);
        const sowB = this.getEarliestHalfMonth(b.actions, SOWING_ACTIONS);
        if (sowA !== sowB) {
          return sowA - sowB;
        }
        return a.name.localeCompare(b.name, 'fr');
      });
      this.calendarEntries.set(entries);
    } finally {
      this.loading.set(false);
      this.loaded.set(true);
    }
  }

  async editBedName(event: Event): Promise<void> {
    event.stopPropagation();
    const result = await firstValueFrom(
      this.dialog.open<CreateBedDialog, CreateBedDialogData, CreateBedDialogResult>(CreateBedDialog, {
        data: { mode: 'edit', name: this.bed().name },
        maxWidth: '500px',
        width: '90vw',
      }).afterClosed()
    );
    if (result !== undefined) {
      this.bedNameChanged.emit(result.name);
    }
  }

  async deleteBed(event: Event): Promise<void> {
    event.stopPropagation();
    const plantNames = this.plants().map(p => p.name).join(', ');
    const confirmed = await firstValueFrom(
      this.dialog.open<ConfirmDialogComponent, ConfirmDialogData, boolean>(ConfirmDialogComponent, {
        data: {
          title: this.translate.instant('Bed.ConfirmDeleteTitle'),
          message: this.plantCount() > 0
            ? this.translate.instant('Bed.ConfirmDeleteMessageWithPlants', { name: this.bed().name, plants: plantNames })
            : this.translate.instant('Bed.ConfirmDeleteMessage', { name: this.bed().name }),
          confirmLabel: this.translate.instant('Bed.Delete'),
        },
      }).afterClosed()
    );
    if (confirmed) {
      this.bedDeleted.emit();
    }
  }

  editPlants(): void {
    const guildId = this.bed().guildId;
    if (guildId) {
      this.router.navigate(['/companions'], {
        queryParams: {
          guild: guildId,
          mode: 'edit',
          returnTo: `/garden/${this.gardenId()}`,
          bedName: this.bed().name || undefined
        }
      });
    }
  }

  openPlantDetail(plantId: string): void {
    const plant = this.plantStore.findById(plantId);
    if (plant) {
      this.dialog.open<PlantDetailDialog, PlantDetailDialogData>(PlantDetailDialog, {
        data: { plant },
        maxWidth: '600px',
        width: '90vw',
      });
    }
  }

  async openHarvestReadiness(event: { plantId: string; plantName: string }): Promise<void> {
    const readiness = await this.calendarService.getHarvestReadiness(event.plantId);
    if (readiness) {
      this.dialog.open<HarvestReadinessDialog, HarvestReadinessDialogData>(HarvestReadinessDialog, {
        data: { readiness, plantName: event.plantName },
        maxWidth: '600px',
        width: '90vw',
      });
    } else {
      this.dialog.open<BadgeInfoDialog, BadgeInfoDialogData>(BadgeInfoDialog, {
        data: {
          titleKey: 'BadgeInfo.Action.Harvest.Title',
          descriptionKey: 'BadgeInfo.Action.Harvest.Description',
        },
        maxWidth: '400px',
      });
    }
  }

  private getEarliestHalfMonth(actions: any[], actionTypes: PlantActionType[]): number {
    const matching = actions.filter((a: any) =>
      a.actionType !== undefined && actionTypes.includes(a.actionType)
    );
    if (matching.length === 0) {
      return 99;
    }
    return Math.min(...matching.map((a: any) => a.halfMonthStart ?? 99));
  }
}
