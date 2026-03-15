import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { GuildsClient, GuildSummaryDto } from '../../api/garden-assistant-api';

@Component({
  selector: 'app-guild-list',
  standalone: true,
  imports: [RouterLink, TranslateModule],
  templateUrl: './guild-list.html'
})
export class GuildListComponent implements OnInit {
  private readonly guildsClient = inject(GuildsClient);
  private readonly translate = inject(TranslateService);

  readonly guilds = signal<GuildSummaryDto[]>([]);
  readonly loading = signal(true);

  async ngOnInit(): Promise<void> {
    try {
      const result = await this.guildsClient.getAll();
      this.guilds.set(result);
    } finally {
      this.loading.set(false);
    }
  }
}
