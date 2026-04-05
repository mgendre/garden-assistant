import { Component, inject, computed, input, effect } from '@angular/core';
import { NgClass } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { faChevronLeft, faChevronRight } from '@fortawesome/free-solid-svg-icons';
import { FaIconComponent } from '@fortawesome/angular-fontawesome';
import { WateringStore } from '../../../shared/services/watering.store';
import { WEEK_DAYS, DayOfWeekStr } from '../../../api/watering.api';
import { Collapsible } from '../../../shared/ui/collapsible/collapsible';

@Component({
  selector: 'app-calendar-watering',
  standalone: true,
  imports: [TranslateModule, FaIconComponent, NgClass, Collapsible],
  templateUrl: './calendar-watering.html',
  styleUrl: './calendar-watering.scss',
  host: { style: 'display:block' }
})
export class CalendarWatering {
  readonly active = input(false);
  protected readonly store = inject(WateringStore);
  protected readonly faLeft = faChevronLeft;
  protected readonly faRight = faChevronRight;
  protected readonly weekDays = WEEK_DAYS;
  protected readonly todayDayOfWeek = this.getTodayDayOfWeek();

  protected readonly weekDayHeaders = computed(() => {
    const offset = this.store.weekOffset();
    return WEEK_DAYS.map((day, i) => {
      const date = new Date();
      date.setDate(date.getDate() - date.getDay() + 1 + i + offset * 7);
      return { day, number: date.getDate(), isToday: offset === 0 && day === this.todayDayOfWeek };
    });
  });

  protected readonly beds = computed(() => this.store.scheduleData()?.beds ?? []);

  private readonly loadOnActivation = effect(() => {
    if (this.active()) {
      this.store.scheduleTabActive.set(true);
      this.store.loadSchedule();
    } else {
      this.store.scheduleTabActive.set(false);
    }
  });

  prevWeek(): void {
    if (this.store.weekOffset() > 0) {
      this.store.weekOffset.update(v => v - 1);
      this.store.loadSchedule();
    }
  }

  nextWeek(): void {
    if (this.store.weekOffset() < 1) {
      this.store.weekOffset.update(v => v + 1);
      this.store.loadSchedule();
    }
  }

  hasDot(day: DayOfWeekStr, days: DayOfWeekStr[]): boolean {
    return days.includes(day);
  }

  waterNeedClass(waterNeeds: string): string {
    return `water-need-badge--${waterNeeds.toLowerCase()}`;
  }

  private getTodayDayOfWeek(): DayOfWeekStr {
    return WEEK_DAYS[new Date().getDay() === 0 ? 6 : new Date().getDay() - 1];
  }
}
