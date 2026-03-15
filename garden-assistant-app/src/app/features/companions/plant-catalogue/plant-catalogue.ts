import { Component, inject } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faMagnifyingGlass } from '@fortawesome/free-solid-svg-icons';
import { CompanionStore } from '../companion.store';
import { MyPlantsStore } from '../../my-plants/my-plants.store';

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
}
