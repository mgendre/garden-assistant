import { Component, inject, OnInit } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { faDroplet } from '@fortawesome/free-solid-svg-icons';
import { FaIconComponent } from '@fortawesome/angular-fontawesome';
import { WateringStore } from '../../../shared/services/watering.store';

@Component({
  selector: 'app-calendar-watering-today',
  standalone: true,
  imports: [TranslateModule, FaIconComponent],
  templateUrl: './calendar-watering-today.html',
  styleUrl: './calendar-watering-today.scss',
  host: { style: 'display:block' }
})
export class CalendarWateringToday implements OnInit {
  protected readonly store = inject(WateringStore);
  protected readonly faDroplet = faDroplet;

  async ngOnInit(): Promise<void> {
    await this.store.loadToday();
  }
}
