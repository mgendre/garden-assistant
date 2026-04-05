import { Injectable, inject, signal, computed, effect } from '@angular/core';
import { CalendarStore } from './calendar.store';
import { WateringService } from './watering.service';
import { WateringTodayDto, WateringScheduleDto, DayOfWeekStr } from '../../api/watering.api';

const DAY_LABELS: Record<DayOfWeekStr, string> = {
  Monday: 'Watering.Day.Monday',
  Tuesday: 'Watering.Day.Tuesday',
  Wednesday: 'Watering.Day.Wednesday',
  Thursday: 'Watering.Day.Thursday',
  Friday: 'Watering.Day.Friday',
  Saturday: 'Watering.Day.Saturday',
  Sunday: 'Watering.Day.Sunday',
};

@Injectable({ providedIn: 'root' })
export class WateringStore {
  private readonly calendarStore = inject(CalendarStore);
  private readonly wateringService = inject(WateringService);

  readonly todayData = signal<WateringTodayDto | null>(null);
  readonly scheduleData = signal<WateringScheduleDto | null>(null);
  readonly loadingToday = signal(false);
  readonly loadingSchedule = signal(false);
  readonly weekOffset = signal(0);
  readonly scheduleTabActive = signal(false);

  readonly todayPlants = computed(() =>
    this.todayData()?.beds.flatMap(b => b.plants.filter(p => p.isToday)) ?? []
  );

  readonly nextWateringDayKey = computed(() => {
    const allPlants = this.todayData()?.beds.flatMap(b => b.plants) ?? [];
    const first = allPlants.find(p => !p.isToday && p.nextWateringDay);
    return first?.nextWateringDay ? DAY_LABELS[first.nextWateringDay] : null;
  });

  private readonly reloadOnFilterChange = effect(() => {
    this.calendarStore.sourceFilter();
    if (this.scheduleTabActive()) {
      this.loadSchedule();
    }
  });

  async loadToday(): Promise<void> {
    this.loadingToday.set(true);
    try {
      this.todayData.set(await this.wateringService.getWateringToday());
    } finally {
      this.loadingToday.set(false);
    }
  }

  async loadSchedule(): Promise<void> {
    this.loadingSchedule.set(true);
    try {
      const halfMonth = this.getHalfMonth(this.weekOffset());
      const source = this.calendarStore.sourceFilter();
      this.scheduleData.set(await this.wateringService.getWateringSchedule(halfMonth, source));
    } finally {
      this.loadingSchedule.set(false);
    }
  }

  private getHalfMonth(weekOffset: number): number {
    const date = new Date();
    date.setDate(date.getDate() + weekOffset * 7);
    return date.getMonth() * 2 + (date.getDate() <= 15 ? 1 : 2);
  }
}
