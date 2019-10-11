import { Component } from '@angular/core';
import state from './state';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent {
  title = 'Shuttle Books (angular)';
  state = state;
}
