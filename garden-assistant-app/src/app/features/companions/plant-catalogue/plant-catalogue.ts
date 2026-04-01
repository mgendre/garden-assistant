import { Component, computed, inject, input, signal } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faHeart as faHeartSolid, faCircleInfo, faLink } from '@fortawesome/free-solid-svg-icons';
import { CompanionStore } from '../../../shared/services/companion.store';
import { MyPlantsStore } from '../../../shared/services/my-plants.store';
import { PlantStore } from '../../../shared/services/plant.store';
import { DialogService } from '../../../shared/services/dialog.service';
import { PlantDialogService } from '../../../shared/services/plant-dialog.service';
import { SearchInput } from '../../../shared/ui/search-input/search-input';
import { PlantDto, RootDepth } from '../../../api/garden-assistant-api';

@Component({
  selector: 'app-plant-catalogue',
  standalone: true,
  imports: [TranslateModule, FontAwesomeModule, SearchInput],
  templateUrl: './plant-catalogue.html',
  styleUrl: './plant-catalogue.scss'
})
export class PlantCatalogue {
  protected readonly store = inject(CompanionStore);
  protected readonly myPlantsStore = inject(MyPlantsStore);
  protected readonly plantStore = inject(PlantStore);
  protected readonly faHeartSolid = faHeartSolid;
  protected readonly faInfo = faCircleInfo;
  protected readonly faLink = faLink;
  protected readonly rootDepths = [
    { value: RootDepth.Shallow, labelKey: 'Plant.RootDepth.Shallow' },
    { value: RootDepth.Medium, labelKey: 'Plant.RootDepth.Medium' },
    { value: RootDepth.Deep, labelKey: 'Plant.RootDepth.Deep' },
  ];
  private readonly dialogService = inject(DialogService);
  private readonly plantDialogService = inject(PlantDialogService);

  readonly mode = input<'association' | 'collection'>('association');

  readonly collectionSearch = signal('');

  readonly collectionPlants = computed(() => {
    const query = this.collectionSearch().toLowerCase();
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

  readonly displayedPlants = computed(() => {
    return this.mode() === 'collection' ? this.collectionPlants() : this.store.filteredPlants();
  });

  onSearch(query: string): void {
    if (this.mode() === 'collection') {
      this.collectionSearch.set(query);
    } else {
      this.store.setSearch(query);
    }
  }

  onPlantClick(plant: PlantDto): void {
    if (this.mode() === 'collection') {
      this.myPlantsStore.toggle(plant);
    } else {
      this.store.addPlant(plant);
    }
  }

  getRootDepthKey(rootDepth: RootDepth | undefined): string {
    switch (rootDepth) {
      case RootDepth.Shallow: return 'Plant.RootDepth.Shallow';
      case RootDepth.Medium: return 'Plant.RootDepth.Medium';
      case RootDepth.Deep: return 'Plant.RootDepth.Deep';
      default: return '';
    }
  }

  private getRootDepthBadgeKey(rootDepth: RootDepth | undefined): string {
    switch (rootDepth) {
      case RootDepth.Shallow: return 'Shallow';
      case RootDepth.Medium: return 'Medium';
      case RootDepth.Deep: return 'Deep';
      default: return '';
    }
  }

  openPlantDetail(plant: PlantDto, event: Event): void {
    event.stopPropagation();
    this.plantDialogService.openDetail(plant);
  }

  openMechanismInfo(mechanism: number, event: Event): void {
    event.stopPropagation();
    const key = this.store.getMechanismKey(mechanism);
    if (key) {
      this.dialogService.openBadgeInfo(
        `BadgeInfo.Mechanism.${key}.Title`,
        `BadgeInfo.Mechanism.${key}.Description`
      );
    }
  }

  openRootDepthInfo(rootDepth: RootDepth | undefined, event: Event): void {
    event.stopPropagation();
    const key = this.getRootDepthBadgeKey(rootDepth);
    if (key) {
      this.dialogService.openBadgeInfo(
        `BadgeInfo.RootDepth.${key}.Title`,
        `BadgeInfo.RootDepth.${key}.Description`
      );
    }
  }
}
