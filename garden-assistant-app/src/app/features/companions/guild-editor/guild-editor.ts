import { Component, inject } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faPen, faPlus, faXmark } from '@fortawesome/free-solid-svg-icons';
import { CompanionStore } from '../../../shared/services/companion.store';
import { PlantDetailPanel } from '../plant-detail-panel/plant-detail-panel';
import { GuildPanel } from '../guild-panel/guild-panel';

@Component({
  selector: 'app-guild-editor',
  standalone: true,
  imports: [TranslateModule, FontAwesomeModule, PlantDetailPanel, GuildPanel],
  templateUrl: './guild-editor.html',
  styleUrl: './guild-editor.scss'
})
export class GuildEditor {
  protected readonly store = inject(CompanionStore);
  protected readonly faPen = faPen;
  protected readonly faPlus = faPlus;
  protected readonly faClose = faXmark;
}
