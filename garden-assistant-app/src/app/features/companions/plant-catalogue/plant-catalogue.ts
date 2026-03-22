import { Component, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faHeart as faHeartSolid, faCircleInfo, faLink } from '@fortawesome/free-solid-svg-icons';
import { CompanionStore } from '../../../shared/services/companion.store';
import { MyPlantsStore } from '../../../shared/services/my-plants.store';
import { PlantStore } from '../../../shared/services/plant.store';
import { PlantDetailDialog, PlantDetailDialogData } from '../../../shared/ui/plant-detail-dialog/plant-detail-dialog';
import { BadgeInfoDialog, BadgeInfoDialogData } from '../../../shared/ui/badge-info-dialog/badge-info-dialog';
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
  private readonly dialog = inject(MatDialog);

  getRootDepthKey(rootDepth: RootDepth | undefined): string {
    switch (rootDepth) {
      case RootDepth.Shallow: return 'Plant.RootDepth.Shallow';
      case RootDepth.Medium: return 'Plant.RootDepth.Medium';
      case RootDepth.Deep: return 'Plant.RootDepth.Deep';
      default: return '';
    }
  }

  openPlantDetail(plant: PlantDto, event: Event): void {
    event.stopPropagation();
    this.dialog.open<PlantDetailDialog, PlantDetailDialogData>(PlantDetailDialog, {
      data: { plant },
      maxWidth: '600px',
      width: '90vw',
    });
  }

  openMechanismInfo(mechanism: number, event: Event): void {
    event.stopPropagation();
    const key = this.store.getMechanismKey(mechanism);
    if (key) {
      this.dialog.open<BadgeInfoDialog, BadgeInfoDialogData>(BadgeInfoDialog, {
        data: {
          titleKey: `BadgeInfo.Mechanism.${key}.Title`,
          descriptionKey: `BadgeInfo.Mechanism.${key}.Description`,
        },
        maxWidth: '400px',
      });
    }
  }
}
