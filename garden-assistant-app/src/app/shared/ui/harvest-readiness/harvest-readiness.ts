import { Component, input } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faEye, faHand, faClock, faScrewdriverWrench } from '@fortawesome/free-solid-svg-icons';
import { IconDefinition } from '@fortawesome/fontawesome-svg-core';
import { HarvestReadinessDto, HarvestCriterionType } from '../../../api/garden-assistant-api';

const CRITERION_ICONS: Record<HarvestCriterionType, IconDefinition> = {
  [HarvestCriterionType.Visual]: faEye,
  [HarvestCriterionType.Touch]: faHand,
  [HarvestCriterionType.Timing]: faClock,
  [HarvestCriterionType.Technique]: faScrewdriverWrench,
};

const CRITERION_LABEL_KEYS: Record<HarvestCriterionType, string> = {
  [HarvestCriterionType.Visual]: 'HarvestReadiness.CriterionVisual',
  [HarvestCriterionType.Touch]: 'HarvestReadiness.CriterionTouch',
  [HarvestCriterionType.Timing]: 'HarvestReadiness.CriterionTiming',
  [HarvestCriterionType.Technique]: 'HarvestReadiness.CriterionTechnique',
};

@Component({
  selector: 'app-harvest-readiness',
  standalone: true,
  imports: [TranslateModule, FontAwesomeModule],
  templateUrl: './harvest-readiness.html',
  styleUrl: './harvest-readiness.scss',
})
export class HarvestReadiness {
  readonly readiness = input<HarvestReadinessDto | null>(null);

  getCriterionIcon(type: HarvestCriterionType | undefined): IconDefinition {
    return CRITERION_ICONS[type ?? HarvestCriterionType.Visual] ?? faEye;
  }

  getCriterionLabelKey(type: HarvestCriterionType | undefined): string {
    return CRITERION_LABEL_KEYS[type ?? HarvestCriterionType.Visual] ?? '';
  }
}
