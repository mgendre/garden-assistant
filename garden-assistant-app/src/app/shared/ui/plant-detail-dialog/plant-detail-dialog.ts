import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faHeart as faHeartSolid } from '@fortawesome/free-solid-svg-icons';
import { faHeart as faHeartRegular } from '@fortawesome/free-regular-svg-icons';
import { PlantDto } from '../../../api/garden-assistant-api';
import { MyPlantsStore } from '../../services/my-plants.store';
import { DialogService } from '../../services/dialog.service';
import { PlantCard } from '../plant-card/plant-card';

export interface PlantDetailDialogData {
  plant: PlantDto;
}

@Component({
  selector: 'app-plant-detail-dialog',
  standalone: true,
  imports: [MatDialogModule, TranslateModule, FontAwesomeModule, PlantCard],
  templateUrl: './plant-detail-dialog.html',
  styleUrl: './plant-detail-dialog.scss'
})
export class PlantDetailDialog {
  readonly data = inject<PlantDetailDialogData>(MAT_DIALOG_DATA);
  private readonly translate = inject(TranslateService);
  private readonly dialogService = inject(DialogService);
  readonly myPlantsStore = inject(MyPlantsStore);

  protected readonly faHeartSolid = faHeartSolid;
  protected readonly faHeartRegular = faHeartRegular;

  async toggleFav(): Promise<void> {
    const p = this.data.plant;
    if (this.myPlantsStore.isSaved(p.id)) {
      const message = this.translate.instant('MyPlants.ConfirmRemoveMessage', { name: p.name });
      const confirmed = await this.dialogService.confirm(
        this.translate.instant('MyPlants.ConfirmRemoveTitle'),
        message,
        this.translate.instant('MyPlants.ConfirmRemoveAction'),
        true
      );
      if (!confirmed) { return; }
    }
    await this.myPlantsStore.toggle(p);
  }
}
