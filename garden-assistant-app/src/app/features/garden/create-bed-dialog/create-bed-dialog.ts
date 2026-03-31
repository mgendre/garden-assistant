import { Component, inject, signal } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { TranslateModule } from '@ngx-translate/core';

export interface CreateBedDialogData {
  mode: 'create' | 'edit';
  name?: string;
}

export interface CreateBedDialogResult {
  name?: string;
}

@Component({
  selector: 'app-create-bed-dialog',
  standalone: true,
  imports: [MatDialogModule, MatButtonModule, TranslateModule],
  templateUrl: './create-bed-dialog.html',
})
export class CreateBedDialog {
  readonly data = inject<CreateBedDialogData>(MAT_DIALOG_DATA);
  private readonly dialogRef = inject(MatDialogRef<CreateBedDialog>);

  readonly name = signal(this.data.name ?? '');

  save(): void {
    this.dialogRef.close({
      name: this.name().trim() || undefined,
    } as CreateBedDialogResult);
  }

  cancel(): void {
    this.dialogRef.close(undefined);
  }
}
