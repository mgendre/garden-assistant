import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { WateringScheduleDto } from '../../api/watering.api';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class WateringService {
  private readonly http = inject(HttpClient);
  private readonly base = environment.apiBaseUrl;

  getGardenSchedule(gardenId: string, halfMonth: number): Promise<WateringScheduleDto> {
    return firstValueFrom(
      this.http.get<WateringScheduleDto>(`${this.base}/api/gardens/${gardenId}/watering/schedule`, {
        params: { halfMonth }
      })
    );
  }
}
