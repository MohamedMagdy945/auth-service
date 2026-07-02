# Navbar Component - Clean Architecture Implementation

## Overview
A responsive navbar component built following clean architecture principles with clear separation of concerns.

## Architecture Layers

### 1. **Domain Layer** (`domain/navbar.model.ts`)
- Defines business entities and interfaces
- `NavItem`: Interface for navigation items with properties like id, label, route, icon, and children
- `NavbarState`: Interface for navbar state management
- Independent of any framework or infrastructure

### 2. **Infrastructure Layer** (`infrastructure/navbar.service.ts`)
- `NavbarService`: Manages navbar state and business logic
- Provides methods for:
  - Initializing navbar items
  - Toggle navbar open/close
  - Set active navigation item
  - Add/remove nav items
- Uses Angular signals for reactive state management

### 3. **Presentation Layer** (`presentation/navbar.component.ts`)
- `NavbarComponent`: UI component for rendering the navbar
- Handles user interactions (clicks, toggles)
- Emits events for parent components
- Responsive design with mobile hamburger menu
- Supports nested sub-menus

## Usage

### 1. Import the Component
```typescript
import { NavbarComponent } from './shared/ui/navbar';
```

### 2. Add to Your App Component
```typescript
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent, NavItem } from './shared/ui/navbar';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, NavbarComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  navItems: NavItem[] = [
    {
      id: 'home',
      label: 'Home',
      route: '/home',
      icon: '🏠'
    },
    {
      id: 'about',
      label: 'About',
      route: '/about',
      icon: 'ℹ️'
    },
    {
      id: 'services',
      label: 'Services',
      route: '/services',
      icon: '⚙️',
      children: [
        { id: 'service1', label: 'Service 1', route: '/services/1' },
        { id: 'service2', label: 'Service 2', route: '/services/2' }
      ]
    },
    {
      id: 'contact',
      label: 'Contact',
      route: '/contact',
      icon: '📧'
    }
  ];

  onItemSelected(item: NavItem): void {
    console.log('Selected:', item.label);
  }

  onNavbarToggled(isOpen: boolean): void {
    console.log('Navbar is:', isOpen ? 'open' : 'closed');
  }
}
```

### 3. Add to Template
```html
<app-navbar 
  [navItems]="navItems"
  [logoText]="'My App'"
  (itemSelected)="onItemSelected($event)"
  (navbarToggled)="onNavbarToggled($event)">
</app-navbar>

<router-outlet></router-outlet>
```

## Features

✅ **Clean Architecture**: Clear separation of domain, infrastructure, and presentation layers
✅ **Reactive State**: Uses Angular signals for reactive programming
✅ **Responsive Design**: Mobile-friendly with hamburger menu
✅ **Sub-menus**: Support for nested navigation items
✅ **Active State**: Automatic tracking of active navigation item
✅ **Standalone Component**: Works with Angular standalone components
✅ **Type-Safe**: Full TypeScript support with proper interfaces
✅ **Event Emission**: Parent component communication via outputs
✅ **Service Injection**: Centralized state management via NavbarService

## Responsive Breakpoints

- **Mobile** (≤ 768px): Hamburger menu, vertical layout
- **Tablet** (769px - 1024px): Optimized spacing
- **Desktop** (> 1024px): Full horizontal navbar

## CSS Variables Used

- `--gray-900`: Dark background
- `--gray-700`: Secondary background
- `--gray-400`: Text color
- `--bright-blue`: Active link color

You can customize these in your app's CSS.

## Future Enhancements

- [ ] Theme support (light/dark mode)
- [ ] User profile dropdown
- [ ] Search functionality
- [ ] Notification badge
- [ ] Accessibility improvements (ARIA labels, keyboard navigation)
- [ ] Animation transitions
