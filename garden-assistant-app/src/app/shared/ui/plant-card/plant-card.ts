import { Component, input, output, inject } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { PlantDto, SunRequirement, WaterNeeds, LifeCycle } from '../../../api/garden-assistant-api';
import { CompanionStore } from '../../../features/companions/companion.store';
import { Collapsible } from '../collapsible/collapsible';

@Component({
  selector: 'app-plant-card',
  standalone: true,
  imports: [TranslateModule, Collapsible],
  templateUrl: './plant-card.html',
  styleUrl: './plant-card.scss'
})
export class PlantCard {
  readonly plant = input.required<PlantDto>();
  readonly removable = input(false);

  readonly remove = output<void>();

  protected readonly store = inject(CompanionStore);

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
}
