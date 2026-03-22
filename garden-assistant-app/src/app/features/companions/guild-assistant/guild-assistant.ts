import { Component, inject, computed } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { MatDialog } from '@angular/material/dialog';
import { CompanionStore, PRIORITY_MECHANISMS } from '../../../shared/services/companion.store';
import { AssociationMechanism, PlantDto, RootDepth } from '../../../api/garden-assistant-api';
import { BadgeInfoDialog, BadgeInfoDialogData } from '../../../shared/ui/badge-info-dialog/badge-info-dialog';
import { PlantDetailDialog, PlantDetailDialogData } from '../../../shared/ui/plant-detail-dialog/plant-detail-dialog';

interface MechanismRow {
  mechanism: AssociationMechanism;
  key: string;
  satisfied: boolean;
  providers: PlantDto[];
}

interface RootDepthRow {
  depth: RootDepth;
  translationKey: string;
  badgeInfoKey: string;
  satisfied: boolean;
  providers: PlantDto[];
}

const ROOT_DEPTH_KEYS: Record<RootDepth, string> = {
  [RootDepth.Shallow]: 'GuildAssistant.RootShallow',
  [RootDepth.Medium]: 'GuildAssistant.RootMedium',
  [RootDepth.Deep]: 'GuildAssistant.RootDeep',
};

const ROOT_DEPTH_BADGE_INFO_KEYS: Record<RootDepth, string> = {
  [RootDepth.Shallow]: 'Shallow',
  [RootDepth.Medium]: 'Medium',
  [RootDepth.Deep]: 'Deep',
};

@Component({
  selector: 'app-guild-assistant',
  standalone: true,
  imports: [TranslateModule],
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

  readonly rootDepthRows = computed<RootDepthRow[]>(() => {
    const empty = new Set(this.store.emptyRootLayers());
    const groups = this.store.rootDepthGroups();
    return [RootDepth.Shallow, RootDepth.Medium, RootDepth.Deep].map(depth => ({
      depth,
      translationKey: ROOT_DEPTH_KEYS[depth],
      badgeInfoKey: ROOT_DEPTH_BADGE_INFO_KEYS[depth],
      satisfied: !empty.has(depth),
      providers: groups.get(depth) ?? [],
    }));
  });

  readonly isBalanced = computed(() => this.store.assistantGapCount() === 0);

  filterMechanism(mechanism: AssociationMechanism): void {
    this.store.rootDepthFilter.set(null);
    this.store.mechanismFilter.set(mechanism);
  }

  filterRootDepth(depth: RootDepth): void {
    this.store.mechanismFilter.set(null);
    this.store.rootDepthFilter.set(depth);
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

  openRootDepthInfo(badgeInfoKey: string): void {
    this.dialog.open<BadgeInfoDialog, BadgeInfoDialogData>(BadgeInfoDialog, {
      data: {
        titleKey: `BadgeInfo.RootDepth.${badgeInfoKey}.Title`,
        descriptionKey: `BadgeInfo.RootDepth.${badgeInfoKey}.Description`,
      },
      maxWidth: '400px',
    });
  }
}
