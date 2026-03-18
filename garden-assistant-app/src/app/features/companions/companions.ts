import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { PlantCatalogue } from './plant-catalogue/plant-catalogue';
import { GuildEditor } from './guild-editor/guild-editor';
import { CompanionStore } from '../../shared/services/companion.store';
import { GuildService } from '../../shared/services/guild.service';

@Component({
  selector: 'app-companions',
  standalone: true,
  imports: [TranslateModule, PlantCatalogue, GuildEditor],
  templateUrl: './companions.html',
  styleUrl: './companions.scss'
})
export class Companions implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly companionStore = inject(CompanionStore);
  private readonly guildService = inject(GuildService);

  async ngOnInit(): Promise<void> {
    const params = this.route.snapshot.queryParams;
    const guildId = params['guild'];
    if (!guildId) {
      return;
    }
    const detail = await this.guildService.getById(guildId);
    this.companionStore.loadGuildForEditing(detail);
    if (params['mode'] === 'edit') {
      this.companionStore.startGuildEditing();
    }
  }
}
