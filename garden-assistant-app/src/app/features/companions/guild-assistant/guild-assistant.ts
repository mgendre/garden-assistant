import { Component, inject, computed } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { CompanionStore, PRIORITY_MECHANISMS } from '../../../shared/services/companion.store';
import { AssociationMechanism, RootDepth } from '../../../api/garden-assistant-api';
import { Collapsible } from '../../../shared/ui/collapsible/collapsible';

interface MechanismRow {
  mechanism: AssociationMechanism;
  key: string;
  satisfied: boolean;
  providers: string[];
  highlighted: boolean;
}

interface RootDepthRow {
  depth: RootDepth;
  translationKey: string;
  satisfied: boolean;
  providers: string[];
  highlighted: boolean;
}

const ROOT_DEPTH_KEYS: Record<RootDepth, string> = {
  [RootDepth.Shallow]: 'GuildAssistant.RootShallow',
  [RootDepth.Medium]: 'GuildAssistant.RootMedium',
  [RootDepth.Deep]: 'GuildAssistant.RootDeep',
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

  readonly initialExpanded = computed(() =>
    window.innerWidth > 640 || this.store.selectedPlants().length < 3
  );

  readonly mechanismRows = computed<MechanismRow[]>(() => {
    const covered = this.store.allGuildMechanisms();
    const providers = this.store.mechanismProviders();
    let firstMissing = true;
    const rows = PRIORITY_MECHANISMS.map(m => {
      const satisfied = covered.has(m);
      let highlighted = false;
      if (!satisfied && firstMissing) {
        highlighted = true;
        firstMissing = false;
      }
      return {
        mechanism: m,
        key: this.store.getMechanismKey(m),
        satisfied,
        providers: providers.get(m) ?? [],
        highlighted,
      };
    });
    return rows.sort((a, b) => {
      if (a.highlighted !== b.highlighted) { return a.highlighted ? -1 : 1; }
      if (a.satisfied !== b.satisfied) { return a.satisfied ? 1 : -1; }
      return 0;
    });
  });

  readonly rootDepthRows = computed<RootDepthRow[]>(() => {
    const empty = new Set(this.store.emptyRootLayers());
    const providers = this.store.rootLayerProviders();
    const mechanismsMissing = this.store.missingPriorityMechanisms().length > 0;
    let firstMissing = true;
    return [RootDepth.Shallow, RootDepth.Medium, RootDepth.Deep].map(depth => {
      const satisfied = !empty.has(depth);
      let highlighted = false;
      if (!satisfied && !mechanismsMissing && firstMissing) {
        highlighted = true;
        firstMissing = false;
      }
      return {
        depth,
        translationKey: ROOT_DEPTH_KEYS[depth],
        satisfied,
        providers: providers.get(depth) ?? [],
        highlighted,
      };
    });
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
