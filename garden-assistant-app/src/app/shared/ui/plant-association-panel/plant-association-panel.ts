import { Component, input, output, computed, inject } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faWarning } from '@fortawesome/free-solid-svg-icons';
import {
  PlantDto,
  GuildAssociationDto,
  AssociationEffect,
  AssociationMechanism,
  RootDepth,
  WaterNeeds,
  PlantActionDto,
  PropagationMethod,
} from '../../../api/garden-assistant-api';
import { CompanionStore, PRIORITY_MECHANISMS } from '../../services/companion.store';
import { DialogService } from '../../services/dialog.service';
import { PlantDialogService } from '../../services/plant-dialog.service';
import { Collapsible } from '../collapsible/collapsible';
import { PlantCard } from '../plant-card/plant-card';
import { PlantBadge } from '../plant-badge/plant-badge';
import { PlantCalendarGantt } from '../plant-calendar-gantt/plant-calendar-gantt';
import { RootStratification } from '../../../features/companions/root-stratification/root-stratification';

export interface PlantCalendarEntry {
  plantId: string;
  name: string;
  actions: PlantActionDto[];
  propagationMethod: PropagationMethod;
  frostSensitive: boolean;
}

interface MechanismRow {
  mechanism: AssociationMechanism;
  key: string;
  satisfied: boolean;
  providers: PlantDto[];
}

@Component({
  selector: 'app-plant-association-panel',
  standalone: true,
  imports: [TranslateModule, FontAwesomeModule, Collapsible, PlantCard, PlantBadge, PlantCalendarGantt, RootStratification],
  templateUrl: './plant-association-panel.html',
})
export class PlantAssociationPanel {
  readonly plants = input.required<PlantDto[]>();
  readonly associations = input<GuildAssociationDto[]>([]);
  readonly calendarEntries = input<PlantCalendarEntry[]>([]);
  readonly centralPlantIds = input<Set<string>>(new Set());

  readonly interactive = input(false);
  readonly showPlantCards = input(false);

  readonly plantNameClick = output<string>();
  readonly harvestReadinessClick = output<{ plantId: string; plantName: string }>();
  readonly mechanismFilterToggle = output<AssociationMechanism>();
  readonly rootDepthFilterToggle = output<RootDepth>();

  protected readonly store = inject(CompanionStore);
  private readonly dialogService = inject(DialogService);
  private readonly plantDialogService = inject(PlantDialogService);
  protected readonly faWarning = faWarning;

  readonly beneficialCount = computed(() =>
    this.uniqueAssociationPairs(AssociationEffect.Beneficial).length
  );

  readonly harmfulCount = computed(() =>
    this.uniqueAssociationPairs(AssociationEffect.Harmful).length
  );

  readonly hasHarmfulAssociations = computed(() => this.harmfulCount() > 0);

  readonly rootDepthGroups = computed(() => {
    const groups = new Map<RootDepth, PlantDto[]>();
    for (const plant of this.plants()) {
      if (plant.rootDepth == null) { continue; }
      const list = groups.get(plant.rootDepth) ?? [];
      list.push(plant);
      groups.set(plant.rootDepth, list);
    }
    return groups;
  });

  readonly mechanismRows = computed<MechanismRow[]>(() => {
    const allMechanisms = new Set<AssociationMechanism>();
    for (const p of this.plants()) {
      for (const m of p.intrinsicMechanisms ?? []) {
        allMechanisms.add(m);
      }
    }
    for (const a of this.associations()) {
      if (a.effect === AssociationEffect.Beneficial && a.mechanism != null) {
        allMechanisms.add(a.mechanism);
      }
    }

    const providers = new Map<AssociationMechanism, PlantDto[]>();
    for (const p of this.plants()) {
      for (const m of p.intrinsicMechanisms ?? []) {
        const list = providers.get(m) ?? [];
        list.push(p);
        providers.set(m, list);
      }
    }

    return PRIORITY_MECHANISMS.map(m => ({
      mechanism: m,
      key: this.store.getMechanismKey(m),
      satisfied: allMechanisms.has(m),
      providers: providers.get(m) ?? [],
    }));
  });

  readonly gapCount = computed(() =>
    this.mechanismRows().filter(r => !r.satisfied).length
  );

  readonly satisfiedCount = computed(() =>
    this.mechanismRows().filter(r => r.satisfied).length
  );

  readonly totalMechanisms = computed(() =>
    this.mechanismRows().length
  );

  readonly hasRootCompetition = computed(() => {
    for (const [, plants] of this.rootDepthGroups()) {
      if (plants.length > 3) {
        return true;
      }
    }
    return false;
  });

  readonly waterConflict = computed(() => {
    const plants = this.plants();
    const lowPlants = plants.filter(p => p.waterNeeds === WaterNeeds.Low).map(p => p.name ?? '');
    const highPlants = plants.filter(p => p.waterNeeds === WaterNeeds.High).map(p => p.name ?? '');
    if (lowPlants.length === 0 || highPlants.length === 0) { return null; }
    return { lowPlants, highPlants };
  });

  readonly hasWaterConflict = computed(() => this.waterConflict() !== null);

  plantName(plantId: string | undefined): string {
    if (!plantId) { return ''; }
    return this.plants().find(p => p.id === plantId)?.name ?? '';
  }

  openMechanismInfo(mechanism: AssociationMechanism): void {
    const key = this.store.getMechanismKey(mechanism);
    if (key) {
      this.dialogService.openBadgeInfo(
        `BadgeInfo.Mechanism.${key}.Title`,
        `BadgeInfo.Mechanism.${key}.Description`
      );
    }
  }

  filterMechanism(mechanism: AssociationMechanism): void {
    this.mechanismFilterToggle.emit(mechanism);
  }

  filterRootDepth(depth: RootDepth): void {
    this.rootDepthFilterToggle.emit(depth);
  }

  readonly filterRootDepthFn = (depth: RootDepth) => {
    this.filterRootDepth(depth);
  };

  openPlantDetail(plant: PlantDto): void {
    this.plantDialogService.openDetail(plant);
  }

  onHarvestReadinessClick(plantId: string, plantName: string): void {
    this.harvestReadinessClick.emit({ plantId, plantName });
  }

  onPlantNameClick(plantId: string): void {
    this.plantNameClick.emit(plantId);
  }

  private uniqueAssociationPairs(effect: AssociationEffect): GuildAssociationDto[] {
    const seen = new Set<string>();
    const result: GuildAssociationDto[] = [];
    for (const a of this.associations()) {
      if (a.effect !== effect) { continue; }
      const key = [a.sourcePlantId, a.targetPlantId].sort().join('-');
      if (seen.has(key)) { continue; }
      seen.add(key);
      result.push(a);
    }
    return result;
  }
}
