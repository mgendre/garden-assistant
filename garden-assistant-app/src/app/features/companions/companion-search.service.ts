import { inject, Injectable, signal } from '@angular/core';
import {
  CompanionRecommendationRequest,
  CompanionSearchResultDto,
  PlantAssociationsClient,
  PlantDto,
  PlantsClient,
} from '../../api/garden-assistant-api';

@Injectable()
export class CompanionSearchService {
  private readonly plantsClient = inject(PlantsClient);
  private readonly associationsClient = inject(PlantAssociationsClient);

  readonly searchResults = signal<PlantDto[]>([]);
  readonly selectedPlants = signal<PlantDto[]>([]);
  readonly companionResults = signal<CompanionSearchResultDto | null>(null);
  readonly searchLoading = signal(false);
  readonly companionsLoading = signal(false);
  readonly error = signal<string | null>(null);

  async searchPlants(query: string): Promise<void> {
    this.searchLoading.set(true);
    this.error.set(null);
    try {
      const results = await this.plantsClient.search(query);
      this.searchResults.set(results);
    } catch {
      this.error.set('Erreur lors de la recherche.');
    } finally {
      this.searchLoading.set(false);
    }
  }

  async addPlant(plant: PlantDto): Promise<void> {
    if (this.selectedPlants().some(p => p.id === plant.id)) return;
    this.selectedPlants.update(plants => [...plants, plant]);
    this.searchResults.set([]);
    await this.refreshCompanions();
  }

  async removePlant(plant: PlantDto): Promise<void> {
    this.selectedPlants.update(plants => plants.filter(p => p.id !== plant.id));
    if (this.selectedPlants().length === 0) {
      this.companionResults.set(null);
      return;
    }
    await this.refreshCompanions();
  }

  clearAll(): void {
    this.selectedPlants.set([]);
    this.companionResults.set(null);
    this.error.set(null);
  }

  isSelected(plant: PlantDto): boolean {
    return this.selectedPlants().some(p => p.id === plant.id);
  }

  private async refreshCompanions(): Promise<void> {
    const plantIds = this.selectedPlants().map(p => p.id!);
    if (plantIds.length === 0) return;

    this.companionsLoading.set(true);
    this.error.set(null);
    try {
      const request: CompanionRecommendationRequest = { plantIds };
      const result = await this.associationsClient.getCompanionRecommendations(request);
      this.companionResults.set(result);
    } catch {
      this.error.set('Erreur lors de l\'analyse des associations.');
    } finally {
      this.companionsLoading.set(false);
    }
  }
}
