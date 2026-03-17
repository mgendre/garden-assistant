import { Injectable, inject } from '@angular/core';
import { UserPlantsClient, PlantDto } from '../../api/garden-assistant-api';

@Injectable({ providedIn: 'root' })
export class MyPlantsService {
  private readonly client = inject(UserPlantsClient);

  getAll(): Promise<PlantDto[]> {
    return this.client.getAll();
  }

  add(plantId: string): Promise<PlantDto> {
    return this.client.add(plantId);
  }

  remove(plantId: string): Promise<void> {
    return this.client.remove(plantId);
  }
}
