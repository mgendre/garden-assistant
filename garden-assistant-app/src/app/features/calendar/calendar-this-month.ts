import { Component, inject, computed } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { CalendarStore } from '../../shared/services/calendar.store';
import { PlantStore } from '../../shared/services/plant.store';
import { PlantActionType, CalendarPlantDto } from '../../api/garden-assistant-api';
import { PlantDialogService } from '../../shared/services/plant-dialog.service';
import { ACTION_TYPE_CONFIGS } from '../../shared/constants/plant-action.constants';

interface ActionGroup {
  type: PlantActionType;
  color: string;
  labelKey: string;
  plants: { id: string; name: string }[];
}

@Component({
  selector: 'app-calendar-this-month',
  standalone: true,
  imports: [TranslateModule],
  templateUrl: './calendar-this-month.html',
  styleUrl: './calendar-this-month.scss',
  host: { style: 'display:block' }
})
export class CalendarThisMonth {
  private readonly store = inject(CalendarStore);
  private readonly plantStore = inject(PlantStore);
  private readonly plantDialogService = inject(PlantDialogService);

  protected readonly currentHalfMonth = computed(() => {
    const now = new Date();
    const month = now.getMonth();
    const day = now.getDate();
    return month * 2 + (day <= 15 ? 1 : 2);
  });

  protected readonly nextHalfMonth = computed(() => {
    return this.currentHalfMonth() % 24 + 1;
  });

  protected readonly currentPeriodKey = computed(() => {
    return 'Calendar.HalfMonth.' + this.currentHalfMonth();
  });

  protected readonly nextPeriodKey = computed(() => {
    return 'Calendar.HalfMonth.' + this.nextHalfMonth();
  });

  protected readonly currentActionGroups = computed<ActionGroup[]>(() => {
    return this.buildActionGroups(this.currentHalfMonth());
  });

  protected readonly upcomingActionGroups = computed<ActionGroup[]>(() => {
    return this.buildActionGroups(this.nextHalfMonth());
  });

  openPlantDetail(plantId: string): void {
    this.plantDialogService.openDetail(plantId);
  }

  private buildActionGroups(halfMonth: number): ActionGroup[] {
    const plants = this.store.filteredPlants();
    if (plants.length === 0) {
      return [];
    }

    const groups: ActionGroup[] = [];

    for (const config of ACTION_TYPE_CONFIGS) {
      const plantsForAction: { id: string; name: string }[] = [];

      for (const plant of plants) {
        if (!plant.actions || !plant.plantId) {
          continue;
        }
        const hasAction = plant.actions.some(action =>
          action.actionType === config.type &&
          this.isActiveInHalfMonth(action.halfMonthStart, action.halfMonthEnd, halfMonth)
        );
        if (hasAction) {
          const plantInfo = this.plantStore.findById(plant.plantId);
          if (plantInfo) {
            plantsForAction.push({ id: plant.plantId, name: plantInfo.name! });
          }
        }
      }

      if (plantsForAction.length > 0) {
        groups.push({
          type: config.type,
          color: config.color,
          labelKey: config.labelKey,
          plants: plantsForAction,
        });
      }
    }

    return groups;
  }

  private isActiveInHalfMonth(start: number | undefined, end: number | undefined, halfMonth: number): boolean {
    if (start === undefined || end === undefined) {
      return false;
    }
    if (start <= end) {
      return halfMonth >= start && halfMonth <= end;
    }
    return halfMonth >= start || halfMonth <= end;
  }
}
