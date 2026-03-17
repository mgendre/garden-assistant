import { Component, inject } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { CompanionStore } from '../../../shared/services/companion.store';
import { PlantDetailPanel } from '../plant-detail-panel/plant-detail-panel';
import { GuildPanel } from '../guild-panel/guild-panel';

@Component({
  selector: 'app-guild-editor',
  standalone: true,
  imports: [TranslateModule, PlantDetailPanel, GuildPanel],
  templateUrl: './guild-editor.html',
  styleUrl: './guild-editor.scss'
})
export class GuildEditor {
  protected readonly store = inject(CompanionStore);
}
