import { Component, input, computed } from '@angular/core';
import { PlantActionDto, PlantActionType } from '../../../api/garden-assistant-api';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faSnowflake } from '@fortawesome/free-solid-svg-icons';
import { ACTION_COLORS, FROST_SENSITIVE_ACTIONS, FROST_HALF_MONTHS_START, FROST_HALF_MONTHS_END } from '../../constants/plant-action.constants';

interface HalfMonthCell {
  halfMonth: number;
  isFirstHalf: boolean;
  isCurrent: boolean;
}

interface MonthGroup {
  cells: HalfMonthCell[];
}

interface ActionBar {
  color: string;
  actionType: PlantActionType;
}

@Component({
  selector: 'app-plant-calendar-bar',
  standalone: true,
  imports: [FontAwesomeModule],
  templateUrl: './plant-calendar-bar.html',
  styleUrl: './plant-calendar-bar.scss',
  host: { style: 'display:flex;flex:1;min-width:0' },
})
export class PlantCalendarBar {
  readonly actions = input<PlantActionDto[]>([]);
  readonly frostSensitive = input(false);
  readonly activeFilters = input<PlantActionType[]>([]);

  protected readonly faSnowflake = faSnowflake;

  readonly months = computed<MonthGroup[]>(() => {
    const currentHm = this.getCurrentHalfMonth();
    const result: MonthGroup[] = [];
    for (let m = 0; m < 12; m++) {
      const hm1 = m * 2 + 1;
      const hm2 = m * 2 + 2;
      result.push({
        cells: [
          { halfMonth: hm1, isFirstHalf: true, isCurrent: hm1 === currentHm },
          { halfMonth: hm2, isFirstHalf: false, isCurrent: hm2 === currentHm },
        ],
      });
    }
    return result;
  });

  getActionBarsForCell(halfMonth: number): ActionBar[] {
    const filters = this.activeFilters();
    return (this.actions() ?? [])
      .filter(action =>
        action.actionType !== undefined &&
        filters.includes(action.actionType) &&
        this.isActiveInHalfMonth(action, halfMonth)
      )
      .map(action => ({
        color: ACTION_COLORS[action.actionType!] ?? '#9ca3af',
        actionType: action.actionType!,
      }));
  }

  showFrostIndicator(halfMonth: number): boolean {
    if (!this.frostSensitive()) {
      return false;
    }
    if (halfMonth < FROST_HALF_MONTHS_START || halfMonth > FROST_HALF_MONTHS_END) {
      return false;
    }
    const filters = this.activeFilters();
    return (this.actions() ?? []).some(
      action =>
        action.actionType !== undefined &&
        FROST_SENSITIVE_ACTIONS.includes(action.actionType) &&
        filters.includes(action.actionType) &&
        this.isActiveInHalfMonth(action, halfMonth)
    );
  }

  private isActiveInHalfMonth(action: PlantActionDto, halfMonth: number): boolean {
    const start = action.halfMonthStart;
    const end = action.halfMonthEnd;
    if (start === undefined || end === undefined) {
      return false;
    }
    if (start <= end) {
      return halfMonth >= start && halfMonth <= end;
    }
    return halfMonth >= start || halfMonth <= end;
  }

  private getCurrentHalfMonth(): number {
    const now = new Date();
    const month = now.getMonth();
    const day = now.getDate();
    return month * 2 + (day <= 15 ? 1 : 2);
  }
}
