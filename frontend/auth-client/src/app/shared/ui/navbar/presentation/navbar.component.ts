/**
 * Presentation Layer - Navbar Component
 * Handles UI rendering and user interactions
 */

import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent {
  @Input() logoText: string = 'MyApp';
  @Output() itemSelected = new EventEmitter<{ id: string; label: string; route: string }>();
}

