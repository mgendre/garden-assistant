export type DayOfWeekStr = 'Monday' | 'Tuesday' | 'Wednesday' | 'Thursday' | 'Friday' | 'Saturday' | 'Sunday';

export const WEEK_DAYS: DayOfWeekStr[] = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];

export interface PlantWateringDto {
  plantId: string;
  plantName: string;
  waterNeeds: string;
  timesPerWeek: number;
  recommendedDays: DayOfWeekStr[];
}

export interface BedWateringDto {
  bedId?: string;
  bedName: string;
  isPersonalPlants: boolean;
  soilType?: string;
  hasMulch: boolean;
  plants: PlantWateringDto[];
}

export interface WateringScheduleDto {
  beds: BedWateringDto[];
}
