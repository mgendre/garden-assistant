import { Component, inject } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { CompanionStore } from '../companion.store';
import { CompanionRecommendationDto } from '../../../api/garden-assistant-api';

@Component({
  selector: 'app-recommendations-panel',
  standalone: true,
  imports: [TranslateModule],
  templateUrl: './recommendations-panel.html',
  styleUrl: './recommendations-panel.scss'
})
export class RecommendationsPanel {
  protected readonly store = inject(CompanionStore);

  onCompanionClick(companion: CompanionRecommendationDto): void {
    const plant = this.store.plants().find(p => p.id === companion.plantId);
    if (plant) this.store.addPlant(plant);
  }

  getMechanismTranslationKey(mechanism: number): string {
    const key = this.store.getMechanismKey(mechanism);
    return key ? `Plant.Mechanism.${key}` : '';
  }
}
