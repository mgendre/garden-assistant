import { Component, inject } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { CompanionStore } from '../../../shared/services/companion.store';
import { PlantCard } from '../../../shared/ui/plant-card/plant-card';

@Component({
  selector: 'app-plant-detail-panel',
  standalone: true,
  imports: [TranslateModule, PlantCard],
  templateUrl: './plant-detail-panel.html',
  styleUrl: './plant-detail-panel.scss'
})
export class PlantDetailPanel {
  protected readonly store = inject(CompanionStore);
}
