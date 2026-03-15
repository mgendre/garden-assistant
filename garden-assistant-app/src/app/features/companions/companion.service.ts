import { Injectable, inject, signal, computed, effect, untracked } from '@angular/core';
import {
  PlantsClient,
  PlantAssociationsClient,
  GuildsClient,
  PlantDto,
  CompanionSearchResultDto,
  CompanionRecommendationRequest,
  GuildDetailDto,
  AssociationMechanism
} from '../../api/garden-assistant-api';

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
export class CompanionService {
  private readonly plantsClient = inject(PlantsClient);
  private readonly associationsClient = inject(PlantAssociationsClient);
  private readonly guildsClient = inject(GuildsClient);

  readonly plants = signal<PlantDto[]>([]);
  readonly selectedPlants = signal<PlantDto[]>([]);
  readonly searchQuery = signal('');
  readonly sortMode = signal<'alpha' | 'family'>('alpha');
  readonly recommendations = signal<CompanionSearchResultDto | null>(null);
  readonly loading = signal(false);
  readonly plantsLoading = signal(false);

  private static readonly MAX_VISIBLE_PLANTS = 15;

  readonly filteredPlants = computed(() => {
    const query = this.searchQuery().toLowerCase();
    const sort = this.sortMode();
    const selectedIds = this.selectedPlantIds();
    let result = this.plants().filter(p => !selectedIds.has(p.id));

    if (query) {
      result = result.filter(p =>
        (p.name?.toLowerCase().includes(query)) ||
        (p.scientificName?.toLowerCase().includes(query))
      );
    }

    const sorted = [...result].sort((a, b) => {
      if (sort === 'family') {
        const famCmp = (a.family ?? '').localeCompare(b.family ?? '', 'fr');
        return famCmp !== 0 ? famCmp : (a.name ?? '').localeCompare(b.name ?? '', 'fr');
      }
      return (a.name ?? '').localeCompare(b.name ?? '', 'fr');
    });

    return sorted.slice(0, CompanionService.MAX_VISIBLE_PLANTS);
  });

  readonly selectedPlantIds = computed(() =>
    new Set(this.selectedPlants().map(p => p.id))
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

  async loadPlants(): Promise<void> {
    this.plantsLoading.set(true);
    try {
      const plants = await this.plantsClient.getAll();
      this.plants.set(plants);
    } finally {
      this.plantsLoading.set(false);
    }
  }

  addPlant(plant: PlantDto): void {
    if (this.selectedPlantIds().has(plant.id)) return;
    this.selectedPlants.update(list => [...list, plant]);
  }

  removePlant(plant: PlantDto): void {
    this.selectedPlants.update(list => list.filter(p => p.id !== plant.id));
  }

  togglePlant(plant: PlantDto): void {
    if (this.selectedPlantIds().has(plant.id)) {
      this.removePlant(plant);
    } else {
      this.addPlant(plant);
    }
  }

  clearSelection(): void {
    this.selectedPlants.set([]);
  }

  setSearch(query: string): void {
    this.searchQuery.set(query);
  }

  setSort(mode: 'alpha' | 'family'): void {
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

  async loadGuildPlants(guildId: string): Promise<GuildDetailDto> {
    return this.guildsClient.getById(guildId);
  }

  private async fetchRecommendations(plants: PlantDto[]): Promise<void> {
    this.loading.set(true);
    try {
      const request: CompanionRecommendationRequest = {
        plantIds: plants.map(p => p.id!).filter(Boolean)
      };
      const result = await this.associationsClient.getCompanionRecommendations(request);
      this.recommendations.set(result);
    } catch {
      this.recommendations.set(null);
    } finally {
      this.loading.set(false);
    }
  }
}
