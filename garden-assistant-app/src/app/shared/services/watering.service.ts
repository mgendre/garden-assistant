import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { WateringTodayDto, WateringScheduleDto } from '../../api/watering.api';

@Injectable({ providedIn: 'root' })
export class WateringService {
  private readonly http = inject(HttpClient);

  getWateringToday(): Promise<WateringTodayDto> {
    return firstValueFrom(this.http.get<WateringTodayDto>('/api/calendar/watering/today'));
  }

  getWateringSchedule(halfMonth: number, source: string): Promise<WateringScheduleDto> {
    return firstValueFrom(
      this.http.get<WateringScheduleDto>('/api/calendar/watering/schedule', {
        params: { halfMonth, source }
      })
    );
  }
}
