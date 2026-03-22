import { Injectable, inject } from '@angular/core';
import {
  CalendarClient,
  CalendarDto,
  PlantsClient,
  PlantActionDto,
  HarvestReadinessDto,
  SwaggerException,
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

  async getHarvestReadiness(plantId: string): Promise<HarvestReadinessDto | null> {
    return await this.plantsClient.getHarvestReadiness(plantId) ?? null;
  }
}
