import { Injectable, inject } from '@angular/core';
import {
  PlantsClient,
  PlantAssociationsClient,
  GuildsClient,
  PlantDto,
  PaginatedResultOfPlantDto,
  CompanionSearchResultDto,
  CompanionRecommendationRequest,
  GuildDetailDto,
} from '../../api/garden-assistant-api';

@Injectable({ providedIn: 'root' })
export class CompanionService {
  private readonly plantsClient = inject(PlantsClient);
  private readonly associationsClient = inject(PlantAssociationsClient);
  private readonly guildsClient = inject(GuildsClient);

  getPlants(search?: string): Promise<PaginatedResultOfPlantDto> {
    return this.plantsClient.getAll(search);
  }

  getRecommendations(request: CompanionRecommendationRequest): Promise<CompanionSearchResultDto> {
    return this.associationsClient.getCompanionRecommendations(request);
  }

  getGuildById(id: string): Promise<GuildDetailDto> {
    return this.guildsClient.getById(id);
  }
}
