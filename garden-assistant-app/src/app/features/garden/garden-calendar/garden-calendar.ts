import { Component, inject, input, signal, effect } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { MatDialog } from '@angular/material/dialog';
import { BedDto, PropagationMethod, PlantActionType } from '../../../api/garden-assistant-api';
import { PlantStore } from '../../../shared/services/plant.store';
import { CalendarService } from '../../../shared/services/calendar.service';
import { PlantDetailDialog, PlantDetailDialogData } from '../../../shared/ui/plant-detail-dialog/plant-detail-dialog';
import { Collapsible } from '../../../shared/ui/collapsible/collapsible';
import { PlantCalendarGantt } from '../../../shared/ui/plant-calendar-gantt/plant-calendar-gantt';
import { PlantCalendarEntry } from '../../../shared/ui/plant-association-panel/plant-association-panel';
import { SOWING_ACTIONS } from '../../../shared/constants/plant-action.constants';

interface BedCalendarGroup {
  bedName: string;
  entries: PlantCalendarEntry[];
}

@Component({
  selector: 'app-garden-calendar',
  standalone: true,
  imports: [TranslateModule, Collapsible, PlantCalendarGantt],
  templateUrl: './garden-calendar.html',
})
export class GardenCalendar {
  readonly beds = input.required<BedDto[]>();

  private readonly plantStore = inject(PlantStore);
  private readonly calendarService = inject(CalendarService);
  private readonly dialog = inject(MatDialog);

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

  readonly loading = signal(false);
  readonly allEntries = signal<PlantCalendarEntry[]>([]);
  readonly bedGroups = signal<BedCalendarGroup[]>([]);
  readonly groupByBed = signal(false);

  constructor() {
    effect(() => {
      const beds = this.beds();
      if (beds.length === 0) {
        this.allEntries.set([]);
        this.bedGroups.set([]);
        return;
      }
      this.loadCalendar(beds);
    });
  }

  toggleGrouping(): void {
    this.groupByBed.update(v => !v);
  }

  private async loadCalendar(beds: BedDto[]): Promise<void> {
    this.loading.set(true);
    try {
      const allPlantIds = new Set<string>();
      const plantIdsByBed = new Map<string, string[]>();

      for (const bed of beds) {
        const ids = (bed.plantIds ?? []).map(id => String(id));
        plantIdsByBed.set(bed.id!, ids);
        for (const id of ids) {
          allPlantIds.add(id);
        }
      }

      if (allPlantIds.size === 0) {
        this.allEntries.set([]);
        this.bedGroups.set([]);
        return;
      }

      const uniqueIds = [...allPlantIds];
      const allActions = await Promise.all(
        uniqueIds.map(id => this.calendarService.getPlantActions(id))
      );

      const actionsByPlant = new Map<string, any[]>();
      for (let i = 0; i < uniqueIds.length; i++) {
        actionsByPlant.set(uniqueIds[i], allActions[i]);
      }

      const buildEntry = (plantId: string): PlantCalendarEntry | null => {
        const plant = this.plantStore.findById(plantId);
        if (!plant) {
          return null;
        }
        return {
          plantId,
          name: plant.name!,
          propagationMethod: plant.propagationMethod ?? PropagationMethod.Seed,
          frostSensitive: plant.frostSensitive ?? false,
          actions: actionsByPlant.get(plantId) ?? [],
        };
      };

      const sortEntries = (entries: PlantCalendarEntry[]) => {
        return entries.sort((a, b) => {
          const sowA = this.getEarliestHalfMonth(a.actions, SOWING_ACTIONS);
          const sowB = this.getEarliestHalfMonth(b.actions, SOWING_ACTIONS);
          if (sowA !== sowB) {
            return sowA - sowB;
          }
          return a.name.localeCompare(b.name, 'fr');
        });
      };

      const flat = new Map<string, PlantCalendarEntry>();
      const groups: BedCalendarGroup[] = [];

      for (const bed of beds) {
        const bedEntries: PlantCalendarEntry[] = [];
        for (const plantId of plantIdsByBed.get(bed.id!) ?? []) {
          const entry = buildEntry(plantId);
          if (entry) {
            bedEntries.push(entry);
            if (!flat.has(plantId)) {
              flat.set(plantId, entry);
            }
          }
        }
        if (bedEntries.length > 0) {
          groups.push({ bedName: bed.name || 'Planche', entries: sortEntries(bedEntries) });
        }
      }

      this.allEntries.set(sortEntries([...flat.values()]));
      this.bedGroups.set(groups);
    } finally {
      this.loading.set(false);
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
