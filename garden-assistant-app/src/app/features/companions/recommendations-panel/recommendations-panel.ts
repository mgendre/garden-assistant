import { Component, inject, signal, computed, effect, untracked } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { CompanionService } from '../companion.service';
import { CompanionRecommendationDto, GuildInfoDto, GuildDetailDto } from '../../../api/garden-assistant-api';

@Component({
  selector: 'app-recommendations-panel',
  standalone: true,
  imports: [TranslateModule],
  templateUrl: './recommendations-panel.html',
  styleUrl: './recommendations-panel.scss'
})
export class RecommendationsPanel {
  protected readonly service = inject(CompanionService);
  readonly guildLoading = signal<string | null>(null);
  readonly guildDetails = signal<Map<string, GuildDetailDto>>(new Map());

  readonly uniqueGuilds = computed(() => {
    const recs = this.service.recommendations();
    if (!recs?.goodCompanions) return [];
    const seen = new Set<string>();
    const guilds: GuildInfoDto[] = [];
    for (const companion of recs.goodCompanions) {
      for (const guild of companion.guilds ?? []) {
        if (guild.id && !seen.has(guild.id)) {
          seen.add(guild.id);
          guilds.push(guild);
        }
      }
    }
    return guilds;
  });

  constructor() {
    effect(() => {
      const guilds = this.uniqueGuilds();
      untracked(() => this.loadGuildDetails(guilds));
    });
  }

  onCompanionClick(companion: CompanionRecommendationDto): void {
    const plant = this.service.plants().find(p => p.id === companion.plantId);
    if (plant) {
      this.service.addPlant(plant);
    }
  }

  onGuildPlantClick(plantId: string | undefined): void {
    if (!plantId) return;
    const plant = this.service.plants().find(p => p.id === plantId);
    if (plant) {
      this.service.addPlant(plant);
    }
  }

  async onGuildClick(guild: GuildInfoDto): Promise<void> {
    if (!guild.id || this.guildLoading()) return;
    this.guildLoading.set(guild.id);
    try {
      const detail = this.guildDetails().get(guild.id)
        ?? await this.service.loadGuildPlants(guild.id);
      const allPlants = this.service.plants();
      for (const guildPlant of detail.plants ?? []) {
        const plant = allPlants.find(p => p.id === guildPlant.id);
        if (plant) {
          this.service.addPlant(plant);
        }
      }
    } finally {
      this.guildLoading.set(null);
    }
  }

  getGuildPlants(guildId: string | undefined): GuildDetailDto | undefined {
    if (!guildId) return undefined;
    return this.guildDetails().get(guildId);
  }

  getPlantEmojiById(plantId: string | undefined): string {
    if (!plantId) return '🌱';
    const plant = this.service.plants().find(p => p.id === plantId);
    return plant ? this.service.getPlantEmoji(plant) : '🌱';
  }

  getPlantEmojiByName(name: string | undefined): string {
    if (!name) return '🌱';
    const plant = this.service.plants().find(p => p.name === name);
    return plant ? this.service.getPlantEmoji(plant) : '🌱';
  }

  getMechanismTranslationKey(mechanism: number): string {
    const key = this.service.getMechanismKey(mechanism);
    return key ? `Companions.Mechanism.${key}` : '';
  }

  private async loadGuildDetails(guilds: GuildInfoDto[]): Promise<void> {
    const current = this.guildDetails();
    const toLoad = guilds.filter(g => g.id && !current.has(g.id));
    if (toLoad.length === 0) return;

    const results = await Promise.all(
      toLoad.map(g => this.service.loadGuildPlants(g.id!))
    );

    this.guildDetails.update(map => {
      const next = new Map(map);
      for (const detail of results) {
        if (detail.id) next.set(detail.id, detail);
      }
      return next;
    });
  }
}
