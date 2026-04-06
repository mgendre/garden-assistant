import { Injectable, inject, signal } from '@angular/core';
import {
  GardenDto,
  CreateGardenRequest,
  UpdateGardenRequest,
  BedDto,
  CreateBedRequest,
  UpdateBedRequest,
} from '../../api/garden-assistant-api';
import { GardenService } from './garden.service';
import { WateringStore } from './watering.store';

@Injectable({ providedIn: 'root' })
export class GardenStore {
  private readonly service = inject(GardenService);
  private readonly wateringStore = inject(WateringStore);

  readonly gardens = signal<GardenDto[]>([]);
  readonly beds = signal<BedDto[]>([]);
  readonly loading = signal(false);
  readonly bedsLoading = signal(false);

  async loadGardens(): Promise<void> {
    this.loading.set(true);
    try {
      const gardens = await this.service.getAll();
      this.gardens.set(gardens);
    } finally {
      this.loading.set(false);
    }
  }

  async createGarden(request: CreateGardenRequest): Promise<GardenDto> {
    const garden = await this.service.create(request);
    this.gardens.update(list => [garden, ...list]);
    return garden;
  }

  async updateGarden(id: string, request: UpdateGardenRequest): Promise<GardenDto> {
    const updated = await this.service.update(id, request);
    this.gardens.update(list => list.map(g => g.id === id ? updated : g));
    return updated;
  }

  async deleteGarden(id: string): Promise<void> {
    await this.service.delete(id);
    this.gardens.update(list => list.filter(g => g.id !== id));
  }

  async loadBeds(gardenId: string): Promise<void> {
    this.bedsLoading.set(true);
    try {
      const beds = await this.service.getBeds(gardenId);
      this.beds.set(beds);
    } finally {
      this.bedsLoading.set(false);
    }
  }

  async createBed(gardenId: string, request: CreateBedRequest): Promise<BedDto> {
    const bed = await this.service.createBed(gardenId, request);
    this.beds.update(list => [...list, bed]);
    this.gardens.update(list => list.map(g =>
      g.id === gardenId ? { ...g, bedCount: (g.bedCount ?? 0) + 1 } as GardenDto : g
    ));
    return bed;
  }

  async updateBed(gardenId: string, bedId: string, request: UpdateBedRequest): Promise<BedDto> {
    const updated = await this.service.updateBed(gardenId, bedId, request);
    this.beds.update(list => list.map(b => b.id === bedId ? updated : b));
    await this.wateringStore.invalidate();
    return updated;
  }

  async deleteBed(gardenId: string, bedId: string): Promise<void> {
    await this.service.deleteBed(gardenId, bedId);
    this.beds.update(list => list.filter(b => b.id !== bedId));
    this.gardens.update(list => list.map(g =>
      g.id === gardenId ? { ...g, bedCount: Math.max((g.bedCount ?? 0) - 1, 0) } as GardenDto : g
    ));
  }

  clearBeds(): void {
    this.beds.set([]);
  }
}
