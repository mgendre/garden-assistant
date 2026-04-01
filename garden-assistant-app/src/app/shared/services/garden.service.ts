import { Injectable, inject } from '@angular/core';
import {
  GardensClient,
  BedsClient,
  GardenDto,
  CreateGardenRequest,
  UpdateGardenRequest,
  BedDto,
  CreateBedRequest,
  UpdateBedRequest,
} from '../../api/garden-assistant-api';

@Injectable({ providedIn: 'root' })
export class GardenService {
  private readonly gardensClient = inject(GardensClient);
  private readonly bedsClient = inject(BedsClient);

  getAll(): Promise<GardenDto[]> {
    return this.gardensClient.getAll();
  }

  getById(id: string): Promise<GardenDto> {
    return this.gardensClient.getById(id);
  }

  create(request: CreateGardenRequest): Promise<GardenDto> {
    return this.gardensClient.create(request);
  }

  update(id: string, request: UpdateGardenRequest): Promise<GardenDto> {
    return this.gardensClient.update(id, request);
  }

  delete(id: string): Promise<void> {
    return this.gardensClient.delete(id);
  }

  getBeds(gardenId: string): Promise<BedDto[]> {
    return this.bedsClient.getAll(gardenId);
  }

  createBed(gardenId: string, request: CreateBedRequest): Promise<BedDto> {
    return this.bedsClient.create(gardenId, request);
  }

  updateBed(gardenId: string, bedId: string, request: UpdateBedRequest): Promise<BedDto> {
    return this.bedsClient.update(gardenId, bedId, request);
  }

  deleteBed(gardenId: string, bedId: string): Promise<void> {
    return this.bedsClient.delete(gardenId, bedId);
  }
}
