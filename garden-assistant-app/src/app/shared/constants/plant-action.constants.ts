import { PlantActionType } from '../../api/garden-assistant-api';

export interface ActionTypeConfig {
  type: PlantActionType;
  color: string;
  labelKey: string;
  badgeKey: string;
}

export const ACTION_TYPE_CONFIGS: ActionTypeConfig[] = [
  { type: PlantActionType.IndoorSowing, color: '#2563eb', labelKey: 'Calendar.ActionType.IndoorSowing', badgeKey: 'IndoorSowing' },
  { type: PlantActionType.DirectSowing, color: '#60a5fa', labelKey: 'Calendar.ActionType.DirectSowing', badgeKey: 'DirectSowing' },
  { type: PlantActionType.Transplanting, color: '#22c55e', labelKey: 'Calendar.ActionType.Transplanting', badgeKey: 'Transplanting' },
  { type: PlantActionType.Harvest, color: '#f59e0b', labelKey: 'Calendar.ActionType.Harvest', badgeKey: 'Harvest' },
  { type: PlantActionType.Pruning, color: '#ef4444', labelKey: 'Calendar.ActionType.Pruning', badgeKey: 'Pruning' },
  { type: PlantActionType.Pinching, color: '#ec4899', labelKey: 'Calendar.ActionType.Pinching', badgeKey: 'Pinching' },
  { type: PlantActionType.Hilling, color: '#8b5cf6', labelKey: 'Calendar.ActionType.Hilling', badgeKey: 'Hilling' },
  { type: PlantActionType.Division, color: '#7c3aed', labelKey: 'Calendar.ActionType.Division', badgeKey: 'Division' },
];

export const ACTION_COLORS: Record<number, string> = Object.fromEntries(
  ACTION_TYPE_CONFIGS.map(c => [c.type, c.color])
);

export const FROST_SENSITIVE_ACTIONS = [PlantActionType.Transplanting, PlantActionType.DirectSowing];
export const FROST_HALF_MONTHS_START = 1;
export const FROST_HALF_MONTHS_END = 10;

export const SOWING_ACTIONS = [PlantActionType.IndoorSowing, PlantActionType.DirectSowing];

export interface FilterConfig {
  key: string;
  labelKey: string;
  color: string;
  actionTypes: PlantActionType[];
}

export const FILTER_CONFIGS: FilterConfig[] = [
  { key: 'sowing', labelKey: 'Calendar.Filter.Sowing', color: '#3b82f6', actionTypes: [PlantActionType.IndoorSowing, PlantActionType.DirectSowing] },
  { key: 'transplanting', labelKey: 'Calendar.ActionType.Transplanting', color: '#22c55e', actionTypes: [PlantActionType.Transplanting] },
  { key: 'harvest', labelKey: 'Calendar.ActionType.Harvest', color: '#f59e0b', actionTypes: [PlantActionType.Harvest] },
  { key: 'pruning', labelKey: 'Calendar.ActionType.Pruning', color: '#ef4444', actionTypes: [PlantActionType.Pruning] },
  { key: 'pinching', labelKey: 'Calendar.ActionType.Pinching', color: '#ec4899', actionTypes: [PlantActionType.Pinching] },
  { key: 'hilling', labelKey: 'Calendar.ActionType.Hilling', color: '#6366f1', actionTypes: [PlantActionType.Hilling] },
  { key: 'division', labelKey: 'Calendar.ActionType.Division', color: '#7c3aed', actionTypes: [PlantActionType.Division] },
];
