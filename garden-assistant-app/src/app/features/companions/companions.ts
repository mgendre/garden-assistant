import { Component } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { PlantCatalogue } from './plant-catalogue/plant-catalogue';
import { GuildEditor } from './guild-editor/guild-editor';
import { RecommendationsPanel } from './recommendations-panel/recommendations-panel';

@Component({
  selector: 'app-companions',
  standalone: true,
  imports: [TranslateModule, PlantCatalogue, GuildEditor, RecommendationsPanel],
  templateUrl: './companions.html',
  styleUrl: './companions.scss'
})
export class Companions {}
