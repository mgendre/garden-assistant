import { Component, effect, inject, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableModule } from '@angular/material/table';
import { MatToolbarModule } from '@angular/material/toolbar';
import { firstValueFrom } from 'rxjs';
import { SwaggerException } from '../../api/garden-assistant-api';
import { ConfirmDialogComponent, ConfirmDialogData } from '../../shared/confirm-dialog/confirm-dialog';
import { CreateGardenDialogComponent, CreateGardenFormValue } from './create-garden-dialog/create-garden-dialog';
import { GardenService } from './garden.service';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-garden',
  standalone: true,
  providers: [GardenService],
  imports: [
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatProgressSpinnerModule,
    MatDialogModule
  ],
  templateUrl: './garden.html',
  styleUrl: './garden.scss'
})
export class GardenComponent implements OnInit {
  private readonly gardenService = inject(GardenService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);

  readonly gardens = this.gardenService.gardens;
  readonly loading = this.gardenService.loading;
  readonly displayedColumns = ['name', 'description', 'actions'];

  constructor() {
    effect(() => {
      const err = this.gardenService.error();
      if (err) this.snackBar.open(err, 'Close', { duration: 4000 });
    });
  }

  ngOnInit(): void {
    this.gardenService.loadAll();
  }

  async openCreateDialog(): Promise<void> {
    const ref = this.dialog.open(CreateGardenDialogComponent, { width: '480px' });
    const value: CreateGardenFormValue | undefined = await firstValueFrom(ref.afterClosed());
    if (!value) return;
    try {
      await this.gardenService.create({
        name: value.name,
        description: value.description || undefined
      });
      this.snackBar.open('Garden created', 'Close', { duration: 3000 });
    } catch (err) {
      const message = err instanceof SwaggerException ? `Error ${err.status}` : 'Failed to create garden';
      this.snackBar.open(message, 'Close', { duration: 4000 });
    }
  }

  async deleteGarden(id: string, name: string): Promise<void> {
    const data: ConfirmDialogData = {
      title: 'Delete garden',
      message: `Delete "${name}"? This cannot be undone.`,
      confirmLabel: 'Delete'
    };
    const ref = this.dialog.open(ConfirmDialogComponent, { width: '400px', data });
    const confirmed: boolean = await firstValueFrom(ref.afterClosed());
    if (!confirmed) return;
    try {
      await this.gardenService.remove(id);
      this.snackBar.open('Garden deleted', 'Close', { duration: 3000 });
    } catch (err) {
      const message = err instanceof SwaggerException ? `Error ${err.status}` : 'Failed to delete garden';
      this.snackBar.open(message, 'Close', { duration: 4000 });
    }
  }
}
