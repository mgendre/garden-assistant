import { Component, inject, input, computed, signal, effect } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { faDroplet } from '@fortawesome/free-solid-svg-icons';
import { FaIconComponent } from '@fortawesome/angular-fontawesome';
import { WateringStore } from '../../services/watering.store';
import { WEEK_DAYS, DayOfWeekStr } from '../../../api/watering.api';
import { Collapsible } from '../collapsible/collapsible';
import { WaterNeedBadge } from '../water-need-badge/water-need-badge';

@Component({
  selector: 'app-bed-watering-section',
  standalone: true,
  imports: [TranslateModule, FaIconComponent, Collapsible, WaterNeedBadge],
  templateUrl: './bed-watering-section.html',
  styleUrl: './bed-watering-section.scss'
})
export class BedWateringSection {
  readonly gardenId = input.required<string>();
  readonly bedId = input.required<string>();

  protected readonly store = inject(WateringStore);

  readonly plants = computed(() => this.store.getBed(this.bedId())?.plants ?? []);

  private readonly isExpanded = signal(false);

  protected readonly faDroplet = faDroplet;
  protected readonly weekDays = WEEK_DAYS;
  protected readonly todayDayOfWeek = this.getTodayDayOfWeek();
  protected readonly weekDayHeaders = WEEK_DAYS.map((day, i) => {
    const date = new Date();
    date.setDate(date.getDate() - date.getDay() + 1 + i);
    return { day, number: date.getDate(), isToday: day === this.todayDayOfWeek };
  });

  constructor() {
    effect(() => {
      if (this.isExpanded() && !this.store.schedule() && !this.store.loading()) {
        this.store.load(this.gardenId(), this.getHalfMonth());
      }
    });
  }

  onToggled(expanded: boolean): void {
    this.isExpanded.set(expanded);
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
