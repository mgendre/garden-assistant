import { Injectable, inject } from '@angular/core';
import {
  PlantAssociationsClient,
  CompanionSearchResultDto,
  CompanionRecommendationRequest,
} from '../../api/garden-assistant-api';

@Injectable({ providedIn: 'root' })
export class CompanionService {
  private readonly associationsClient = inject(PlantAssociationsClient);

  getRecommendations(request: CompanionRecommendationRequest): Promise<CompanionSearchResultDto> {
    return this.associationsClient.getCompanionRecommendations(request);
  }
}
