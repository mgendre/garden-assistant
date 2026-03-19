import { Component, Input, Output, EventEmitter } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faPen, faTrash, faWandMagicSparkles, faEye, faPlus } from '@fortawesome/free-solid-svg-icons';
import { GuildDto } from '../../../api/garden-assistant-api';

@Component({
  selector: 'app-guild-card',
  standalone: true,
  imports: [TranslateModule, FontAwesomeModule],
  templateUrl: './guild-card.html',
  styleUrl: './guild-card.scss'
})
export class GuildCard {
  @Input({ required: true }) guild!: GuildDto;
  @Input() showDelete = false;
  @Input() showOpenButton = false;
  @Input() showPlantAdd = false;

  @Output() view = new EventEmitter<void>();
  @Output() edit = new EventEmitter<void>();
  @Output() delete = new EventEmitter<void>();
  @Output() plantClick = new EventEmitter<string>();
  @Output() plantAdd = new EventEmitter<string>();

  protected readonly faPen = faPen;
  protected readonly faTrash = faTrash;
  protected readonly faCustomize = faWandMagicSparkles;
  protected readonly faView = faEye;
  protected readonly faPlus = faPlus;

  onCardClick(): void {
    if (!this.showOpenButton) {
      this.view.emit();
    }
  }

  onEditClick(event: Event): void {
    event.stopPropagation();
    this.edit.emit();
  }

  onDeleteClick(event: Event): void {
    event.stopPropagation();
    this.delete.emit();
  }

  onPlantClick(plantId: string | undefined, event: Event): void {
    event.stopPropagation();
    if (plantId) {
      this.plantClick.emit(plantId);
    }
  }

  onPlantAddClick(plantId: string | undefined, event: Event): void {
    event.stopPropagation();
    if (plantId) {
      this.plantAdd.emit(plantId);
    }
  }
}
