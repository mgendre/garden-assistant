import { Component, effect, inject, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableModule } from '@angular/material/table';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { firstValueFrom } from 'rxjs';
import { SwaggerException } from '../../api/garden-assistant-api';
import { ConfirmDialogComponent, ConfirmDialogData } from '../../shared/confirm-dialog/confirm-dialog';
import { CreateGardenDialogComponent, CreateGardenFormValue } from './create-garden-dialog/create-garden-dialog';
import { GardenService } from './garden.service';

@Component({
  selector: 'app-garden',
  standalone: true,
  providers: [GardenService],
  imports: [
    MatButtonModule,
    MatTableModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    TranslateModule
  ],
  templateUrl: './garden.html',
  styleUrl: './garden.scss'
})
export class GardenComponent implements OnInit {
  private readonly gardenService = inject(GardenService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);
  private readonly translate = inject(TranslateService);

  readonly gardens = this.gardenService.gardens;
  readonly loading = this.gardenService.loading;
  readonly displayedColumns = ['name', 'description', 'actions'];

  constructor() {
    effect(() => {
      const err = this.gardenService.error();
      if (err) this.snackBar.open(err, this.translate.instant('Snackbar.Close'), { duration: 4000 });
    });
  }

  ngOnInit(): void {
    this.gardenService.loadAll();
  }

  async openCreateDialog(): Promise<void> {
    const ref = this.dialog.open(CreateGardenDialogComponent, { width: '90vw', maxWidth: '480px' });
    const value: CreateGardenFormValue | undefined = await firstValueFrom(ref.afterClosed());
    if (!value) return;
    try {
      await this.gardenService.create({
        name: value.name,
        description: value.description || undefined
      });
      this.snackBar.open(
        this.translate.instant('Snackbar.GardenCreated'),
        this.translate.instant('Snackbar.Close'),
        { duration: 3000 }
      );
    } catch (err) {
      const message = err instanceof SwaggerException
        ? this.translate.instant('Snackbar.ErrorStatus', { status: err.status })
        : this.translate.instant('Snackbar.ErrorCreate');
      this.snackBar.open(message, this.translate.instant('Snackbar.Close'), { duration: 4000 });
    }
  }

  async deleteGarden(id: string, name: string): Promise<void> {
    const data: ConfirmDialogData = {
      title: this.translate.instant('DeleteGarden.Title'),
      message: this.translate.instant('DeleteGarden.Message', { name }),
      confirmLabel: this.translate.instant('DeleteGarden.Confirm'),
      cancelLabel: this.translate.instant('ConfirmDialog.Cancel')
    };
    const ref = this.dialog.open(ConfirmDialogComponent, { width: '90vw', maxWidth: '400px', data });
    const confirmed: boolean = await firstValueFrom(ref.afterClosed());
    if (!confirmed) return;
    try {
      await this.gardenService.remove(id);
      this.snackBar.open(
        this.translate.instant('Snackbar.GardenDeleted'),
        this.translate.instant('Snackbar.Close'),
        { duration: 3000 }
      );
    } catch (err) {
      const message = err instanceof SwaggerException
        ? this.translate.instant('Snackbar.ErrorStatus', { status: err.status })
        : this.translate.instant('Snackbar.ErrorDelete');
      this.snackBar.open(message, this.translate.instant('Snackbar.Close'), { duration: 4000 });
    }
  }
}
