import { Component, inject, input, output, signal, effect } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { BedDto, PropagationMethod } from '../../../api/garden-assistant-api';
import { PlantStore } from '../../../shared/services/plant.store';
import { PlantDialogService } from '../../../shared/services/plant-dialog.service';
import { Collapsible } from '../../../shared/ui/collapsible/collapsible';
import { PlantCalendarGantt } from '../../../shared/ui/plant-calendar-gantt/plant-calendar-gantt';
import { PlantCalendarEntry } from '../../../shared/ui/plant-association-panel/plant-association-panel';
import { SOWING_ACTIONS, getEarliestHalfMonth } from '../../../shared/constants/plant-action.constants';
import { EmptyState } from '../../../shared/ui/empty-state/empty-state';
import { ToggleGroup, ToggleOption } from '../../../shared/ui/toggle-group/toggle-group';

interface BedCalendarGroup {
  bedName: string;
  entries: PlantCalendarEntry[];
}

@Component({
  selector: 'app-garden-calendar',
  standalone: true,
  imports: [TranslateModule, Collapsible, PlantCalendarGantt, EmptyState, ToggleGroup],
  templateUrl: './garden-calendar.html',
})
export class GardenCalendar {
  readonly beds = input.required<BedDto[]>();
  readonly open = input<boolean | null>(null);
  readonly toggled = output<boolean>();

  private readonly plantStore = inject(PlantStore);
  private readonly plantDialogService = inject(PlantDialogService);

  openPlantDetail(plantId: string): void {
    this.plantDialogService.openDetail(plantId);
  }

  readonly loading = signal(false);
  readonly allEntries = signal<PlantCalendarEntry[]>([]);
  readonly bedGroups = signal<BedCalendarGroup[]>([]);
  readonly groupByBed = signal(false);
  readonly viewOptions: ToggleOption[] = [
    { value: 'flat', labelKey: 'GardenCalendar.FlatView' },
    { value: 'byBed', labelKey: 'GardenCalendar.GroupedByBed' },
  ];

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

  private loadCalendar(beds: BedDto[]): void {
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
          actions: plant.actions ?? [],
        };
      };

      const sortEntries = (entries: PlantCalendarEntry[]) => {
        return entries.sort((a, b) => {
          const sowA = getEarliestHalfMonth(a.actions, SOWING_ACTIONS);
          const sowB = getEarliestHalfMonth(b.actions, SOWING_ACTIONS);
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

}
