import { Component, inject } from '@angular/core';
import { NgClass } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faFilter } from '@fortawesome/free-solid-svg-icons';
import { RootDepth } from '../../../api/garden-assistant-api';
import { CompanionStore } from '../../../shared/services/companion.store';

@Component({
    selector: 'app-root-stratification',
    standalone: true,
    imports: [TranslateModule, NgClass, FontAwesomeModule],
    templateUrl: './root-stratification.html',
    styleUrl: './root-stratification.scss'
})
export class RootStratification {
    protected readonly store = inject(CompanionStore);
    protected readonly faFilter = faFilter;
    private readonly COMPETITION_THRESHOLD = 3;

    protected readonly layers = [
        { depth: RootDepth.Shallow, labelKey: 'Stratification.ShallowLabel', rangeKey: 'Stratification.ShallowRange', cssClass: 'soil-band-shallow' },
        { depth: RootDepth.Medium,  labelKey: 'Stratification.MediumLabel',  rangeKey: 'Stratification.MediumRange',  cssClass: 'soil-band-medium' },
        { depth: RootDepth.Deep,    labelKey: 'Stratification.DeepLabel',    rangeKey: 'Stratification.DeepRange',    cssClass: 'soil-band-deep' },
    ];

    hasCompetitionRisk(): boolean {
        for (const [, plants] of this.store.rootDepthGroups()) {
            if (plants.length > this.COMPETITION_THRESHOLD) {
                return true;
            }
        }
        return false;
    }
}
