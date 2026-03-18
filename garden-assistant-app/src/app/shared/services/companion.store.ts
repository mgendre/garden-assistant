import { Injectable, inject, signal, computed, effect, untracked } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import {
  PlantDto,
  CompanionSearchResultDto,
  CompanionRecommendationRequest,
  GuildDetailDto,
  AssociationMechanism,
  CreateGuildRequest,
  UpdateGuildRequest,
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
  readonly sortMode = signal<'alpha' | 'family' | 'compat'>('alpha');
  readonly myPlantsOnly = signal(false);
  readonly mechanismFilter = signal<number | null>(null);
  readonly recommendations = signal<CompanionSearchResultDto | null>(null);
  readonly loading = signal(false);

  readonly editingGuild = signal<GuildDetailDto | null>(null);
  readonly guildName = signal('');
  readonly guildDescription = signal('');
  readonly guildSaving = signal(false);
  readonly guildMode = signal<'companions' | 'creating' | 'viewing' | 'editing'>('companions');

  readonly isFormVisible = computed(() =>
    this.guildMode() === 'creating' || this.guildMode() === 'editing'
  );

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

  readonly availableMechanisms = computed(() => {
    const mechanisms = new Set<number>();
    for (const plant of this.plantStore.allPlants()) {
      for (const m of plant.intrinsicMechanisms ?? []) {
        mechanisms.add(m);
      }
    }
    for (const [, entry] of this.catalogAssociationMechanisms()) {
      for (const m of entry.beneficial) { mechanisms.add(m); }
      for (const m of entry.harmful) { mechanisms.add(m); }
    }
    return [...mechanisms].sort((a, b) => {
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
        const compatScore = (p: PlantDto) => {
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

    return sorted.slice(0, 20);
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

  removePlant(plant: PlantDto): void {
    this.selectedPlants.update(list => list.filter(p => p.id !== plant.id));
    if (this.selectedPlants().length === 0) {
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
    this.editingGuild.set(null);
    this.guildName.set('');
    this.guildDescription.set('');
    this.guildMode.set('companions');
  }

  loadGuildForEditing(guild: GuildDetailDto): void {
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
      this.guildMode.set('viewing');
    }
  }

  async saveGuild(): Promise<void> {
    const plantIds = this.selectedPlants().map(p => p.id!).filter(Boolean);
    if (plantIds.length === 0 || !this.guildName()) {
      return;
    }

    this.guildSaving.set(true);
    try {
      const editing = this.editingGuild();
      if (editing?.id && !editing.isOfficial) {
        const request: UpdateGuildRequest = {
          name: this.guildName(),
          description: this.guildDescription() || undefined,
          plantIds,
        };
        const updated = await this.guildService.update(editing.id, request);
        this.editingGuild.set(updated);
      } else {
        const request: CreateGuildRequest = {
          name: this.guildName(),
          description: this.guildDescription() || undefined,
          plantIds,
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

  getCompatibility(plantId: string | undefined): 'good' | 'avoid' | 'neutral' {
    if (!plantId || !this.recommendations()) {
      return 'neutral';
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
