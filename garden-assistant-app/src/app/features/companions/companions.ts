import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
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
export class Companions implements OnInit, OnDestroy {
  private readonly route = inject(ActivatedRoute);
  protected readonly companionStore = inject(CompanionStore);
  private readonly guildService = inject(GuildService);
  private paramsSub?: Subscription;

  ngOnInit(): void {
    this.paramsSub = this.route.queryParams.subscribe(params => {
      this.handleParams(params);
    });
  }

  ngOnDestroy(): void {
    this.paramsSub?.unsubscribe();
  }

  private async handleParams(params: Record<string, string>): Promise<void> {
    const returnTo = params['returnTo'];
    const guildId = params['guild'];
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

    if (returnTo && !bedName) {
      this.companionStore.setReturnTo(returnTo);
    }

    const detail = await this.guildService.getById(guildId);
    this.companionStore.loadGuildForEditing(detail);
    if (params['mode'] === 'edit') {
      this.companionStore.startGuildEditing();
    }
  }
}
