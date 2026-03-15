import { Component, inject } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { CompanionStore } from '../companion.store';

@Component({
  selector: 'app-plant-catalogue',
  standalone: true,
  imports: [TranslateModule],
  templateUrl: './plant-catalogue.html',
  styleUrl: './plant-catalogue.scss'
})
export class PlantCatalogue {
  protected readonly store = inject(CompanionStore);
}
