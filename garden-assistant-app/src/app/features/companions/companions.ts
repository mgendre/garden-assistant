import { Component, computed, inject, signal } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import {
  AssociationMechanism,
  PlantDto,
} from '../../api/garden-assistant-api';
import { CompanionSearchService } from './companion-search.service';

interface GuildGroup {
  id: string;
  name: string;
  description: string | undefined;
  plants: { plantId: string; plantName: string; scientificName: string | undefined }[];
}

const MECHANISM_KEYS: Record<AssociationMechanism, string> = {
  [AssociationMechanism.OlfactoryConfusion]: 'Companions.Mechanism.OlfactoryConfusion',
  [AssociationMechanism.PollinatorAttraction]: 'Companions.Mechanism.PollinatorAttraction',
  [AssociationMechanism.TrapCrop]: 'Companions.Mechanism.TrapCrop',
  [AssociationMechanism.RootAllelopathy]: 'Companions.Mechanism.RootAllelopathy',
  [AssociationMechanism.AerialRepulsion]: 'Companions.Mechanism.AerialRepulsion',
  [AssociationMechanism.NitrogenFixation]: 'Companions.Mechanism.NitrogenFixation',
  [AssociationMechanism.PredatorAttraction]: 'Companions.Mechanism.PredatorAttraction',
  [AssociationMechanism.PhysicalSupport]: 'Companions.Mechanism.PhysicalSupport',
  [AssociationMechanism.SoilCover]: 'Companions.Mechanism.SoilCover',
  [AssociationMechanism.DynamicAccumulation]: 'Companions.Mechanism.DynamicAccumulation',
  [AssociationMechanism.MycorrhizalNetwork]: 'Companions.Mechanism.MycorrhizalNetwork',
  [AssociationMechanism.HydraulicLift]: 'Companions.Mechanism.HydraulicLift',
  [AssociationMechanism.MicroclimateModification]: 'Companions.Mechanism.MicroclimateModification',
  [AssociationMechanism.WeedSuppression]: 'Companions.Mechanism.WeedSuppression',
  [AssociationMechanism.Biofumigation]: 'Companions.Mechanism.Biofumigation',
  [AssociationMechanism.NursePlant]: 'Companions.Mechanism.NursePlant',
};

@Component({
  selector: 'app-companions',
  standalone: true,
  imports: [DecimalPipe, RouterLink, TranslateModule],
  providers: [CompanionSearchService],
  templateUrl: './companions.html'
})
export class CompanionsComponent {
  readonly service = inject(CompanionSearchService);
  private readonly translate = inject(TranslateService);
  readonly searchTerm = signal('');
  private searchTimeout: ReturnType<typeof setTimeout> | null = null;

  readonly showDropdown = computed(() =>
    this.searchTerm().length >= 1 &&
    (this.service.searchResults().length > 0 || this.service.searchLoading() || this.searchTerm().length >= 1)
  );

  readonly goodCompanions = computed(() =>
    this.service.companionResults()?.goodCompanions ?? []
  );

  readonly plantsToAvoid = computed(() =>
    this.service.companionResults()?.plantsToAvoid ?? []
  );

  readonly guildGroups = computed<GuildGroup[]>(() => {
    const companions = this.goodCompanions();
    const guildMap = new Map<string, GuildGroup>();
    for (const companion of companions) {
      if (!companion.guilds) continue;
      for (const guild of companion.guilds) {
        if (!guild.id) continue;
        let group = guildMap.get(guild.id);
        if (!group) {
          group = { id: guild.id, name: guild.name!, description: guild.description ?? undefined, plants: [] };
          guildMap.set(guild.id, group);
        }
        group.plants.push({
          plantId: companion.plantId!,
          plantName: companion.plantName!,
          scientificName: companion.scientificName ?? undefined
        });
      }
    }
    return Array.from(guildMap.values());
  });

  onSearchInput(value: string): void {
    this.searchTerm.set(value);
    if (this.searchTimeout) clearTimeout(this.searchTimeout);
    if (value.length < 1) {
      this.service.searchResults.set([]);
      return;
    }
    this.searchTimeout = setTimeout(() => this.service.searchPlants(value), 300);
  }

  selectPlant(plant: PlantDto): void {
    if (this.service.isSelected(plant)) return;
    this.service.addPlant(plant);
    this.searchTerm.set('');
    this.service.searchResults.set([]);
  }

  removePlant(plant: PlantDto): void {
    this.service.removePlant(plant);
  }

  clearAll(): void {
    this.service.clearAll();
    this.searchTerm.set('');
  }

  clearSearch(): void {
    this.searchTerm.set('');
    this.service.searchResults.set([]);
  }

  mechanismLabel(mechanism: AssociationMechanism | undefined): string {
    if (mechanism === undefined) return '';
    const key = MECHANISM_KEYS[mechanism];
    return key ? this.translate.instant(key) : '';
  }

  isSelected(plant: PlantDto): boolean {
    return this.service.isSelected(plant);
  }

}
