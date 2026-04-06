import { Component, inject } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faLink } from '@fortawesome/free-solid-svg-icons';
import { CompanionStore } from '../../../shared/services/companion.store';
import { DialogService } from '../../../shared/services/dialog.service';
import { AssociationMechanism } from '../../../api/garden-assistant-api';
import { PlantCard } from '../../../shared/ui/plant-card/plant-card';
import { EmptyState } from '../../../shared/ui/empty-state/empty-state';

@Component({
  selector: 'app-plant-detail-panel',
  standalone: true,
  imports: [TranslateModule, FontAwesomeModule, PlantCard, EmptyState],
  templateUrl: './plant-detail-panel.html',
  styleUrl: './plant-detail-panel.scss'
})
export class PlantDetailPanel {
  protected readonly store = inject(CompanionStore);
  protected readonly faLink = faLink;
  private readonly dialogService = inject(DialogService);

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
