import { Component, inject, input, output, effect } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { faDroplet } from '@fortawesome/free-solid-svg-icons';
import { FaIconComponent } from '@fortawesome/angular-fontawesome';
import { WateringStore } from '../../../shared/services/watering.store';
import { WEEK_DAYS, DayOfWeekStr } from '../../../api/watering.api';
import { Collapsible } from '../../../shared/ui/collapsible/collapsible';
import { WaterNeedBadge } from '../../../shared/ui/water-need-badge/water-need-badge';

@Component({
  selector: 'app-garden-watering',
  standalone: true,
  imports: [TranslateModule, FaIconComponent, Collapsible, WaterNeedBadge],
  templateUrl: './garden-watering.html',
  styleUrl: './garden-watering.scss'
})
export class GardenWatering {
  readonly gardenId = input.required<string>();
  readonly open = input<boolean | null>(null);
  readonly toggled = output<boolean>();

  protected readonly store = inject(WateringStore);

  protected readonly faDroplet = faDroplet;
  protected readonly weekDays = WEEK_DAYS;
  protected readonly todayDayOfWeek = this.getTodayDayOfWeek();
  protected readonly weekDayHeaders = WEEK_DAYS.map((day, i) => {
    const date = new Date();
    date.setDate(date.getDate() - date.getDay() + 1 + i);
    return { day, number: date.getDate(), isToday: day === this.todayDayOfWeek };
  });

  private hasLoaded = false;

  constructor() {
    effect(() => {
      if (this.open() && !this.hasLoaded) {
        this.hasLoaded = true;
        this.store.load(this.gardenId(), this.getHalfMonth());
      }
    });
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
}
