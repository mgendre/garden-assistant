import { Component, input, output } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { IconDefinition } from '@fortawesome/fontawesome-svg-core';

export interface ToggleOption {
  value: string;
  labelKey: string;
  icon?: IconDefinition;
}

@Component({
  selector: 'app-toggle-group',
  standalone: true,
  imports: [TranslateModule, FontAwesomeModule],
  templateUrl: './toggle-group.html',
})
export class ToggleGroup {
  readonly options = input.required<ToggleOption[]>();
  readonly selectedValue = input.required<string>();
  readonly valueChange = output<string>();
}
