import { Injectable, inject, signal, computed } from '@angular/core';
import { GuildDto } from '../../api/garden-assistant-api';
import { GuildService } from './guild.service';

@Injectable({ providedIn: 'root' })
export class GuildStore {
  private readonly service = inject(GuildService);

  readonly guilds = signal<GuildDto[]>([]);
  readonly loading = signal(false);

  readonly officialGuilds = computed(() =>
    this.guilds().filter(g => g.isOfficial)
  );

  readonly userGuilds = computed(() =>
    this.guilds().filter(g => !g.isOfficial)
  );

  async load(): Promise<void> {
    this.loading.set(true);
    try {
      const guilds = await this.service.getAll();
      this.guilds.set(guilds);
    } finally {
      this.loading.set(false);
    }
  }

  async deleteGuild(id: string): Promise<void> {
    await this.service.delete(id);
    this.guilds.update(list => list.filter(g => g.id !== id));
  }
}
