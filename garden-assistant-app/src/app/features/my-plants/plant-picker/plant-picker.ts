import { Component, inject, signal, computed, effect, untracked, DestroyRef } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faMagnifyingGlass, faPlus } from '@fortawesome/free-solid-svg-icons';
import { PlantDto } from '../../../api/garden-assistant-api';
import { CompanionService } from '../../companions/companion.service';
import { CompanionStore } from '../../companions/companion.store';
import { MyPlantsStore } from '../my-plants.store';

@Component({
  selector: 'app-plant-picker',
  standalone: true,
  imports: [TranslateModule, FontAwesomeModule],
  templateUrl: './plant-picker.html',
  styleUrl: './plant-picker.scss'
})
export class PlantPicker {
  protected readonly companionStore = inject(CompanionStore);
  protected readonly myPlantsStore = inject(MyPlantsStore);
  private readonly companionService = inject(CompanionService);
  private readonly destroyRef = inject(DestroyRef);
  protected readonly faSearch = faMagnifyingGlass;
  protected readonly faPlus = faPlus;

  readonly searchQuery = signal('');
  readonly plants = signal<PlantDto[]>([]);
  readonly totalCount = signal(0);
  readonly loading = signal(false);
  private debounceTimer: ReturnType<typeof setTimeout> | null = null;
  private searchInitialized = false;

  readonly filteredPlants = computed(() => {
    const savedIds = this.myPlantsStore.plantIds();
    const result = this.plants().filter(p => !savedIds.has(p.id));
    return [...result].sort((a, b) => (a.name ?? '').localeCompare(b.name ?? '', 'fr'));
  });

  constructor() {
    effect(() => {
      const query = this.searchQuery();
      untracked(() => {
        if (!this.searchInitialized) {
          this.searchInitialized = true;
          return;
        }
        this.debouncedSearch(query);
      });
    });

    this.destroyRef.onDestroy(() => {
      if (this.debounceTimer) {
        clearTimeout(this.debounceTimer);
      }
    });

    this.loadPlants();
  }

  onPlantClick(plant: PlantDto): void {
    this.myPlantsStore.toggle(plant);
  }

  private debouncedSearch(query: string): void {
    if (this.debounceTimer) {
      clearTimeout(this.debounceTimer);
    }
    this.debounceTimer = setTimeout(() => {
      this.loadPlants(query);
    }, 300);
  }

  private async loadPlants(search?: string): Promise<void> {
    this.loading.set(true);
    try {
      const result = await this.companionService.getPlants(search || undefined);
      this.plants.set(result.items ?? []);
      this.totalCount.set(result.totalCount ?? 0);
    } finally {
      this.loading.set(false);
    }
  }
}
