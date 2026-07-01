import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './shared/ui/navbar';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, NavbarComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('auth-client');

  onNavItemSelected(): void {
    console.log('Navigation item selected');
  }

  onNavbarToggled(): void {
    console.log('Navbar toggled');
  }
}
