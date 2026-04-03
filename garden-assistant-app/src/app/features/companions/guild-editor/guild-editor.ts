import { Component, inject, signal, effect } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faPen, faPlus, faXmark, faTrash } from '@fortawesome/free-solid-svg-icons';
import { CompanionStore } from '../../../shared/services/companion.store';
import { GuildStore } from '../../../shared/services/guild.store';
import { DialogService } from '../../../shared/services/dialog.service';
import { TranslateService } from '@ngx-translate/core';
import { PlantStore } from '../../../shared/services/plant.store';
import { PlantDialogService } from '../../../shared/services/plant-dialog.service';
import { PlantActionType, PropagationMethod } from '../../../api/garden-assistant-api';
import { SOWING_ACTIONS, getEarliestHalfMonth } from '../../../shared/constants/plant-action.constants';
import { PlantDetailPanel } from '../plant-detail-panel/plant-detail-panel';
import { GuildPanel } from '../guild-panel/guild-panel';
import { Collapsible } from '../../../shared/ui/collapsible/collapsible';
import { PlantAssociationPanel, PlantCalendarEntry } from '../../../shared/ui/plant-association-panel/plant-association-panel';
import { InfoBanner } from '../../../shared/ui/info-banner/info-banner';

@Component({
  selector: 'app-guild-editor',
  standalone: true,
  imports: [TranslateModule, FontAwesomeModule, PlantDetailPanel, GuildPanel, Collapsible, PlantAssociationPanel, InfoBanner],
  templateUrl: './guild-editor.html',
  styleUrl: './guild-editor.scss'
})
export class GuildEditor {
  protected readonly store = inject(CompanionStore);
  protected readonly guildStore = inject(GuildStore);
  protected readonly plantStore = inject(PlantStore);
  protected readonly faPen = faPen;
  protected readonly faPlus = faPlus;
  protected readonly faClose = faXmark;
  protected readonly faTrash = faTrash;
  private readonly plantDialogService = inject(PlantDialogService);
  private readonly dialogService = inject(DialogService);
  private readonly translate = inject(TranslateService);

  readonly plantCalendars = signal<PlantCalendarEntry[]>([]);

  constructor() {
    effect(() => {
      const plants = this.store.selectedPlants();
      if (plants.length < 2) {
        this.plantCalendars.set([]);
        return;
      }
      const entries: PlantCalendarEntry[] = [];
      const plantIds = plants.map(p => p.id!).filter(Boolean);
      for (const id of plantIds) {
        const plant = this.plantStore.findById(id);
        if (plant) {
          entries.push({
            plantId: id,
            name: plant.name!,
            propagationMethod: plant.propagationMethod ?? PropagationMethod.Seed,
            frostSensitive: plant.frostSensitive ?? false,
            actions: plant.actions ?? [],
          });
        }
      }
      entries.sort((a, b) => {
        const sowA = getEarliestHalfMonth(a.actions, SOWING_ACTIONS);
        const sowB = getEarliestHalfMonth(b.actions, SOWING_ACTIONS);
        if (sowA !== sowB) { return sowA - sowB; }
        const transA = getEarliestHalfMonth(a.actions, [PlantActionType.Transplanting]);
        const transB = getEarliestHalfMonth(b.actions, [PlantActionType.Transplanting]);
        if (transA !== transB) { return transA - transB; }
        const harvA = getEarliestHalfMonth(a.actions, [PlantActionType.Harvest]);
        const harvB = getEarliestHalfMonth(b.actions, [PlantActionType.Harvest]);
        if (harvA !== harvB) { return harvA - harvB; }
        return a.name.localeCompare(b.name, 'fr');
      });
      this.plantCalendars.set(entries);
    });
  }

  async openHarvestReadiness(plantId: string, plantName: string): Promise<void> {
    await this.plantDialogService.openHarvestReadiness(plantId, plantName);
  }

  openPlantDetail(plantId: string): void {
    this.plantDialogService.openDetail(plantId);
  }

  async deleteCurrentGuild(): Promise<void> {
    const guild = this.store.editingGuild();
    if (!guild?.id) { return; }
    const confirmed = await this.dialogService.confirm(
      this.translate.instant('Guilds.ConfirmDeleteTitle'),
      this.translate.instant('Guilds.ConfirmDeleteMessage', { name: guild.name }),
      this.translate.instant('Guilds.Delete'),
      true
    );
    if (confirmed) {
      await this.guildStore.deleteGuild(guild.id);
      this.store.clearSelection();
    }
  }
}
