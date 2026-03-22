import { Component, input, output, inject, signal, ViewEncapsulation, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faHeart as faHeartSolid, faLink } from '@fortawesome/free-solid-svg-icons';
import { faHeart as faHeartRegular } from '@fortawesome/free-regular-svg-icons';
import { firstValueFrom } from 'rxjs';
import { PlantDto, PlantActionDto, HarvestReadinessDto, SunRequirement, WaterNeeds, LifeCycle, RootDepth, PropagationMethod, AssociationMechanism } from '../../../api/garden-assistant-api';
import { CompanionStore } from '../../services/companion.store';
import { MyPlantsStore } from '../../services/my-plants.store';
import { CalendarService } from '../../services/calendar.service';
import { ConfirmDialogComponent, ConfirmDialogData } from '../../confirm-dialog/confirm-dialog';
import { BadgeInfoDialog, BadgeInfoDialogData } from '../badge-info-dialog/badge-info-dialog';
import { PlantCalendarGantt } from '../plant-calendar-gantt/plant-calendar-gantt';
import { HarvestReadinessDialog, HarvestReadinessDialogData } from '../harvest-readiness/harvest-readiness-dialog';
import { Collapsible } from '../collapsible/collapsible';

@Component({
  selector: 'app-plant-card',
  standalone: true,
  imports: [TranslateModule, FontAwesomeModule, Collapsible, PlantCalendarGantt],
  templateUrl: './plant-card.html',
  styleUrl: './plant-card.scss',
  encapsulation: ViewEncapsulation.None,
  host: { class: 'plant-card' }
})
export class PlantCard implements OnInit {
  readonly plant = input.required<PlantDto>();
  readonly initialExpanded = input(false);
  readonly forceExpanded = input(false);
  readonly removable = input(false);
  readonly hideMechanisms = input(false);
  readonly hideFavButton = input(false);
  readonly relationalMechanisms = input<number[]>([]);

  readonly remove = output<void>();

  protected readonly store = inject(CompanionStore);
  protected readonly myPlantsStore = inject(MyPlantsStore);
  private readonly calendarService = inject(CalendarService);

  readonly plantActions = signal<PlantActionDto[]>([]);
  readonly harvestReadiness = signal<HarvestReadinessDto | null>(null);
  protected readonly faHeartSolid = faHeartSolid;
  protected readonly faHeartRegular = faHeartRegular;
  protected readonly faLink = faLink;
  private readonly dialog = inject(MatDialog);
  private readonly translate = inject(TranslateService);

  async ngOnInit(): Promise<void> {
    const plantId = this.plant().id;
    if (!plantId) {
      return;
    }
    try {
      const [actions, readiness] = await Promise.all([
        this.calendarService.getPlantActions(plantId),
        this.calendarService.getHarvestReadiness(plantId),
      ]);
      this.plantActions.set(actions ?? []);
      this.harvestReadiness.set(readiness);
    } catch {
    }
  }

  get propagationMethod(): PropagationMethod {
    return this.plant().propagationMethod ?? PropagationMethod.Seed;
  }

  get frostSensitive(): boolean {
    return this.plant().frostSensitive ?? false;
  }

  openHarvestReadiness(): void {
    const readiness = this.harvestReadiness();
    if (readiness) {
      this.dialog.open<HarvestReadinessDialog, HarvestReadinessDialogData>(HarvestReadinessDialog, {
        data: { readiness, plantName: this.plant().name! },
        maxWidth: '600px',
        width: '90vw',
      });
    }
  }

  getSunKey(sun: SunRequirement | undefined): string {
    switch (sun) {
      case SunRequirement.FullSun: return 'Plant.Sun.FullSun';
      case SunRequirement.PartialShade: return 'Plant.Sun.PartialShade';
      case SunRequirement.Shade: return 'Plant.Sun.Shade';
      default: return '';
    }
  }

  getWaterKey(water: WaterNeeds | undefined): string {
    switch (water) {
      case WaterNeeds.Low: return 'Plant.Water.Low';
      case WaterNeeds.Medium: return 'Plant.Water.Medium';
      case WaterNeeds.High: return 'Plant.Water.High';
      default: return '';
    }
  }

  getLifeCycleKey(cycle: LifeCycle | undefined): string {
    switch (cycle) {
      case LifeCycle.Annual: return 'Plant.LifeCycle.Annual';
      case LifeCycle.Biennial: return 'Plant.LifeCycle.Biennial';
      case LifeCycle.Perennial: return 'Plant.LifeCycle.Perennial';
      default: return '';
    }
  }

  getRootDepthKey(rootDepth: RootDepth | undefined): string {
    switch (rootDepth) {
      case RootDepth.Shallow: return 'Plant.RootDepth.Shallow';
      case RootDepth.Medium: return 'Plant.RootDepth.Medium';
      case RootDepth.Deep: return 'Plant.RootDepth.Deep';
      default: return '';
    }
  }

  getRootDepthBadgeKey(rootDepth: RootDepth | undefined): string {
    switch (rootDepth) {
      case RootDepth.Shallow: return 'Shallow';
      case RootDepth.Medium: return 'Medium';
      case RootDepth.Deep: return 'Deep';
      default: return '';
    }
  }

  getSunHours(sun: SunRequirement | undefined): string {
    switch (sun) {
      case SunRequirement.FullSun: return '6h+';
      case SunRequirement.PartialShade: return '3–6h';
      case SunRequirement.Shade: return '< 3h';
      default: return '';
    }
  }

  getRootDepthCm(rootDepth: RootDepth | undefined): string {
    switch (rootDepth) {
      case RootDepth.Shallow: return '15–30 cm';
      case RootDepth.Medium: return '30–60 cm';
      case RootDepth.Deep: return '60 cm+';
      default: return '';
    }
  }

  getHeightLabel(plant: PlantDto): string {
    if (plant.heightAtMaturityCm == null) return '—';
    return `${plant.heightAtMaturityCm} cm`;
  }

  getIntrinsicMechanisms(plant: PlantDto): AssociationMechanism[] {
    return plant.intrinsicMechanisms ?? [];
  }

  hasAllelopathicRisk(plant: PlantDto): boolean {
    return (plant.intrinsicMechanisms ?? []).includes(AssociationMechanism.RootAllelopathy);
  }

  openBadgeInfo(titleKey: string, descriptionKey: string): void {
    this.dialog.open<BadgeInfoDialog, BadgeInfoDialogData>(BadgeInfoDialog, {
      data: { titleKey, descriptionKey },
      maxWidth: '400px',
    });
  }

  getSunBadgeKey(sun: SunRequirement | undefined): string {
    switch (sun) {
      case SunRequirement.FullSun: return 'FullSun';
      case SunRequirement.PartialShade: return 'PartialShade';
      case SunRequirement.Shade: return 'Shade';
      default: return '';
    }
  }

  getWaterBadgeKey(water: WaterNeeds | undefined): string {
    switch (water) {
      case WaterNeeds.Low: return 'Low';
      case WaterNeeds.Medium: return 'Medium';
      case WaterNeeds.High: return 'High';
      default: return '';
    }
  }

  async toggleFav(event: Event): Promise<void> {
    event.stopPropagation();
    const p = this.plant();
    if (this.myPlantsStore.isSaved(p.id)) {
      const message = this.translate.instant('MyPlants.ConfirmRemoveMessage', { name: p.name });
      const confirmed = await firstValueFrom(
        this.dialog.open<ConfirmDialogComponent, ConfirmDialogData, boolean>(ConfirmDialogComponent, {
          data: {
            title: this.translate.instant('MyPlants.ConfirmRemoveTitle'),
            message,
            confirmLabel: this.translate.instant('MyPlants.ConfirmRemoveAction'),
          },
        }).afterClosed()
      );
      if (!confirmed) return;
    }
    this.myPlantsStore.toggle(p);
  }
}
