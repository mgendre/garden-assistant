import { Component, inject, signal } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';

export interface CreateGardenDialogData {
  mode: 'create' | 'edit';
  name?: string;
  description?: string;
}

export interface CreateGardenDialogResult {
  name: string;
  description?: string;
}

@Component({
  selector: 'app-create-garden-dialog',
  standalone: true,
  imports: [TranslateModule],
  templateUrl: './create-garden-dialog.html',
})
export class CreateGardenDialog {
  readonly data = inject<CreateGardenDialogData>(MAT_DIALOG_DATA);
  private readonly dialogRef = inject(MatDialogRef<CreateGardenDialog>);

  readonly name = signal(this.data.name ?? '');
  readonly description = signal(this.data.description ?? '');

  get isValid(): boolean {
    return this.name().trim().length > 0;
  }

  save(): void {
    if (!this.isValid) {
      return;
    }
    this.dialogRef.close({
      name: this.name().trim(),
      description: this.description().trim() || undefined,
    } as CreateGardenDialogResult);
  }

  cancel(): void {
    this.dialogRef.close(undefined);
  }
}
