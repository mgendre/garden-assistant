import { inject, Injectable, signal } from '@angular/core';
import {
  AssociationEffect,
  PlantAssociationsClient,
  PlantAssociationDto,
  PlantDto,
  PlantsClient
} from '../../api/garden-assistant-api';

@Injectable()
export class CompanionSearchService {
  private readonly plantsClient = inject(PlantsClient);
  private readonly associationsClient = inject(PlantAssociationsClient);
  private readonly cache = new Map<string, PlantAssociationDto[]>();

  readonly plants = signal<PlantDto[]>([]);
  readonly selectedPlants = signal<PlantDto[]>([]);
  readonly associations = signal<PlantAssociationDto[]>([]);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  async loadPlants(): Promise<void> {
    this.loading.set(true);
    this.error.set(null);
    try {
      this.plants.set(await this.plantsClient.getAll() ?? []);
    } catch {
      this.error.set('Impossible de charger les plantes.');
    } finally {
      this.loading.set(false);
    }
  }

  async togglePlant(plant: PlantDto): Promise<void> {
    const current = this.selectedPlants();
    const isSelected = current.some(p => p.id === plant.id);

    const next = isSelected
      ? current.filter(p => p.id !== plant.id)
      : [...current, plant];

    this.selectedPlants.set(next);
    await this.refreshAssociations(next);
  }

  private async refreshAssociations(selected: PlantDto[]): Promise<void> {
    if (selected.length < 2) {
      this.associations.set([]);
      return;
    }

    this.loading.set(true);
    this.error.set(null);
    try {
      await this.fetchMissingAssociations(selected);

      const selectedIds = new Set(selected.map(p => p.id!));
      const seen = new Set<string>();
      const result: PlantAssociationDto[] = [];

      for (const plant of selected) {
        for (const assoc of this.cache.get(plant.id!) ?? []) {
          if (seen.has(assoc.id!)) continue;
          if (selectedIds.has(assoc.sourcePlantId!) && selectedIds.has(assoc.targetPlantId!)) {
            seen.add(assoc.id!);
            result.push(assoc);
          }
        }
      }

      result.sort((a, b) => (a.effect ?? 0) - (b.effect ?? 0));
      this.associations.set(result);
    } catch {
      this.error.set('Impossible de charger les associations.');
    } finally {
      this.loading.set(false);
    }
  }

  private async fetchMissingAssociations(plants: PlantDto[]): Promise<void> {
    const missing = plants.filter(p => !this.cache.has(p.id!));
    await Promise.all(
      missing.map(async p => {
        const result = await this.associationsClient.getForPlant(p.id!);
        this.cache.set(p.id!, result ?? []);
      })
    );
  }

  isSelected(plant: PlantDto): boolean {
    return this.selectedPlants().some(p => p.id === plant.id);
  }

  score(): { beneficial: number; harmful: number; neutral: number } {
    const assocs = this.associations();
    return {
      beneficial: assocs.filter(a => a.effect === AssociationEffect.Beneficial).length,
      harmful: assocs.filter(a => a.effect === AssociationEffect.Harmful).length,
      neutral: assocs.filter(a => a.effect === AssociationEffect.Neutral).length,
    };
  }
}
