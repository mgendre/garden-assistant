import { Component, inject, signal, computed } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faPlus } from '@fortawesome/free-solid-svg-icons';
import { SearchInput } from '../../../shared/ui/search-input/search-input';
import { PlantDto } from '../../../api/garden-assistant-api';
import { PlantStore } from '../../../shared/services/plant.store';
import { MyPlantsStore } from '../../../shared/services/my-plants.store';
import { CompanionStore } from '../../../shared/services/companion.store';

@Component({
  selector: 'app-plant-picker',
  standalone: true,
  imports: [TranslateModule, FontAwesomeModule, SearchInput],
  templateUrl: './plant-picker.html',
  styleUrl: './plant-picker.scss'
})
export class PlantPicker {
  private readonly plantStore = inject(PlantStore);
  protected readonly myPlantsStore = inject(MyPlantsStore);
  protected readonly companionStore = inject(CompanionStore);
  protected readonly faPlus = faPlus;

  readonly searchQuery = signal('');

  readonly filteredPlants = computed(() => {
    const query = this.searchQuery().toLowerCase();
    const savedIds = this.myPlantsStore.plantIds();
    let result = this.plantStore.allPlants().filter(p => !savedIds.has(p.id));

    if (query) {
      result = result.filter(p =>
        (p.name ?? '').toLowerCase().includes(query) ||
        (p.scientificName ?? '').toLowerCase().includes(query)
      );
    }

    return [...result].sort((a, b) => (a.name ?? '').localeCompare(b.name ?? '', 'fr'));
  });

  onPlantClick(plant: PlantDto): void {
    this.myPlantsStore.toggle(plant);
  }
}
