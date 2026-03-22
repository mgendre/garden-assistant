import { Component, computed, inject, signal, effect } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faPen, faPlus, faXmark, faLink, faTriangleExclamation } from '@fortawesome/free-solid-svg-icons';
import { MatDialog } from '@angular/material/dialog';
import { CompanionStore } from '../../../shared/services/companion.store';
import { GuildStore } from '../../../shared/services/guild.store';
import { PlantStore } from '../../../shared/services/plant.store';
import { CalendarService } from '../../../shared/services/calendar.service';
import { PlantActionDto, PlantActionType, PropagationMethod } from '../../../api/garden-assistant-api';
import { SOWING_ACTIONS } from '../../../shared/constants/plant-action.constants';
import { PlantDetailPanel } from '../plant-detail-panel/plant-detail-panel';
import { GuildPanel } from '../guild-panel/guild-panel';
import { Collapsible } from '../../../shared/ui/collapsible/collapsible';
import { PlantCalendarGantt } from '../../../shared/ui/plant-calendar-gantt/plant-calendar-gantt';
import { HarvestReadinessDialog, HarvestReadinessDialogData } from '../../../shared/ui/harvest-readiness/harvest-readiness-dialog';
import { BadgeInfoDialog, BadgeInfoDialogData } from '../../../shared/ui/badge-info-dialog/badge-info-dialog';
import { RootStratification } from '../root-stratification/root-stratification';

interface PlantCalendarEntry {
  plantId: string;
  name: string;
  propagationMethod: PropagationMethod;
  frostSensitive: boolean;
  actions: PlantActionDto[];
}

@Component({
  selector: 'app-guild-editor',
  standalone: true,
  imports: [TranslateModule, FontAwesomeModule, PlantDetailPanel, GuildPanel, Collapsible, RootStratification, PlantCalendarGantt],
  templateUrl: './guild-editor.html',
  styleUrl: './guild-editor.scss'
})
export class GuildEditor {
  protected readonly store = inject(CompanionStore);
  protected readonly guildStore = inject(GuildStore);
  protected readonly plantStore = inject(PlantStore);
  protected readonly faPen = faPen;
  protected readonly faPlus = faPlus;
  protected readonly faClose = faXmark;
  protected readonly faLink = faLink;
  protected readonly faWarning = faTriangleExclamation;
  private readonly dialog = inject(MatDialog);
  private readonly calendarService = inject(CalendarService);

  readonly plantCalendars = signal<PlantCalendarEntry[]>([]);

  constructor() {
    effect(async () => {
      const plants = this.store.selectedPlants();
      if (plants.length < 2) {
        this.plantCalendars.set([]);
        return;
      }
      const entries: PlantCalendarEntry[] = [];
      const plantIds = plants.map(p => p.id!).filter(Boolean);
      const actionsPromises = plantIds.map(id => this.calendarService.getPlantActions(id));
      const allActions = await Promise.all(actionsPromises);
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
        if (sowA !== sowB) { return sowA - sowB; }
        const transA = this.getEarliestHalfMonth(a.actions, [PlantActionType.Transplanting]);
        const transB = this.getEarliestHalfMonth(b.actions, [PlantActionType.Transplanting]);
        if (transA !== transB) { return transA - transB; }
        const harvA = this.getEarliestHalfMonth(a.actions, [PlantActionType.Harvest]);
        const harvB = this.getEarliestHalfMonth(b.actions, [PlantActionType.Harvest]);
        if (harvA !== harvB) { return harvA - harvB; }
        return a.name.localeCompare(b.name, 'fr');
      });
      this.plantCalendars.set(entries);
    });
  }

  protected readonly hasHarmfulAssociations = computed(() => {
    const associations = this.store.recommendations()?.selectedPlantAssociations;
    return associations?.some(a => a.effect === 1) ?? false;
  });

  plantName(id: string | undefined): string {
    return this.plantStore.findById(id)?.name ?? '';
  }

  async openHarvestReadiness(plantId: string, plantName: string): Promise<void> {
    const readiness = await this.calendarService.getHarvestReadiness(plantId);
    if (readiness) {
      this.dialog.open<HarvestReadinessDialog, HarvestReadinessDialogData>(HarvestReadinessDialog, {
        data: { readiness, plantName },
        maxWidth: '500px',
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

  private getEarliestHalfMonth(actions: PlantActionDto[], actionTypes: PlantActionType[]): number {
    const matching = actions.filter(a =>
      a.actionType !== undefined && actionTypes.includes(a.actionType)
    );
    if (matching.length === 0) {
      return 99;
    }
    return Math.min(...matching.map(a => a.halfMonthStart ?? 99));
  }

  openMechanismInfo(mechanism: number): void {
    const key = this.store.getMechanismKey(mechanism);
    if (key) {
      this.dialog.open<BadgeInfoDialog, BadgeInfoDialogData>(BadgeInfoDialog, {
        data: {
          titleKey: `BadgeInfo.Mechanism.${key}.Title`,
          descriptionKey: `BadgeInfo.Mechanism.${key}.Description`,
        },
        maxWidth: '400px',
      });
    }
  }
}
