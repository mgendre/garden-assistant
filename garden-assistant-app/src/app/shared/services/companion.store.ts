import { Injectable, inject, signal, computed, effect, untracked } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import {
  PlantDto,
  CompanionSearchResultDto,
  CompanionRecommendationRequest,
  GuildDto,
  AssociationMechanism,
  AssociationEffect,
  RootDepth,
  CreateGuildRequest,
  UpdateGuildRequest,
  GuildPlantRole,
  GuildPlantRequest,
} from '../../api/garden-assistant-api';
import { CompanionService } from './companion.service';
import { GuildService } from './guild.service';
import { GuildStore } from './guild.store';
import { MyPlantsStore } from './my-plants.store';
import { PlantStore } from './plant.store';

const FAMILY_EMOJI_MAP: Record<string, string> = {
  'Solanaceae': '🍅',
  'Lamiaceae': '🌿',
  'Apiaceae': '🥕',
  'Amaryllidaceae': '🧅',
  'Fabaceae': '🫘',
  'Poaceae': '🌽',
  'Cucurbitaceae': '🥒',
  'Brassicaceae': '🥦',
  'Asteraceae': '🌻',
  'Rosaceae': '🍎',
  'Boraginaceae': '💙',
};

const FAMILY_CLASS_MAP: Record<string, string> = {
  'Solanaceae': 'fam-solanum',
  'Lamiaceae': 'fam-lamiaceae',
  'Apiaceae': 'fam-apiaceae',
  'Amaryllidaceae': 'fam-allium',
  'Fabaceae': 'fam-legume',
  'Poaceae': 'fam-poaceae',
  'Cucurbitaceae': 'fam-cucurbit',
  'Brassicaceae': 'fam-brassica',
  'Asteraceae': 'fam-asteraceae',
  'Rosaceae': 'fam-rosaceae',
  'Boraginaceae': 'fam-boraginaceae',
};

const MECHANISM_KEY_MAP: Record<number, string> = {
  [AssociationMechanism.OlfactoryConfusion]: 'OlfactoryConfusion',
  [AssociationMechanism.PollinatorAttraction]: 'PollinatorAttraction',
  [AssociationMechanism.TrapCrop]: 'TrapCrop',
  [AssociationMechanism.RootAllelopathy]: 'RootAllelopathy',
  [AssociationMechanism.AerialRepulsion]: 'AerialRepulsion',
  [AssociationMechanism.NitrogenFixation]: 'NitrogenFixation',
  [AssociationMechanism.PredatorAttraction]: 'PredatorAttraction',
  [AssociationMechanism.PhysicalSupport]: 'PhysicalSupport',
  [AssociationMechanism.SoilCover]: 'SoilCover',
  [AssociationMechanism.DynamicAccumulation]: 'DynamicAccumulation',
  [AssociationMechanism.MycorrhizalNetwork]: 'MycorrhizalNetwork',
  [AssociationMechanism.HydraulicLift]: 'HydraulicLift',
  [AssociationMechanism.MicroclimateModification]: 'MicroclimateModification',
  [AssociationMechanism.WeedSuppression]: 'WeedSuppression',
  [AssociationMechanism.Biofumigation]: 'Biofumigation',
  [AssociationMechanism.NursePlant]: 'NursePlant',
};

export const PRIORITY_MECHANISMS: AssociationMechanism[] = [
  AssociationMechanism.NitrogenFixation,
  AssociationMechanism.SoilCover,
  AssociationMechanism.PollinatorAttraction,
  AssociationMechanism.DynamicAccumulation,
  AssociationMechanism.PredatorAttraction,
];

@Injectable({ providedIn: 'root' })
export class CompanionStore {
  private readonly service = inject(CompanionService);
  private readonly guildService = inject(GuildService);
  private readonly guildStore = inject(GuildStore);
  private readonly myPlantsStore = inject(MyPlantsStore);
  private readonly plantStore = inject(PlantStore);
  private readonly translate = inject(TranslateService);

  readonly selectedPlants = signal<PlantDto[]>([]);
  readonly searchQuery = signal('');
  readonly sortMode = signal<'compat' | 'alpha' | 'family'>('compat');
  readonly myPlantsOnly = signal(false);
  readonly mechanismFilter = signal<number | null>(null);
  readonly rootDepthFilter = signal<RootDepth | null>(null);
  readonly recommendations = signal<CompanionSearchResultDto | null>(null);
  readonly loading = signal(false);

  readonly editingGuild = signal<GuildDto | null>(null);
  readonly guildName = signal('');
  readonly guildDescription = signal('');
  readonly guildSaving = signal(false);
  readonly guildMode = signal<'companions' | 'creating' | 'viewing' | 'editing'>('companions');
  readonly centralPlantIds = signal<Set<string>>(new Set());

  readonly isFormVisible = computed(() =>
    this.guildMode() === 'creating' || this.guildMode() === 'editing'
  );

  readonly hasCentralPlants = computed(() => this.centralPlantIds().size > 0);

  readonly sortedSelectedPlants = computed(() => {
    const centralIds = this.centralPlantIds();
    return [...this.selectedPlants()].sort((a, b) => {
      const aCentral = centralIds.has(a.id!) ? 0 : 1;
      const bCentral = centralIds.has(b.id!) ? 0 : 1;
      if (aCentral !== bCentral) { return aCentral - bCentral; }
      return (a.name ?? '').localeCompare(b.name ?? '', 'fr');
    });
  });

  readonly rootDepthGroups = computed(() => {
    const groups = new Map<RootDepth, PlantDto[]>();
    for (const plant of this.selectedPlants()) {
      if (plant.rootDepth == null) { continue; }
      const list = groups.get(plant.rootDepth) ?? [];
      list.push(plant);
      groups.set(plant.rootDepth, list);
    }
    for (const [key, list] of groups) {
      groups.set(key, list.sort((a, b) => (a.name ?? '').localeCompare(b.name ?? '', 'fr')));
    }
    return groups;
  });

  readonly canCreateGuild = computed(() =>
    this.guildMode() === 'companions' && this.selectedPlants().length >= 2
  );

  readonly selectedPlantIds = computed(() =>
    new Set(this.selectedPlants().map(p => p.id))
  );

  readonly avoidPlantIds = computed(() =>
    new Set(this.recommendations()?.plantsToAvoid?.map(p => p.plantId).filter(Boolean) ?? [])
  );

  readonly catalogAssociationMechanisms = computed(() => {
    const map = new Map<string, { beneficial: number[]; harmful: number[] }>();
    const sortMechanisms = (mechanisms: number[]) =>
      [...mechanisms].sort((a, b) => {
        const keyA = this.translate.instant(`Plant.Mechanism.${MECHANISM_KEY_MAP[a] ?? ''}`);
        const keyB = this.translate.instant(`Plant.Mechanism.${MECHANISM_KEY_MAP[b] ?? ''}`);
        return keyA.localeCompare(keyB, 'fr');
      });
    for (const c of this.recommendations()?.goodCompanions ?? []) {
      if (c.plantId && c.mechanisms?.length) {
        const entry = map.get(c.plantId) ?? { beneficial: [], harmful: [] };
        entry.beneficial = sortMechanisms(c.mechanisms);
        map.set(c.plantId, entry);
      }
    }
    for (const a of this.recommendations()?.plantsToAvoid ?? []) {
      if (a.plantId && a.mechanisms?.length) {
        const entry = map.get(a.plantId) ?? { beneficial: [], harmful: [] };
        entry.harmful = sortMechanisms(a.mechanisms);
        map.set(a.plantId, entry);
      }
    }
    return map;
  });

  readonly goodCompanionIds = computed(() => {
    const avoid = this.avoidPlantIds();
    const selected = this.selectedPlantIds();
    const ids = this.recommendations()?.goodCompanions
      ?.filter(c => c.plantId && !avoid.has(c.plantId) && !selected.has(c.plantId))
      .map(c => c.plantId) ?? [];
    return new Set(ids);
  });

  readonly centralCompanionIds = computed(() => {
    const centralIds = this.centralPlantIds();
    if (centralIds.size === 0) { return new Set<string>(); }
    const ids = this.recommendations()?.goodCompanions
      ?.filter(c => c.plantId && c.linkedPlantIds?.some(id => centralIds.has(id)))
      .map(c => c.plantId!) ?? [];
    return new Set(ids);
  });

  readonly availableMechanisms = computed(() => {
    const all = Object.keys(MECHANISM_KEY_MAP).map(Number);
    return all.sort((a, b) => {
      const keyA = this.translate.instant(`Plant.Mechanism.${MECHANISM_KEY_MAP[a] ?? ''}`);
      const keyB = this.translate.instant(`Plant.Mechanism.${MECHANISM_KEY_MAP[b] ?? ''}`);
      return keyA.localeCompare(keyB, 'fr');
    });
  });

  readonly filteredPlants = computed(() => {
    const query = this.searchQuery().toLowerCase();
    const sort = this.sortMode();
    const selectedIds = this.selectedPlantIds();

    let result = this.plantStore.allPlants().filter(p => !selectedIds.has(p.id));

    if (this.myPlantsOnly()) {
      const myIds = this.myPlantsStore.plantIds();
      result = result.filter(p => myIds.has(p.id));
    }

    const mFilter = this.mechanismFilter();
    if (mFilter !== null) {
      const assocMap = this.catalogAssociationMechanisms();
      result = result.filter(p => {
        if (p.intrinsicMechanisms?.includes(mFilter)) { return true; }
        const assoc = assocMap.get(p.id!);
        return assoc?.beneficial.includes(mFilter) || assoc?.harmful.includes(mFilter);
      });
    }

    const rdFilter = this.rootDepthFilter();
    if (rdFilter !== null) {
      result = result.filter(p => p.rootDepth === rdFilter);
    }

    if (query) {
      result = result.filter(p =>
        (p.name ?? '').toLowerCase().includes(query) ||
        (p.scientificName ?? '').toLowerCase().includes(query)
      );
    }

    const byName = (a: PlantDto, b: PlantDto) =>
      (a.name ?? '').localeCompare(b.name ?? '', 'fr');

    const myPlantIds = this.myPlantsStore.plantIds();
    const favFirst = (a: PlantDto, b: PlantDto) => {
      const aFav = myPlantIds.has(a.id) ? 0 : 1;
      const bFav = myPlantIds.has(b.id) ? 0 : 1;
      return aFav - bFav;
    };

    const sorted = [...result].sort((a, b) => {
      if (sort === 'family') {
        const famCmp = (a.family ?? '').localeCompare(b.family ?? '', 'fr');
        return famCmp !== 0 ? famCmp : byName(a, b);
      }
      if (sort === 'compat') {
        const centralCompanions = this.centralCompanionIds();
        const compatScore = (p: PlantDto) => {
          if (centralCompanions.has(p.id!)) {
            return -1;
          }
          if (this.goodCompanionIds().has(p.id)) {
            return 0;
          }
          if (myPlantIds.has(p.id)) {
            return 0.5;
          }
          if (this.avoidPlantIds().has(p.id)) {
            return 2;
          }
          return 1;
        };
        const scoreDiff = compatScore(a) - compatScore(b);
        return scoreDiff !== 0 ? scoreDiff : byName(a, b);
      }
      const fav = favFirst(a, b);
      return fav !== 0 ? fav : byName(a, b);
    });

    return sorted;
  });

  readonly intrinsicMechanismsByPlant = computed(() => {
    const map = new Map<string, number[]>();
    for (const entry of this.recommendations()?.intrinsicMechanismsByPlant ?? []) {
      if (!entry.plantId) { continue; }
      const sorted = [...(entry.mechanisms ?? [])].sort((a, b) => {
        const keyA = this.translate.instant(`Plant.Mechanism.${MECHANISM_KEY_MAP[a] ?? ''}`);
        const keyB = this.translate.instant(`Plant.Mechanism.${MECHANISM_KEY_MAP[b] ?? ''}`);
        return keyA.localeCompare(keyB, 'fr');
      });
      map.set(entry.plantId, sorted);
    }
    return map;
  });

  readonly relationalMechanismsByPlant = computed(() => {
    const map = new Map<string, number[]>();
    for (const entry of this.recommendations()?.selectedPlantsMechanisms ?? []) {
      if (!entry.plantId) { continue; }
      const sorted = [...(entry.mechanisms ?? [])].sort((a, b) => {
        const keyA = this.translate.instant(`Plant.Mechanism.${MECHANISM_KEY_MAP[a] ?? ''}`);
        const keyB = this.translate.instant(`Plant.Mechanism.${MECHANISM_KEY_MAP[b] ?? ''}`);
        return keyA.localeCompare(keyB, 'fr');
      });
      map.set(entry.plantId, sorted);
    }
    return map;
  });

  readonly guildIntrinsicMechanisms = computed(() => {
    const intrinsic = new Set(
      (this.recommendations()?.intrinsicMechanismsByPlant ?? [])
        .flatMap(entry => entry.mechanisms ?? [])
    );
    return [...intrinsic].sort((a, b) => {
      const keyA = this.translate.instant(`Plant.Mechanism.${MECHANISM_KEY_MAP[a] ?? ''}`);
      const keyB = this.translate.instant(`Plant.Mechanism.${MECHANISM_KEY_MAP[b] ?? ''}`);
      return keyA.localeCompare(keyB, 'fr');
    });
  });

  readonly guildRelationalOnlyMechanisms = computed(() => {
    const relational = this.recommendations()?.selectedPlantMechanisms ?? [];
    const intrinsicSet = new Set(
      (this.recommendations()?.intrinsicMechanismsByPlant ?? [])
        .flatMap(entry => entry.mechanisms ?? [])
    );
    return relational.filter(m => !intrinsicSet.has(m)).sort((a, b) => {
      const keyA = this.translate.instant(`Plant.Mechanism.${MECHANISM_KEY_MAP[a] ?? ''}`);
      const keyB = this.translate.instant(`Plant.Mechanism.${MECHANISM_KEY_MAP[b] ?? ''}`);
      return keyA.localeCompare(keyB, 'fr');
    });
  });

  readonly allGuildMechanisms = computed(() => {
    const intrinsic = new Set(this.guildIntrinsicMechanisms());
    for (const m of this.guildRelationalOnlyMechanisms()) {
      intrinsic.add(m);
    }
    return intrinsic;
  });

  readonly missingPriorityMechanisms = computed(() => {
    const covered = this.allGuildMechanisms();
    return PRIORITY_MECHANISMS.filter(m => !covered.has(m));
  });

  readonly mechanismProviders = computed(() => {
    const map = new Map<AssociationMechanism, PlantDto[]>();
    const seen = new Map<AssociationMechanism, Set<string>>();
    const addProvider = (plantId: string, mechanism: AssociationMechanism) => {
      if (!PRIORITY_MECHANISMS.includes(mechanism)) { return; }
      const seenIds = seen.get(mechanism) ?? new Set();
      if (seenIds.has(plantId)) { return; }
      seenIds.add(plantId);
      seen.set(mechanism, seenIds);
      const plant = this.plantStore.findById(plantId);
      if (!plant) { return; }
      const plants = map.get(mechanism) ?? [];
      plants.push(plant);
      map.set(mechanism, plants);
    };
    for (const entry of this.recommendations()?.intrinsicMechanismsByPlant ?? []) {
      if (!entry.plantId) { continue; }
      for (const m of entry.mechanisms ?? []) {
        addProvider(entry.plantId, m);
      }
    }
    for (const entry of this.recommendations()?.selectedPlantsMechanisms ?? []) {
      if (!entry.plantId) { continue; }
      for (const m of entry.mechanisms ?? []) {
        addProvider(entry.plantId, m);
      }
    }
    for (const [mechanism, plants] of map) {
      map.set(mechanism, plants.sort((a, b) => (a.name ?? '').localeCompare(b.name ?? '', 'fr')));
    }
    return map;
  });

  readonly emptyRootLayers = computed(() => {
    const groups = this.rootDepthGroups();
    const empty: RootDepth[] = [];
    if (!groups.has(RootDepth.Shallow)) { empty.push(RootDepth.Shallow); }
    if (!groups.has(RootDepth.Medium)) { empty.push(RootDepth.Medium); }
    if (!groups.has(RootDepth.Deep)) { empty.push(RootDepth.Deep); }
    return empty;
  });

  readonly familyDiversityWarnings = computed(() => {
    const plants = this.selectedPlants();
    if (plants.length < 3) { return []; }
    const familyCounts = new Map<string, number>();
    for (const p of plants) {
      if (!p.family) { continue; }
      familyCounts.set(p.family, (familyCounts.get(p.family) ?? 0) + 1);
    }
    const warnings: { family: string; count: number; total: number }[] = [];
    for (const [family, count] of familyCounts) {
      if (count >= 3 && count / plants.length > 0.4) {
        warnings.push({ family, count, total: plants.length });
      }
    }
    return warnings;
  });

  readonly harmfulAssociationPairs = computed(() => {
    const associations = this.recommendations()?.selectedPlantAssociations ?? [];
    const harmful = associations.filter(a => a.effect === AssociationEffect.Harmful);
    const seen = new Set<string>();
    const pairs: { plantA: string; plantB: string }[] = [];
    for (const a of harmful) {
      const key = [a.sourcePlantId, a.targetPlantId].sort().join('-');
      if (seen.has(key)) { continue; }
      seen.add(key);
      pairs.push({
        plantA: this.plantStore.findById(a.sourcePlantId)?.name ?? '',
        plantB: this.plantStore.findById(a.targetPlantId)?.name ?? '',
      });
    }
    return pairs;
  });

  readonly assistantGapCount = computed(() =>
    this.missingPriorityMechanisms().length
  );

  constructor() {
    effect(() => {
      const selected = this.selectedPlants();
      untracked(() => {
        if (selected.length === 0) {
          this.recommendations.set(null);
          return;
        }
        this.fetchRecommendations(selected);
      });
    });
  }

  addPlant(plant: PlantDto): void {
    if (this.selectedPlantIds().has(plant.id)) {
      return;
    }
    this.selectedPlants.update(list => [...list, plant]);
    if (this.guildMode() === 'viewing') {
      this.guildMode.set('editing');
    }
  }

  isCentralPlant(plantId: string | undefined): boolean {
    if (!plantId) { return false; }
    return this.centralPlantIds().has(plantId);
  }

  toggleCentralPlant(plantId: string): void {
    this.centralPlantIds.update(set => {
      const next = new Set(set);
      if (next.has(plantId)) {
        next.delete(plantId);
      } else {
        next.add(plantId);
      }
      return next;
    });
  }

  removePlant(plant: PlantDto): void {
    this.selectedPlants.update(list => list.filter(p => p.id !== plant.id));
    if (plant.id) {
      this.centralPlantIds.update(set => {
        const next = new Set(set);
        next.delete(plant.id!);
        return next;
      });
    }
    if (this.selectedPlants().length === 0) {
      this.centralPlantIds.set(new Set());
      this.editingGuild.set(null);
      this.guildName.set('');
      this.guildDescription.set('');
      this.guildMode.set('companions');
    } else if (this.guildMode() === 'viewing') {
      this.guildMode.set('editing');
    }
  }

  clearSelection(): void {
    this.selectedPlants.set([]);
    this.centralPlantIds.set(new Set());
    this.editingGuild.set(null);
    this.guildName.set('');
    this.guildDescription.set('');
    this.guildMode.set('companions');
  }

  loadGuildForEditing(guild: GuildDto): void {
    this.selectedPlants.set([]);
    this.editingGuild.set(guild);
    this.guildName.set(guild.name ?? '');
    this.guildDescription.set(guild.description ?? '');
    for (const guildPlant of guild.plants ?? []) {
      const plant = this.plantStore.allPlants().find(p => p.id === guildPlant.id);
      if (plant) {
        this.selectedPlants.update(list => [...list, plant]);
      }
    }
    const centralIds = new Set<string>(
      (guild.plants ?? [])
        .filter(gp => gp.role === GuildPlantRole.Central && gp.id)
        .map(gp => gp.id!)
    );
    this.centralPlantIds.set(centralIds);
    this.guildMode.set('viewing');
  }

  startNewGuild(): void {
    this.clearSelection();
  }

  startGuildCreation(): void {
    this.editingGuild.set({ id: undefined, name: '', description: '' });
    this.guildName.set('');
    this.guildDescription.set('');
    this.guildMode.set('creating');
  }

  startGuildEditing(): void {
    this.guildMode.set('editing');
  }

  cancelGuildEditing(): void {
    if (this.guildMode() === 'creating') {
      this.centralPlantIds.set(new Set());
      this.editingGuild.set(null);
      this.guildName.set('');
      this.guildDescription.set('');
      this.guildMode.set('companions');
    } else if (this.guildMode() === 'editing') {
      const guild = this.editingGuild();
      this.guildName.set(guild?.name ?? '');
      this.guildDescription.set(guild?.description ?? '');
      const originalPlants: PlantDto[] = [];
      for (const gp of guild?.plants ?? []) {
        const plant = this.plantStore.allPlants().find(p => p.id === gp.id);
        if (plant) {
          originalPlants.push(plant);
        }
      }
      this.selectedPlants.set(originalPlants);
      const centralIds = new Set<string>(
        (guild?.plants ?? [])
          .filter(gp => gp.role === GuildPlantRole.Central && gp.id)
          .map(gp => gp.id!)
      );
      this.centralPlantIds.set(centralIds);
      this.guildMode.set('viewing');
    }
  }

  async saveGuild(): Promise<void> {
    const plants: GuildPlantRequest[] = this.selectedPlants()
      .filter(p => !!p.id)
      .map(p => ({
        plantId: p.id!,
        role: this.centralPlantIds().has(p.id!) ? GuildPlantRole.Central : GuildPlantRole.Companion,
      }));
    if (plants.length === 0 || !this.guildName()) {
      return;
    }

    this.guildSaving.set(true);
    try {
      const editing = this.editingGuild();
      if (editing?.id && !editing.isOfficial) {
        const request: UpdateGuildRequest = {
          name: this.guildName(),
          description: this.guildDescription() || undefined,
          plants,
        };
        const updated = await this.guildService.update(editing.id, request);
        this.editingGuild.set(updated);
      } else {
        const request: CreateGuildRequest = {
          name: this.guildName(),
          description: this.guildDescription() || undefined,
          plants,
        };
        const created = await this.guildService.create(request);
        this.editingGuild.set(created);
      }
      this.guildMode.set('viewing');
      await this.guildStore.load();
    } finally {
      this.guildSaving.set(false);
    }
  }

  setSearch(query: string): void {
    this.searchQuery.set(query);
  }

  toggleMyPlantsOnly(): void {
    this.myPlantsOnly.update(v => !v);
  }

  toggleMechanismFilter(mechanism: number): void {
    this.mechanismFilter.update(v => v === mechanism ? null : mechanism);
  }

  toggleRootDepthFilter(depth: RootDepth): void {
    this.rootDepthFilter.update(v => v === depth ? null : depth);
  }

  setSort(mode: 'alpha' | 'family' | 'compat'): void {
    this.sortMode.set(mode);
  }

  isSelected(plant: PlantDto): boolean {
    return this.selectedPlantIds().has(plant.id);
  }

  getPlantEmoji(plant: PlantDto): string {
    return FAMILY_EMOJI_MAP[plant.family ?? ''] ?? '🌱';
  }

  getFamilyClass(family: string | undefined): string {
    return FAMILY_CLASS_MAP[family ?? ''] ?? '';
  }

  getMechanismKey(mechanism: AssociationMechanism): string {
    return MECHANISM_KEY_MAP[mechanism] ?? '';
  }

  getCompatibility(plantId: string | undefined): 'central-companion' | 'good' | 'avoid' | 'neutral' {
    if (!plantId || !this.recommendations()) {
      return 'neutral';
    }
    if (this.centralCompanionIds().has(plantId)) {
      return 'central-companion';
    }
    if (this.goodCompanionIds().has(plantId)) {
      return 'good';
    }
    if (this.avoidPlantIds().has(plantId)) {
      return 'avoid';
    }
    return 'neutral';
  }

  private async fetchRecommendations(plants: PlantDto[]): Promise<void> {
    this.loading.set(true);
    try {
      const request: CompanionRecommendationRequest = {
        plantIds: plants.map(p => p.id!).filter(Boolean),
        minScore: 0,
      };
      const result = await this.service.getRecommendations(request);
      this.recommendations.set(result);
    } catch {
      this.recommendations.set(null);
    } finally {
      this.loading.set(false);
    }
  }

}
