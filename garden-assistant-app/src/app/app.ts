import { Component } from '@angular/core';
import { Shell } from './layout/shell/shell';

@Component({
  selector: 'app-root',
  imports: [Shell],
  templateUrl: './app.html',
  styles: [':host { display: block; min-height: 100vh; }']
})
export class App {}
