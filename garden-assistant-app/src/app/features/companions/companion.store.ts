import { Injectable, inject, signal, computed, effect, untracked, DestroyRef } from '@angular/core';
import {
  PlantDto,
  CompanionSearchResultDto,
  CompanionRecommendationRequest,
  GuildDetailDto,
  GuildInfoDto,
  AssociationMechanism,
} from '../../api/garden-assistant-api';
import { CompanionService } from './companion.service';
import { MyPlantsStore } from '../my-plants/my-plants.store';

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
  private readonly myPlantsStore = inject(MyPlantsStore);

  private readonly destroyRef = inject(DestroyRef);
  private searchDebounceTimer: ReturnType<typeof setTimeout> | null = null;
  private searchInitialized = false;

  readonly plants = signal<PlantDto[]>([]);
  readonly totalCount = signal(0);
  readonly selectedPlants = signal<PlantDto[]>([]);
  readonly searchQuery = signal('');
  readonly sortMode = signal<'alpha' | 'family' | 'compat'>('alpha');
  readonly recommendations = signal<CompanionSearchResultDto | null>(null);
  readonly loading = signal(false);
  readonly plantsLoading = signal(false);
  readonly guildDetails = signal<Map<string, GuildDetailDto>>(new Map());
  readonly guildLoading = signal<string | null>(null);


  readonly selectedPlantIds = computed(() =>
    new Set(this.selectedPlants().map(p => p.id))
  );

  readonly avoidPlantIds = computed(() =>
    new Set(this.recommendations()?.plantsToAvoid?.map(p => p.plantId).filter(Boolean) ?? [])
  );

  readonly goodCompanions = computed(() => {
    const avoid = this.avoidPlantIds();
    const selected = this.selectedPlantIds();
    const myPlants = this.myPlantsStore.plantIds();
    const filtered = this.recommendations()?.goodCompanions
      ?.filter(c => !avoid.has(c.plantId) && !selected.has(c.plantId)) ?? [];
    return [...filtered].sort((a, b) => {
      const aFav = myPlants.has(a.plantId) ? 0 : 1;
      const bFav = myPlants.has(b.plantId) ? 0 : 1;
      if (aFav !== bFav) {
        return aFav - bFav;
      }
      return (a.plantName ?? '').localeCompare(b.plantName ?? '', 'fr');
    });
  });

  readonly goodCompanionIds = computed(() =>
    new Set(this.goodCompanions().map(c => c.plantId).filter(Boolean))
  );

  readonly filteredPlants = computed(() => {
    const sort = this.sortMode();
    const selectedIds = this.selectedPlantIds();
    const result = this.plants().filter(p => !selectedIds.has(p.id));

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
          if (this.goodCompanionIds().has(p.id)) return 0;
          if (myPlantIds.has(p.id)) return 0.5;
          if (this.avoidPlantIds().has(p.id)) return 2;
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

  readonly guildsForSelectedPlants = computed(() => {
    const guilds = new Map<string, GuildDetailDto>();
    for (const plant of this.selectedPlants()) {
      for (const guild of this.getGuildsForPlant(plant.id)) {
        if (guild.id) guilds.set(guild.id, guild);
      }
    }
    return Array.from(guilds.values());
  });

  constructor() {
    effect(() => {
      const selected = this.selectedPlants();
      untracked(() => {
        if (selected.length === 0) {
          this.recommendations.set(null);
          this.guildDetails.set(new Map());
          return;
        }
        this.fetchRecommendations(selected);
      });
    });

    effect(() => {
      const recs = this.recommendations();
      untracked(() => this.loadAllGuildDetails(recs));
    });

    effect(() => {
      const query = this.searchQuery();
      untracked(() => {
        if (!this.searchInitialized) {
          this.searchInitialized = true;
          return;
        }
        this.debouncedLoadPlants(query);
      });
    });

    this.destroyRef.onDestroy(() => {
      if (this.searchDebounceTimer) {
        clearTimeout(this.searchDebounceTimer);
      }
    });
  }

  async loadPlants(search?: string): Promise<void> {
    this.plantsLoading.set(true);
    try {
      const result = await this.service.getPlants(search || undefined);
      this.plants.set(result.items ?? []);
      this.totalCount.set(result.totalCount ?? 0);
    } finally {
      this.plantsLoading.set(false);
    }
  }

  private debouncedLoadPlants(query: string): void {
    if (this.searchDebounceTimer) {
      clearTimeout(this.searchDebounceTimer);
    }
    this.searchDebounceTimer = setTimeout(() => {
      this.loadPlants(query);
    }, 300);
  }

  addPlant(plant: PlantDto): void {
    if (this.selectedPlantIds().has(plant.id)) return;
    this.selectedPlants.update(list => [...list, plant]);
  }

  removePlant(plant: PlantDto): void {
    this.selectedPlants.update(list => list.filter(p => p.id !== plant.id));
  }

  clearSelection(): void {
    this.selectedPlants.set([]);
  }

  setSearch(query: string): void {
    this.searchQuery.set(query);
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

  getPlantEmojiById(plantId: string | undefined): string {
    if (!plantId) return '🌱';
    const plant = this.plants().find(p => p.id === plantId);
    return plant ? this.getPlantEmoji(plant) : '🌱';
  }

  getFamilyClass(family: string | undefined): string {
    return FAMILY_CLASS_MAP[family ?? ''] ?? '';
  }

  getMechanismKey(mechanism: AssociationMechanism): string {
    return MECHANISM_KEY_MAP[mechanism] ?? '';
  }

  getCompatibility(plantId: string | undefined): 'good' | 'avoid' | 'neutral' {
    if (!plantId || !this.recommendations()) return 'neutral';
    if (this.goodCompanionIds().has(plantId)) return 'good';
    if (this.avoidPlantIds().has(plantId)) return 'avoid';
    return 'neutral';
  }

  getGuildsForPlant(plantId: string | undefined): GuildDetailDto[] {
    if (!plantId) return [];
    return Array.from(this.guildDetails().values())
      .filter(guild => guild.plants?.some(p => p.id === plantId));
  }

  async addGuild(guild: GuildInfoDto | GuildDetailDto): Promise<void> {
    const guildId = guild.id;
    if (!guildId || this.guildLoading()) return;
    this.guildLoading.set(guildId);
    try {
      const detail = this.guildDetails().get(guildId) ?? await this.service.getGuildById(guildId);
      for (const guildPlant of detail.plants ?? []) {
        const plant = this.plants().find(p => p.id === guildPlant.id);
        if (plant) this.addPlant(plant);
      }
    } finally {
      this.guildLoading.set(null);
    }
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

  private async loadAllGuildDetails(recs: CompanionSearchResultDto | null): Promise<void> {
    if (!recs?.goodCompanions) return;
    const current = this.guildDetails();
    const toLoad: string[] = [];
    for (const companion of recs.goodCompanions) {
      for (const guild of companion.guilds ?? []) {
        if (guild.id && !current.has(guild.id) && !toLoad.includes(guild.id)) {
          toLoad.push(guild.id);
        }
      }
    }
    if (toLoad.length === 0) return;
    const results = await Promise.all(toLoad.map(id => this.service.getGuildById(id)));
    this.guildDetails.update(map => {
      const next = new Map(map);
      for (const detail of results) {
        if (detail.id) next.set(detail.id, detail);
      }
      return next;
    });
  }
}
