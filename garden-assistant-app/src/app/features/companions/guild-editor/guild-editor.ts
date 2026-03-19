import { Component, inject } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faPen, faPlus, faXmark, faLink } from '@fortawesome/free-solid-svg-icons';
import { MatDialog } from '@angular/material/dialog';
import { CompanionStore } from '../../../shared/services/companion.store';
import { GuildStore } from '../../../shared/services/guild.store';
import { PlantStore } from '../../../shared/services/plant.store';
import { PlantDetailPanel } from '../plant-detail-panel/plant-detail-panel';
import { GuildPanel } from '../guild-panel/guild-panel';
import { Collapsible } from '../../../shared/ui/collapsible/collapsible';
import { BadgeInfoDialog, BadgeInfoDialogData } from '../../../shared/ui/badge-info-dialog/badge-info-dialog';

@Component({
  selector: 'app-guild-editor',
  standalone: true,
  imports: [TranslateModule, FontAwesomeModule, PlantDetailPanel, GuildPanel, Collapsible],
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
  protected readonly faLink = faLink;
  private readonly dialog = inject(MatDialog);

  plantName(id: string | undefined): string {
    return this.plantStore.findById(id)?.name ?? '';
  }

  openMechanismInfo(mechanism: number): void {
    const key = this.store.getMechanismKey(mechanism);
    if (key) {
      this.dialog.open<BadgeInfoDialog, BadgeInfoDialogData>(BadgeInfoDialog, {
        data: {
          titleKey: `BadgeInfo.Mechanism.${key}.Title`,
          descriptionKey: `BadgeInfo.Mechanism.${key}.Description`,
        },
        maxWidth: '400px',
      });
    }
  }
}
