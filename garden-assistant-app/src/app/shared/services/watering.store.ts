import { Injectable, inject, signal, computed } from '@angular/core';
import { WateringService } from './watering.service';
import { WateringScheduleDto, BedWateringDto } from '../../api/watering.api';

@Injectable({ providedIn: 'root' })
export class WateringStore {
  private readonly service = inject(WateringService);

  readonly schedule = signal<WateringScheduleDto | null>(null);
  readonly loading = signal(false);

  private gardenId: string | null = null;
  private halfMonth: number | null = null;

  readonly beds = computed(() => this.schedule()?.beds ?? []);

  getBed(bedId: string): BedWateringDto | null {
    return this.schedule()?.beds.find(b => b.bedId === bedId) ?? null;
  }

  async load(gardenId: string, halfMonth: number): Promise<void> {
    this.gardenId = gardenId;
    this.halfMonth = halfMonth;
    this.loading.set(true);
    try {
      this.schedule.set(await this.service.getGardenSchedule(gardenId, halfMonth));
    } finally {
      this.loading.set(false);
    }
  }

  async invalidate(): Promise<void> {
    this.schedule.set(null);
    if (this.gardenId && this.halfMonth) {
      await this.load(this.gardenId, this.halfMonth);
    }
  }
}
