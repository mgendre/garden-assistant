import { Component, inject, signal } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';
import { SoilType } from '../../../api/garden-assistant-api';

export interface CreateBedDialogData {
  mode: 'create' | 'edit';
  name?: string;
  soilType?: string;
}

export interface CreateBedDialogResult {
  name?: string;
  soilType?: string;
}

@Component({
  selector: 'app-create-bed-dialog',
  standalone: true,
  imports: [TranslateModule],
  templateUrl: './create-bed-dialog.html',
})
export class CreateBedDialog {
  readonly data = inject<CreateBedDialogData>(MAT_DIALOG_DATA);
  private readonly dialogRef = inject(MatDialogRef<CreateBedDialog>);

  readonly name = signal(this.data.name ?? '');
  readonly soilType = signal(this.data.soilType ?? '');

  readonly soilTypeOptions = [
    { value: '', labelKey: 'Bed.SoilType.None' },
    { value: SoilType.Sandy, labelKey: 'Bed.SoilType.Sandy' },
    { value: SoilType.Loam, labelKey: 'Bed.SoilType.Loam' },
    { value: SoilType.Clay, labelKey: 'Bed.SoilType.Clay' },
    { value: SoilType.Silty, labelKey: 'Bed.SoilType.Silty' },
    { value: SoilType.Chalky, labelKey: 'Bed.SoilType.Chalky' },
    { value: SoilType.Peaty, labelKey: 'Bed.SoilType.Peaty' },
    { value: SoilType.Rocky, labelKey: 'Bed.SoilType.Rocky' },
  ];

  save(): void {
    this.dialogRef.close({
      name: this.name().trim() || undefined,
      soilType: this.soilType() || undefined,
    } as CreateBedDialogResult);
  }

  cancel(): void {
    this.dialogRef.close(undefined);
  }
}
