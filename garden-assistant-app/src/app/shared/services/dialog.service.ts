import { Injectable, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { firstValueFrom } from 'rxjs';
import { ConfirmDialogComponent, ConfirmDialogData } from '../confirm-dialog/confirm-dialog';
import { BadgeInfoDialog, BadgeInfoDialogData } from '../ui/badge-info-dialog/badge-info-dialog';

@Injectable({ providedIn: 'root' })
export class DialogService {
  private readonly dialog = inject(MatDialog);

  async confirm(title: string, message: string, confirmLabel?: string, danger = false): Promise<boolean> {
    const result = await firstValueFrom(
      this.dialog.open<ConfirmDialogComponent, ConfirmDialogData, boolean>(ConfirmDialogComponent, {
        data: { title, message, confirmLabel, danger },
        maxWidth: '400px',
        width: '90vw',
      }).afterClosed()
    );
    return result === true;
  }

  openBadgeInfo(titleKey: string, descriptionKey: string, timesPerWeek?: number): void {
    this.dialog.open<BadgeInfoDialog, BadgeInfoDialogData>(BadgeInfoDialog, {
      data: { titleKey, descriptionKey, timesPerWeek },
      maxWidth: '400px',
    });
  }
}
