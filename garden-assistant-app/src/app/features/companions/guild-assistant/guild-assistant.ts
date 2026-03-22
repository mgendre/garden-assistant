import { Component, inject, computed } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { MatDialog } from '@angular/material/dialog';
import { CompanionStore, PRIORITY_MECHANISMS } from '../../../shared/services/companion.store';
import { AssociationMechanism, RootDepth } from '../../../api/garden-assistant-api';
import { Collapsible } from '../../../shared/ui/collapsible/collapsible';
import { BadgeInfoDialog, BadgeInfoDialogData } from '../../../shared/ui/badge-info-dialog/badge-info-dialog';

interface MechanismRow {
  mechanism: AssociationMechanism;
  key: string;
  satisfied: boolean;
  providers: string[];
}

interface RootDepthRow {
  depth: RootDepth;
  translationKey: string;
  badgeInfoKey: string;
  satisfied: boolean;
  providers: string[];
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
  imports: [TranslateModule, Collapsible],
  templateUrl: './guild-assistant.html',
  styleUrl: './guild-assistant.scss'
})
export class GuildAssistant {
  protected readonly store = inject(CompanionStore);
  private readonly dialog = inject(MatDialog);

  readonly initialExpanded = computed(() =>
    window.innerWidth > 640 || this.store.selectedPlants().length < 3
  );

  readonly mechanismRows = computed<MechanismRow[]>(() => {
    const covered = this.store.allGuildMechanisms();
    const providers = this.store.mechanismProviders();
    const rows = PRIORITY_MECHANISMS.map(m => ({
      mechanism: m,
      key: this.store.getMechanismKey(m),
      satisfied: covered.has(m),
      providers: providers.get(m) ?? [],
    }));
    return rows.sort((a, b) => {
      if (a.satisfied !== b.satisfied) { return a.satisfied ? 1 : -1; }
      return 0;
    });
  });

  readonly rootDepthRows = computed<RootDepthRow[]>(() => {
    const empty = new Set(this.store.emptyRootLayers());
    const providers = this.store.rootLayerProviders();
    return [RootDepth.Shallow, RootDepth.Medium, RootDepth.Deep].map(depth => ({
      depth,
      translationKey: ROOT_DEPTH_KEYS[depth],
      badgeInfoKey: ROOT_DEPTH_BADGE_INFO_KEYS[depth],
      satisfied: !empty.has(depth),
      providers: providers.get(depth) ?? [],
    }));
  });

  readonly isBalanced = computed(() => this.store.assistantGapCount() === 0);

  filterMechanism(mechanism: AssociationMechanism): void {
    this.store.rootDepthFilter.set(null);
    this.store.mechanismFilter.set(mechanism);
    this.scrollToCatalog();
  }

  filterRootDepth(depth: RootDepth): void {
    this.store.mechanismFilter.set(null);
    this.store.rootDepthFilter.set(depth);
    this.scrollToCatalog();
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

  scrollToAssociations(): void {
    const el = document.querySelector('.guild-associations');
    if (el) {
      el.scrollIntoView({ behavior: 'smooth' });
    }
  }

  private scrollToCatalog(): void {
    const el = document.querySelector('app-plant-catalogue');
    if (el) {
      el.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  }
}
