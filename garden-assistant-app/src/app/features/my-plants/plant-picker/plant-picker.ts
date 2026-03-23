import { Component, inject, signal, computed } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faCircleInfo } from '@fortawesome/free-solid-svg-icons';
import { SearchInput } from '../../../shared/ui/search-input/search-input';
import { PlantDto } from '../../../api/garden-assistant-api';
import { PlantStore } from '../../../shared/services/plant.store';
import { MyPlantsStore } from '../../../shared/services/my-plants.store';
import { CompanionStore } from '../../../shared/services/companion.store';
import { PlantDetailDialog, PlantDetailDialogData } from '../../../shared/ui/plant-detail-dialog/plant-detail-dialog';

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
  private readonly dialog = inject(MatDialog);
  protected readonly faInfo = faCircleInfo;

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

  addPlant(plant: PlantDto): void {
    this.myPlantsStore.toggle(plant);
  }

  openPlantDetail(plant: PlantDto, event: Event): void {
    event.stopPropagation();
    this.dialog.open<PlantDetailDialog, PlantDetailDialogData>(PlantDetailDialog, {
      data: { plant },
      maxWidth: '600px',
      width: '90vw',
    });
  }
}
