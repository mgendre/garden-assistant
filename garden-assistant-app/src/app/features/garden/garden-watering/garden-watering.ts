import { Component, inject, input, output, signal, computed, effect } from '@angular/core';
import { NgClass } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { faDroplet } from '@fortawesome/free-solid-svg-icons';
import { FaIconComponent } from '@fortawesome/angular-fontawesome';
import { WateringService } from '../../../shared/services/watering.service';
import { WateringScheduleDto, WEEK_DAYS, DayOfWeekStr } from '../../../api/watering.api';
import { Collapsible } from '../../../shared/ui/collapsible/collapsible';

@Component({
  selector: 'app-garden-watering',
  standalone: true,
  imports: [TranslateModule, FaIconComponent, NgClass, Collapsible],
  templateUrl: './garden-watering.html',
  styleUrl: './garden-watering.scss'
})
export class GardenWatering {
  readonly gardenId = input.required<string>();
  readonly open = input<boolean | null>(null);
  readonly toggled = output<boolean>();

  private readonly wateringService = inject(WateringService);

  readonly scheduleData = signal<WateringScheduleDto | null>(null);
  readonly loading = signal(false);
  readonly beds = computed(() => this.scheduleData()?.beds ?? []);

  protected readonly faDroplet = faDroplet;
  protected readonly weekDays = WEEK_DAYS;
  protected readonly todayDayOfWeek = this.getTodayDayOfWeek();
  protected readonly weekDayHeaders = WEEK_DAYS.map((day, i) => {
    const date = new Date();
    date.setDate(date.getDate() - date.getDay() + 1 + i);
    return { day, number: date.getDate(), isToday: day === this.todayDayOfWeek };
  });

  private readonly hasLoaded = signal(false);

  constructor() {
    effect(() => {
      if (this.open() && !this.hasLoaded()) {
        this.hasLoaded.set(true);
        this.loadSchedule();
      }
    });
  }

  private async loadSchedule(): Promise<void> {
    this.loading.set(true);
    try {
      const halfMonth = this.getHalfMonth();
      this.scheduleData.set(await this.wateringService.getGardenSchedule(this.gardenId(), halfMonth));
    } finally {
      this.loading.set(false);
    }
  }

  private getHalfMonth(): number {
    const date = new Date();
    return date.getMonth() * 2 + (date.getDate() <= 15 ? 1 : 2);
  }

  private getTodayDayOfWeek(): DayOfWeekStr {
    return WEEK_DAYS[new Date().getDay() === 0 ? 6 : new Date().getDay() - 1];
  }

  hasDot(day: DayOfWeekStr, days: DayOfWeekStr[]): boolean {
    return days.includes(day);
  }

  waterNeedClass(waterNeeds: string): string {
    return `water-need-badge--${waterNeeds.toLowerCase()}`;
  }
}
