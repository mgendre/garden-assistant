import { inject, Injectable, signal } from '@angular/core';
import { CreateGardenRequest, GardenDto, GardensClient } from '../../api/garden-assistant-api';

@Injectable()
export class GardenService {
  private readonly client = inject(GardensClient);

  readonly gardens = signal<GardenDto[]>([]);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  async loadAll(): Promise<void> {
    this.loading.set(true);
    this.error.set(null);
    try {
      this.gardens.set(await this.client.getAll());
    } catch {
      this.error.set('Failed to load gardens');
    } finally {
      this.loading.set(false);
    }
  }

  async create(request: CreateGardenRequest): Promise<void> {
    await this.client.create(request);
    this.loading.set(true);
    this.error.set(null);
    try {
      this.gardens.set(await this.client.getAll());
    } catch {
      this.error.set('Failed to reload gardens');
    } finally {
      this.loading.set(false);
    }
  }

  async remove(id: string): Promise<void> {
    await this.client.delete(id);
    this.gardens.update(gs => gs.filter(g => g.id !== id));
  }
}
