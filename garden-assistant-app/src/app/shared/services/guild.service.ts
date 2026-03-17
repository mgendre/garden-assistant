import { Injectable, inject } from '@angular/core';
import {
  GuildsClient,
  GuildSummaryDto,
  GuildDetailDto,
  CreateGuildRequest,
  UpdateGuildRequest,
} from '../../api/garden-assistant-api';

@Injectable({ providedIn: 'root' })
export class GuildService {
  private readonly client = inject(GuildsClient);

  getAll(): Promise<GuildSummaryDto[]> {
    return this.client.getAll();
  }

  getById(id: string): Promise<GuildDetailDto> {
    return this.client.getById(id);
  }

  create(request: CreateGuildRequest): Promise<GuildDetailDto> {
    return this.client.create(request);
  }

  update(id: string, request: UpdateGuildRequest): Promise<GuildDetailDto> {
    return this.client.update(id, request);
  }

  delete(id: string): Promise<void> {
    return this.client.delete(id);
  }
}
