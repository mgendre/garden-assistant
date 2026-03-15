import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { GuildsClient, GuildDetailDto } from '../../api/garden-assistant-api';

@Component({
  selector: 'app-guild-detail',
  standalone: true,
  imports: [RouterLink, TranslateModule],
  templateUrl: './guild-detail.html'
})
export class GuildDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly guildsClient = inject(GuildsClient);

  readonly guild = signal<GuildDetailDto | null>(null);
  readonly loading = signal(true);

  async ngOnInit(): Promise<void> {
    const id = this.route.snapshot.paramMap.get('id')!;
    try {
      const result = await this.guildsClient.getById(id);
      this.guild.set(result);
    } finally {
      this.loading.set(false);
    }
  }
}
