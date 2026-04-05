import { Component, computed, inject, input, signal } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faHeart as faHeartSolid, faCircleInfo, faLink } from '@fortawesome/free-solid-svg-icons';
import { CompanionStore } from '../../../shared/services/companion.store';
import { MyPlantsStore } from '../../../shared/services/my-plants.store';
import { PlantStore } from '../../../shared/services/plant.store';
import { DialogService } from '../../../shared/services/dialog.service';
import { PlantDialogService } from '../../../shared/services/plant-dialog.service';
import { SearchInput } from '../../../shared/ui/search-input/search-input';
import { Collapsible } from '../../../shared/ui/collapsible/collapsible';
import { PlantDto, RootDepth, AssociationMechanism } from '../../../api/garden-assistant-api';
import { EmptyState } from '../../../shared/ui/empty-state/empty-state';
import { RatingDetailDialog, RatingDetailData } from '../../../shared/ui/rating-detail-dialog/rating-detail-dialog';

@Component({
  selector: 'app-plant-catalogue',
  standalone: true,
  imports: [TranslateModule, FontAwesomeModule, SearchInput, EmptyState, Collapsible],
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
  private readonly dialog = inject(MatDialog);
  private readonly dialogService = inject(DialogService);
  private readonly plantDialogService = inject(PlantDialogService);

  readonly mode = input<'association' | 'collection'>('association');

  protected readonly rootDepths = [
    { value: RootDepth.Shallow, labelKey: 'Plant.RootDepth.Shallow' },
    { value: RootDepth.Medium, labelKey: 'Plant.RootDepth.Medium' },
    { value: RootDepth.Deep, labelKey: 'Plant.RootDepth.Deep' },
  ];
  protected readonly soilTypes = ['Sandy', 'Silty', 'Clay', 'Loam', 'Chalky', 'Peaty', 'Rocky'];

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

  openMechanismInfo(mechanism: AssociationMechanism, event: Event): void {
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

  getRating(plant: PlantDto): number | null {
    if (this.store.selectedPlants().length === 0) { return null; }
    if (this.store.isSelected(plant)) { return null; }
    const rec = this.store.recommendations()?.goodCompanions?.find(c => c.plantId === plant.id);
    if (rec) { return rec.rating ?? 3; }
    return 3;
  }

  readonly hasActiveFilters = computed(() => {
    return this.store.mechanismFilter() !== null ||
      this.store.rootDepthFilter() !== null ||
      this.store.soilTypeFilter() !== null ||
      !this.store.phCompatibleFilter() ||
      this.store.myPlantsOnly();
  });

  openRatingDetail(plant: PlantDto, event: Event): void {
    event.stopPropagation();
    const rec = this.store.recommendations()?.goodCompanions?.find(c => c.plantId === plant.id);
    if (!rec) { return; }

    const badgeKeys = (rec.mechanisms ?? []).map(m => this.store.getMechanismKey(m));
    const mechanismLabels = badgeKeys.map(k => 'Plant.Mechanism.' + k);

    const harmfulBadgeKeys = (rec.harmfulMechanisms ?? []).map(m => this.store.getMechanismKey(m));
    const harmfulLabels = harmfulBadgeKeys.map(k => 'Plant.Mechanism.' + k);

    this.dialog.open(RatingDetailDialog, {
      data: {
        plantName: plant.name ?? '',
        rating: rec.rating ?? 3,
        score: rec.score ?? 0,
        mechanisms: mechanismLabels,
        mechanismBadgeKeys: badgeKeys,
        harmfulMechanisms: harmfulLabels,
        harmfulMechanismBadgeKeys: harmfulBadgeKeys,
        hasRootDepthBonus: rec.hasRootDepthBonus ?? false,
        hasSameFamilyMalus: rec.hasSameFamilyMalus ?? false,
        hasWaterIncompatibility: rec.hasWaterIncompatibility ?? false,
        isCentralCompanion: this.store.centralCompanionIds().has(plant.id!),
      } as RatingDetailData,
      maxWidth: '400px',
      width: '90vw',
    });
  }

  resetFilters(): void {
    this.store.mechanismFilter.set(null);
    this.store.rootDepthFilter.set(null);
    this.store.soilTypeFilter.set(null);
    this.store.phCompatibleFilter.set(true);
    if (this.store.myPlantsOnly()) {
      this.store.toggleMyPlantsOnly();
    }
  }
}
