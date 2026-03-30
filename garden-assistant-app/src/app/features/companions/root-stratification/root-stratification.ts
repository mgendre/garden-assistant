import { Component, inject, input } from '@angular/core';
import { NgClass } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faFilter } from '@fortawesome/free-solid-svg-icons';
import { MatDialog } from '@angular/material/dialog';
import { PlantDto, RootDepth } from '../../../api/garden-assistant-api';
import { PlantDetailDialog, PlantDetailDialogData } from '../../../shared/ui/plant-detail-dialog/plant-detail-dialog';

@Component({
    selector: 'app-root-stratification',
    standalone: true,
    imports: [TranslateModule, NgClass, FontAwesomeModule],
    templateUrl: './root-stratification.html',
    styleUrl: './root-stratification.scss'
})
export class RootStratification {
    readonly rootDepthGroups = input.required<Map<RootDepth, PlantDto[]>>();
    readonly rootDepthFilter = input<RootDepth | null>(null);
    readonly interactive = input(false);
    readonly filterToggle = input<((depth: RootDepth) => void) | null>(null);

    protected readonly faFilter = faFilter;
    private readonly dialog = inject(MatDialog);
    private readonly COMPETITION_THRESHOLD = 3;

    protected readonly layers = [
        { depth: RootDepth.Shallow, labelKey: 'Stratification.ShallowLabel', rangeKey: 'Stratification.ShallowRange', cssClass: 'soil-band-shallow' },
        { depth: RootDepth.Medium,  labelKey: 'Stratification.MediumLabel',  rangeKey: 'Stratification.MediumRange',  cssClass: 'soil-band-medium' },
        { depth: RootDepth.Deep,    labelKey: 'Stratification.DeepLabel',    rangeKey: 'Stratification.DeepRange',    cssClass: 'soil-band-deep' },
    ];

    openPlantDetail(plant: PlantDto): void {
        this.dialog.open<PlantDetailDialog, PlantDetailDialogData>(PlantDetailDialog, {
            data: { plant },
            maxWidth: '600px',
            width: '90vw',
        });
    }

    onFilterClick(depth: RootDepth): void {
        const toggle = this.filterToggle();
        if (toggle) {
            toggle(depth);
        }
    }

    hasCompetitionRisk(): boolean {
        for (const [, plants] of this.rootDepthGroups()) {
            if (plants.length > this.COMPETITION_THRESHOLD) {
                return true;
            }
        }
        return false;
    }
}
