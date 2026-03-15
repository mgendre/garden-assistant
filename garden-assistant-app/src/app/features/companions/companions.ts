import { Component, inject, OnInit } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { CompanionService } from './companion.service';
import { PlantCatalogue } from './plant-catalogue/plant-catalogue';
import { PlantDetailPanel } from './plant-detail-panel/plant-detail-panel';
import { RecommendationsPanel } from './recommendations-panel/recommendations-panel';

@Component({
  selector: 'app-companions',
  standalone: true,
  imports: [TranslateModule, PlantCatalogue, PlantDetailPanel, RecommendationsPanel],
  templateUrl: './companions.html',
  styleUrl: './companions.scss'
})
export class Companions implements OnInit {
  private readonly service = inject(CompanionService);

  async ngOnInit(): Promise<void> {
    await this.service.loadPlants();
  }
}
