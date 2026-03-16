import { Component, inject } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { CompanionStore } from './companion.store';
import { PlantCatalogue } from './plant-catalogue/plant-catalogue';
import { PlantDetailPanel } from './plant-detail-panel/plant-detail-panel';
import { RecommendationsPanel } from './recommendations-panel/recommendations-panel';
import { GuildPanel } from './guild-panel/guild-panel';

@Component({
  selector: 'app-companions',
  standalone: true,
  imports: [TranslateModule, PlantCatalogue, PlantDetailPanel, RecommendationsPanel, GuildPanel],
  templateUrl: './companions.html',
  styleUrl: './companions.scss'
})
export class Companions {
  protected readonly store = inject(CompanionStore);
}
