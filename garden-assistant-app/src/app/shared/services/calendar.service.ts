import { Injectable, inject } from '@angular/core';
import {
  CalendarClient,
  CalendarDto,
  PlantsClient,
  PlantActionDto,
  HarvestReadinessDto,
} from '../../api/garden-assistant-api';

@Injectable({ providedIn: 'root' })
export class CalendarService {
  private readonly calendarClient = inject(CalendarClient);
  private readonly plantsClient = inject(PlantsClient);

  getMyPlantsCalendar(): Promise<CalendarDto> {
    return this.calendarClient.getMyPlantsCalendar();
  }

  getPlantActions(plantId: string): Promise<PlantActionDto[]> {
    return this.plantsClient.getActions(plantId);
  }

  getHarvestReadiness(plantId: string): Promise<HarvestReadinessDto | null> {
    return this.plantsClient.getHarvestReadiness(plantId);
  }
}
