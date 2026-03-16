import { Component, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faMagnifyingGlass, faHeart as faHeartSolid, faCircleInfo } from '@fortawesome/free-solid-svg-icons';
import { CompanionStore } from '../companion.store';
import { MyPlantsStore } from '../../my-plants/my-plants.store';
import { PlantDetailDialog, PlantDetailDialogData } from '../../../shared/ui/plant-detail-dialog/plant-detail-dialog';
import { PlantDto } from '../../../api/garden-assistant-api';

@Component({
  selector: 'app-plant-catalogue',
  standalone: true,
  imports: [TranslateModule, FontAwesomeModule],
  templateUrl: './plant-catalogue.html',
  styleUrl: './plant-catalogue.scss'
})
export class PlantCatalogue {
  protected readonly store = inject(CompanionStore);
  protected readonly myPlantsStore = inject(MyPlantsStore);
  protected readonly faSearch = faMagnifyingGlass;
  protected readonly faHeartSolid = faHeartSolid;
  protected readonly faInfo = faCircleInfo;
  private readonly dialog = inject(MatDialog);

  openPlantDetail(plant: PlantDto, event: Event): void {
    event.stopPropagation();
    this.dialog.open<PlantDetailDialog, PlantDetailDialogData>(PlantDetailDialog, {
      data: { plant },
      maxWidth: '500px',
      width: '90vw',
    });
  }
}
