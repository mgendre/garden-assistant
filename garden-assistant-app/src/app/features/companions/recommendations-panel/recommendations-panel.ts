import { Component, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faHeart as faHeartSolid, faCircleInfo } from '@fortawesome/free-solid-svg-icons';
import { CompanionStore } from '../../../shared/services/companion.store';
import { PlantStore } from '../../../shared/services/plant.store';
import { CompanionRecommendationDto } from '../../../api/garden-assistant-api';
import { MyPlantsStore } from '../../../shared/services/my-plants.store';
import { BadgeInfoDialog, BadgeInfoDialogData } from '../../../shared/ui/badge-info-dialog/badge-info-dialog';
import { PlantDetailDialog, PlantDetailDialogData } from '../../../shared/ui/plant-detail-dialog/plant-detail-dialog';

@Component({
  selector: 'app-recommendations-panel',
  standalone: true,
  imports: [TranslateModule, FontAwesomeModule],
  templateUrl: './recommendations-panel.html',
  styleUrl: './recommendations-panel.scss'
})
export class RecommendationsPanel {
  protected readonly store = inject(CompanionStore);
  protected readonly myPlantsStore = inject(MyPlantsStore);
  protected readonly faHeartSolid = faHeartSolid;
  protected readonly faInfo = faCircleInfo;
  private readonly dialog = inject(MatDialog);
  private readonly plantStore = inject(PlantStore);

  onCompanionClick(companion: CompanionRecommendationDto): void {
    const plant = this.plantStore.allPlants().find(p => p.id === companion.plantId);
    if (plant) {
      this.store.addPlant(plant);
    }
  }

  getMechanismTranslationKey(mechanism: number): string {
    const key = this.store.getMechanismKey(mechanism);
    return key ? `Plant.Mechanism.${key}` : '';
  }

  openPlantDetail(plantId: string | undefined, event: Event): void {
    event.stopPropagation();
    if (!plantId) {
      return;
    }
    const plant = this.plantStore.allPlants().find(p => p.id === plantId);
    if (plant) {
      this.dialog.open<PlantDetailDialog, PlantDetailDialogData>(PlantDetailDialog, {
        data: { plant },
        maxWidth: '500px',
        width: '90vw',
      });
    }
  }

  openMechanismInfo(mechanism: number): void {
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
