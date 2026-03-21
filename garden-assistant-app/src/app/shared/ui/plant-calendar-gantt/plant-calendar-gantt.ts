import { Component, inject, input, computed, output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faSnowflake, faCircleQuestion } from '@fortawesome/free-solid-svg-icons';
import { PlantActionDto, PlantActionType, PropagationMethod } from '../../../api/garden-assistant-api';
import { BadgeInfoDialog, BadgeInfoDialogData } from '../badge-info-dialog/badge-info-dialog';
import { ACTION_COLORS, ACTION_TYPE_CONFIGS, FROST_SENSITIVE_ACTIONS, FROST_HALF_MONTHS_START, FROST_HALF_MONTHS_END, SOWING_ACTIONS } from '../../constants/plant-action.constants';

const MONTH_LABELS = ['J', 'F', 'M', 'A', 'M', 'J', 'J', 'A', 'S', 'O', 'N', 'D'];

const BULB_TUBER_METHODS = [PropagationMethod.Bulb, PropagationMethod.Tuber];

const ACTION_TYPE_ORDER = ACTION_TYPE_CONFIGS.map(c => c.type);

interface GanttRow {
  actionType: PlantActionType;
  labelKey: string;
  color: string;
  actions: PlantActionDto[];
}

interface GanttCell {
  halfMonth: number;
  isFirstHalf: boolean;
  isCurrent: boolean;
}

@Component({
  selector: 'app-plant-calendar-gantt',
  standalone: true,
  imports: [TranslateModule, FontAwesomeModule],
  templateUrl: './plant-calendar-gantt.html',
  styleUrl: './plant-calendar-gantt.scss',
})
export class PlantCalendarGantt {
  private readonly dialog = inject(MatDialog);

  readonly actions = input<PlantActionDto[]>([]);
  readonly propagationMethod = input<PropagationMethod>(PropagationMethod.Seed);
  readonly frostSensitive = input(false);
  readonly showHeader = input(true);
  readonly activeFilters = input<PlantActionType[]>([]);
  readonly plantName = input<string>('');
  readonly oddPlant = input(false);
  readonly plantNameClick = output<void>();
  readonly hasHarvestReadiness = input(false);
  readonly harvestReadinessClick = output<void>();

  protected readonly faSnowflake = faSnowflake;
  protected readonly faCircleQuestion = faCircleQuestion;
  protected readonly PlantActionType = PlantActionType;
  protected readonly monthLabels = MONTH_LABELS;

  readonly currentHalfMonth = computed(() => {
    const now = new Date();
    const month = now.getMonth();
    const day = now.getDate();
    return month * 2 + (day <= 15 ? 1 : 2);
  });

  readonly monthHeaders = computed<GanttCell[][]>(() => {
    const currentHm = this.currentHalfMonth();
    const result: GanttCell[][] = [];
    for (let m = 0; m < 12; m++) {
      const hm1 = m * 2 + 1;
      const hm2 = m * 2 + 2;
      result.push([
        { halfMonth: hm1, isFirstHalf: true, isCurrent: hm1 === currentHm },
        { halfMonth: hm2, isFirstHalf: false, isCurrent: hm2 === currentHm },
      ]);
    }
    return result;
  });

  readonly rows = computed<GanttRow[]>(() => {
    const allActions = this.actions() ?? [];
    const method = this.propagationMethod();
    const usePlantation = BULB_TUBER_METHODS.includes(method);
    const filters = this.activeFilters();

    const grouped = new Map<PlantActionType, PlantActionDto[]>();
    for (const action of allActions) {
      if (action.actionType === undefined) {
        continue;
      }
      if (filters.length > 0 && !filters.includes(action.actionType)) {
        continue;
      }
      const existing = grouped.get(action.actionType) ?? [];
      existing.push(action);
      grouped.set(action.actionType, existing);
    }

    return ACTION_TYPE_ORDER
      .filter(type => grouped.has(type))
      .map(type => ({
        actionType: type,
        labelKey: this.getActionLabelKey(type, usePlantation),
        color: ACTION_COLORS[type] ?? '#9ca3af',
        actions: grouped.get(type)!,
      }));
  });

  isCellActive(row: GanttRow, halfMonth: number): boolean {
    return row.actions.some(action => this.isActiveInHalfMonth(action, halfMonth));
  }

  isFirstBar(row: GanttRow, halfMonth: number): boolean {
    if (halfMonth === 1) {
      return true;
    }
    return !this.isCellActive(row, halfMonth - 1);
  }

  isLastBar(row: GanttRow, halfMonth: number): boolean {
    if (halfMonth === 24) {
      return true;
    }
    return !this.isCellActive(row, halfMonth + 1);
  }

  onPlantNameClick(): void {
    this.plantNameClick.emit();
  }

  openActionInfo(actionType: PlantActionType): void {
    const key = ACTION_TYPE_CONFIGS.find(c => c.type === actionType)?.badgeKey;
    if (key) {
      this.dialog.open<BadgeInfoDialog, BadgeInfoDialogData>(BadgeInfoDialog, {
        data: {
          titleKey: `BadgeInfo.Action.${key}.Title`,
          descriptionKey: `BadgeInfo.Action.${key}.Description`,
        },
        maxWidth: '400px',
      });
    }
  }

  showFrostIndicator(row: GanttRow, halfMonth: number): boolean {
    if (!this.frostSensitive()) {
      return false;
    }
    if (halfMonth < FROST_HALF_MONTHS_START || halfMonth > FROST_HALF_MONTHS_END) {
      return false;
    }
    if (!FROST_SENSITIVE_ACTIONS.includes(row.actionType)) {
      return false;
    }
    return this.isCellActive(row, halfMonth);
  }

  private getActionLabelKey(type: PlantActionType, usePlantation: boolean): string {
    if (usePlantation && SOWING_ACTIONS.includes(type)) {
      return 'Calendar.ActionType.Plantation';
    }
    switch (type) {
      case PlantActionType.IndoorSowing: return 'Calendar.ActionType.IndoorSowing';
      case PlantActionType.DirectSowing: return 'Calendar.ActionType.DirectSowing';
      case PlantActionType.Transplanting: return 'Calendar.ActionType.Transplanting';
      case PlantActionType.Harvest: return 'Calendar.ActionType.Harvest';
      case PlantActionType.Pruning: return 'Calendar.ActionType.Pruning';
      case PlantActionType.Pinching: return 'Calendar.ActionType.Pinching';
      case PlantActionType.Hilling: return 'Calendar.ActionType.Hilling';
      case PlantActionType.Division: return 'Calendar.ActionType.Division';
      default: return '';
    }
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
}
