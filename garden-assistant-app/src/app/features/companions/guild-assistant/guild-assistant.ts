import { Component, inject, computed } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { CompanionStore, PRIORITY_MECHANISMS } from '../../../shared/services/companion.store';
import { AssociationMechanism, PlantDto, RootDepth } from '../../../api/garden-assistant-api';
import { DialogService } from '../../../shared/services/dialog.service';
import { PlantDialogService } from '../../../shared/services/plant-dialog.service';
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
  private readonly dialogService = inject(DialogService);
  private readonly plantDialogService = inject(PlantDialogService);

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
    this.plantDialogService.openDetail(plant);
  }

  openMechanismInfo(mechanism: AssociationMechanism): void {
    const key = this.store.getMechanismKey(mechanism);
    if (key) {
      this.dialogService.openBadgeInfo(
        `BadgeInfo.Mechanism.${key}.Title`,
        `BadgeInfo.Mechanism.${key}.Description`
      );
    }
  }
}
