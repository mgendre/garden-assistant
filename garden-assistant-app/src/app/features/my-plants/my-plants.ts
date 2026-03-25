import { Component, inject } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { MyPlantsStore } from '../../shared/services/my-plants.store';
import { PlantCard } from '../../shared/ui/plant-card/plant-card';
import { PlantCatalogue } from '../companions/plant-catalogue/plant-catalogue';

@Component({
  selector: 'app-my-plants',
  standalone: true,
  imports: [TranslateModule, PlantCard, PlantCatalogue],
  templateUrl: './my-plants.html',
  styleUrl: './my-plants.scss'
})
export class MyPlants {
  protected readonly store = inject(MyPlantsStore);
}
