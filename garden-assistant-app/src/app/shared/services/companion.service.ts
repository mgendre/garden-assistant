import { Injectable, inject } from '@angular/core';
import {
  PlantAssociationsClient,
  GuildsClient,
  CompanionSearchResultDto,
  CompanionRecommendationRequest,
  GuildDetailDto,
} from '../../api/garden-assistant-api';

@Injectable({ providedIn: 'root' })
export class CompanionService {
  private readonly associationsClient = inject(PlantAssociationsClient);
  private readonly guildsClient = inject(GuildsClient);

  getRecommendations(request: CompanionRecommendationRequest): Promise<CompanionSearchResultDto> {
    return this.associationsClient.getCompanionRecommendations(request);
  }

  getGuildById(id: string): Promise<GuildDetailDto> {
    return this.guildsClient.getById(id);
  }
}
