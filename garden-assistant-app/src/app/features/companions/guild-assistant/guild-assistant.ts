import { Component, inject, computed } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { MatDialog } from '@angular/material/dialog';
import { CompanionStore, PRIORITY_MECHANISMS } from '../../../shared/services/companion.store';
import { AssociationMechanism, PlantDto, RootDepth } from '../../../api/garden-assistant-api';
import { BadgeInfoDialog, BadgeInfoDialogData } from '../../../shared/ui/badge-info-dialog/badge-info-dialog';
import { PlantDetailDialog, PlantDetailDialogData } from '../../../shared/ui/plant-detail-dialog/plant-detail-dialog';
import { PlantBadge } from '../../../shared/ui/plant-badge/plant-badge';
import { RootStratification } from '../root-stratification/root-stratification';

interface MechanismRow {
  mechanism: AssociationMechanism;
  key: string;
  satisfied: boolean;
  providers: PlantDto[];
}

@Component({
  selector: 'app-guild-assistant',
  standalone: true,
  imports: [TranslateModule, PlantBadge, RootStratification],
  templateUrl: './guild-assistant.html',
  styleUrl: './guild-assistant.scss'
})
export class GuildAssistant {
  protected readonly store = inject(CompanionStore);
  private readonly dialog = inject(MatDialog);

  readonly mechanismRows = computed<MechanismRow[]>(() => {
    const covered = this.store.allGuildMechanisms();
    const providers = this.store.mechanismProviders();
    return PRIORITY_MECHANISMS.map(m => ({
      mechanism: m,
      key: this.store.getMechanismKey(m),
      satisfied: covered.has(m),
      providers: providers.get(m) ?? [],
    }));
  });

  readonly isBalanced = computed(() => this.store.assistantGapCount() === 0);

  readonly filterRootDepth = (depth: RootDepth) => {
    this.store.mechanismFilter.set(null);
    this.store.toggleRootDepthFilter(depth);
  };

  filterMechanism(mechanism: AssociationMechanism): void {
    this.store.rootDepthFilter.set(null);
    this.store.toggleMechanismFilter(mechanism);
  }

  openPlantDetail(plant: PlantDto): void {
    this.dialog.open<PlantDetailDialog, PlantDetailDialogData>(PlantDetailDialog, {
      data: { plant },
      maxWidth: '600px',
      width: '90vw',
    });
  }

  openMechanismInfo(mechanism: AssociationMechanism): void {
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
