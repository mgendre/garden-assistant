import { Component, inject, signal } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { CompanionService } from '../companion.service';
import { PlantDto, SunRequirement, WaterNeeds, LifeCycle } from '../../../api/garden-assistant-api';

@Component({
  selector: 'app-plant-detail-panel',
  standalone: true,
  imports: [TranslateModule],
  templateUrl: './plant-detail-panel.html',
  styleUrl: './plant-detail-panel.scss'
})
export class PlantDetailPanel {
  protected readonly service = inject(CompanionService);
  readonly expandedPlants = signal<Set<string>>(new Set());

  toggleExpanded(plantId: string | undefined): void {
    if (!plantId) return;
    this.expandedPlants.update(set => {
      const next = new Set(set);
      if (next.has(plantId)) {
        next.delete(plantId);
      } else {
        next.add(plantId);
      }
      return next;
    });
  }

  isExpanded(plantId: string | undefined): boolean {
    if (!plantId) return false;
    return this.expandedPlants().has(plantId);
  }

  getSunKey(sun: SunRequirement | undefined): string {
    switch (sun) {
      case SunRequirement.FullSun: return 'Companions.SunFullSun';
      case SunRequirement.PartialShade: return 'Companions.SunPartialShade';
      case SunRequirement.Shade: return 'Companions.SunShade';
      default: return '';
    }
  }

  getWaterKey(water: WaterNeeds | undefined): string {
    switch (water) {
      case WaterNeeds.Low: return 'Companions.WaterLow';
      case WaterNeeds.Medium: return 'Companions.WaterMedium';
      case WaterNeeds.High: return 'Companions.WaterHigh';
      default: return '';
    }
  }

  getLifeCycleKey(cycle: LifeCycle | undefined): string {
    switch (cycle) {
      case LifeCycle.Annual: return 'Companions.LifeCycleAnnual';
      case LifeCycle.Biennial: return 'Companions.LifeCycleBiennial';
      case LifeCycle.Perennial: return 'Companions.LifeCyclePerennial';
      default: return '';
    }
  }

  getHeightLabel(plant: PlantDto): string {
    if (plant.heightAtMaturityCm == null) return '—';
    return `${plant.heightAtMaturityCm} cm`;
  }

  getBadgeKeys(plant: PlantDto): string[] {
    const keys: string[] = [];
    if (plant.nitrogenFixer) keys.push('Companions.TraitNitrogenFixer');
    if (plant.pollinatorPlant) keys.push('Companions.TraitPollinator');
    if (plant.allelopathicRisk) keys.push('Companions.TraitAllelopathic');
    return keys;
  }
}
