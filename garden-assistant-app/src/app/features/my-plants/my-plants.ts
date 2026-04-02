import { Component, computed, inject, signal } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { MyPlantsStore } from '../../shared/services/my-plants.store';
import { CompanionStore } from '../../shared/services/companion.store';
import { PlantCatalogue } from '../companions/plant-catalogue/plant-catalogue';
import { EmptyState } from '../../shared/ui/empty-state/empty-state';
import { SearchInput } from '../../shared/ui/search-input/search-input';
import { PlantDialogService } from '../../shared/services/plant-dialog.service';
import { PlantDto } from '../../api/garden-assistant-api';

@Component({
  selector: 'app-my-plants',
  standalone: true,
  imports: [TranslateModule, PlantCatalogue, EmptyState, SearchInput],
  templateUrl: './my-plants.html',
  styleUrl: './my-plants.scss'
})
export class MyPlants {
  protected readonly myPlantsStore = inject(MyPlantsStore);
  protected readonly store = inject(CompanionStore);
  private readonly plantDialogService = inject(PlantDialogService);

  readonly searchQuery = signal('');

  readonly filteredPlants = computed(() => {
    const query = this.searchQuery().toLowerCase();
    let plants = this.myPlantsStore.sortedPlants();

    if (query) {
      plants = plants.filter(p =>
        (p.name ?? '').toLowerCase().includes(query) ||
        (p.scientificName ?? '').toLowerCase().includes(query) ||
        (p.family ?? '').toLowerCase().includes(query)
      );
    }

    return plants;
  });

  openPlantDetail(plant: PlantDto): void {
    this.plantDialogService.openDetail(plant);
  }

  removePlant(plant: PlantDto, event: Event): void {
    event.stopPropagation();
    this.myPlantsStore.toggle(plant);
  }
}
