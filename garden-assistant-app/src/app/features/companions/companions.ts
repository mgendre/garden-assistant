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
  protected readonly companionStore = inject(CompanionStore);
  private readonly guildService = inject(GuildService);

  async ngOnInit(): Promise<void> {
    const params = this.route.snapshot.queryParams;
    const returnTo = params['returnTo'];
    const guildId = params['guild'];

    if (returnTo) {
      this.companionStore.setReturnTo(returnTo);
    }

    const bedName = params['bedName'];
    if (bedName) {
      this.companionStore.clearSelection();
      this.companionStore.setEditingBedName(bedName);
      if (returnTo) {
        this.companionStore.setReturnTo(returnTo);
      }
    } else if (this.companionStore.editingBedName()) {
      this.companionStore.setEditingBedName(null);
      this.companionStore.setReturnTo(null);
    }

    if (!guildId) {
      if (params['mode'] === 'create') {
        this.companionStore.startGuildCreation();
      }
      return;
    }
    const detail = await this.guildService.getById(guildId);
    this.companionStore.loadGuildForEditing(detail);
    if (params['mode'] === 'edit') {
      this.companionStore.startGuildEditing();
    }
  }
}
