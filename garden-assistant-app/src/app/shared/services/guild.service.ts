import { Injectable, inject } from '@angular/core';
import {
  GuildsClient,
  GuildDto,
  CreateGuildRequest,
  UpdateGuildRequest,
} from '../../api/garden-assistant-api';

@Injectable({ providedIn: 'root' })
export class GuildService {
  private readonly client = inject(GuildsClient);

  getAll(): Promise<GuildDto[]> {
    return this.client.getAll();
  }

  getById(id: string): Promise<GuildDto> {
    return this.client.getById(id);
  }

  create(request: CreateGuildRequest): Promise<GuildDto> {
    return this.client.create(request);
  }

  update(id: string, request: UpdateGuildRequest): Promise<GuildDto> {
    return this.client.update(id, request);
  }

  delete(id: string): Promise<void> {
    return this.client.delete(id);
  }
}
