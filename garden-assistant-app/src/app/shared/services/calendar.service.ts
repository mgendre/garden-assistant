import { Injectable, inject } from '@angular/core';
import { CalendarClient, CalendarDto } from '../../api/garden-assistant-api';

@Injectable({ providedIn: 'root' })
export class CalendarService {
  private readonly calendarClient = inject(CalendarClient);

  getMyPlantsCalendar(): Promise<CalendarDto> {
    return this.calendarClient.getMyPlantsCalendar();
  }
}
