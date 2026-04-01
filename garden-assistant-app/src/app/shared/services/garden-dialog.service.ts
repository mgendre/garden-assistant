import { Injectable, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { firstValueFrom } from 'rxjs';
import { CreateGardenDialog, CreateGardenDialogData, CreateGardenDialogResult } from '../../features/garden/create-garden-dialog/create-garden-dialog';
import { CreateBedDialog, CreateBedDialogData, CreateBedDialogResult } from '../../features/garden/create-bed-dialog/create-bed-dialog';

@Injectable({ providedIn: 'root' })
export class GardenDialogService {
  private readonly dialog = inject(MatDialog);

  async openCreateGarden(data?: { mode: 'create' | 'edit'; name?: string; description?: string }): Promise<CreateGardenDialogResult | undefined> {
    const dialogData: CreateGardenDialogData = data ?? { mode: 'create' };
    const result = await firstValueFrom(
      this.dialog.open<CreateGardenDialog, CreateGardenDialogData, CreateGardenDialogResult>(CreateGardenDialog, {
        data: dialogData,
        maxWidth: '500px',
        width: '90vw',
      }).afterClosed()
    );
    return result ?? undefined;
  }

  async openCreateBed(data?: { mode: 'create' | 'edit'; name?: string }): Promise<CreateBedDialogResult | undefined> {
    const dialogData: CreateBedDialogData = data ?? { mode: 'create' };
    const result = await firstValueFrom(
      this.dialog.open<CreateBedDialog, CreateBedDialogData, CreateBedDialogResult>(CreateBedDialog, {
        data: dialogData,
        maxWidth: '500px',
        width: '90vw',
      }).afterClosed()
    );
    return result ?? undefined;
  }
}
