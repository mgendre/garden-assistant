import { Component, input, output, inject, ViewEncapsulation } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faHeart as faHeartSolid } from '@fortawesome/free-solid-svg-icons';
import { faHeart as faHeartRegular } from '@fortawesome/free-regular-svg-icons';
import { firstValueFrom } from 'rxjs';
import { PlantDto, SunRequirement, WaterNeeds, LifeCycle } from '../../../api/garden-assistant-api';
import { CompanionStore } from '../../../features/companions/companion.store';
import { MyPlantsStore } from '../../../features/my-plants/my-plants.store';
import { ConfirmDialogComponent, ConfirmDialogData } from '../../confirm-dialog/confirm-dialog';
import { BadgeInfoDialog, BadgeInfoDialogData } from '../badge-info-dialog/badge-info-dialog';
import { Collapsible } from '../collapsible/collapsible';

@Component({
  selector: 'app-plant-card',
  standalone: true,
  imports: [TranslateModule, FontAwesomeModule, Collapsible],
  templateUrl: './plant-card.html',
  styleUrl: './plant-card.scss',
  encapsulation: ViewEncapsulation.None,
  host: { class: 'plant-card' }
})
export class PlantCard {
  readonly plant = input.required<PlantDto>();
  readonly initialExpanded = input(false);
  readonly removable = input(false);

  readonly remove = output<void>();

  protected readonly store = inject(CompanionStore);
  protected readonly myPlantsStore = inject(MyPlantsStore);
  protected readonly faHeartSolid = faHeartSolid;
  protected readonly faHeartRegular = faHeartRegular;
  private readonly dialog = inject(MatDialog);
  private readonly translate = inject(TranslateService);

  getSunKey(sun: SunRequirement | undefined): string {
    switch (sun) {
      case SunRequirement.FullSun: return 'Plant.Sun.FullSun';
      case SunRequirement.PartialShade: return 'Plant.Sun.PartialShade';
      case SunRequirement.Shade: return 'Plant.Sun.Shade';
      default: return '';
    }
  }

  getWaterKey(water: WaterNeeds | undefined): string {
    switch (water) {
      case WaterNeeds.Low: return 'Plant.Water.Low';
      case WaterNeeds.Medium: return 'Plant.Water.Medium';
      case WaterNeeds.High: return 'Plant.Water.High';
      default: return '';
    }
  }

  getLifeCycleKey(cycle: LifeCycle | undefined): string {
    switch (cycle) {
      case LifeCycle.Annual: return 'Plant.LifeCycle.Annual';
      case LifeCycle.Biennial: return 'Plant.LifeCycle.Biennial';
      case LifeCycle.Perennial: return 'Plant.LifeCycle.Perennial';
      default: return '';
    }
  }

  getHeightLabel(plant: PlantDto): string {
    if (plant.heightAtMaturityCm == null) return '—';
    return `${plant.heightAtMaturityCm} cm`;
  }

  getBadgeKeys(plant: PlantDto): string[] {
    const keys: string[] = [];
    if (plant.nitrogenFixer) keys.push('Plant.Trait.NitrogenFixer');
    if (plant.pollinatorPlant) keys.push('Plant.Trait.Pollinator');
    if (plant.allelopathicRisk) keys.push('Plant.Trait.Allelopathic');
    return keys;
  }

  openBadgeInfo(titleKey: string, descriptionKey: string): void {
    this.dialog.open<BadgeInfoDialog, BadgeInfoDialogData>(BadgeInfoDialog, {
      data: { titleKey, descriptionKey },
      maxWidth: '400px',
    });
  }

  getSunBadgeKey(sun: SunRequirement | undefined): string {
    switch (sun) {
      case SunRequirement.FullSun: return 'FullSun';
      case SunRequirement.PartialShade: return 'PartialShade';
      case SunRequirement.Shade: return 'Shade';
      default: return '';
    }
  }

  getWaterBadgeKey(water: WaterNeeds | undefined): string {
    switch (water) {
      case WaterNeeds.Low: return 'Low';
      case WaterNeeds.Medium: return 'Medium';
      case WaterNeeds.High: return 'High';
      default: return '';
    }
  }

  getTraitBadgeKey(translationKey: string): string {
    return translationKey.replace('Plant.Trait.', '');
  }

  async toggleFav(event: Event): Promise<void> {
    event.stopPropagation();
    const p = this.plant();
    if (this.myPlantsStore.isSaved(p.id)) {
      const message = this.translate.instant('MyPlants.ConfirmRemoveMessage', { name: p.name });
      const confirmed = await firstValueFrom(
        this.dialog.open<ConfirmDialogComponent, ConfirmDialogData, boolean>(ConfirmDialogComponent, {
          data: {
            title: this.translate.instant('MyPlants.ConfirmRemoveTitle'),
            message,
            confirmLabel: this.translate.instant('MyPlants.ConfirmRemoveAction'),
          },
        }).afterClosed()
      );
      if (!confirmed) return;
    }
    this.myPlantsStore.toggle(p);
  }
}
