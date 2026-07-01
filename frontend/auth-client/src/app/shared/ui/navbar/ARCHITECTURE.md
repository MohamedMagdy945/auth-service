# Navbar Component - Clean Architecture Implementation Guide

## 📋 Overview

A professional-grade navbar component built using **Clean Architecture** principles in Angular 22 with standalone components. The implementation demonstrates clear separation of concerns across three architectural layers.

## 🏗️ Architecture Layers

### 1. **Domain Layer** 
**Location:** `src/app/shared/ui/navbar/domain/navbar.model.ts`

Defines business entities and contracts independent of any framework.

```typescript
interface NavItem {
  id: string;
  label: string;
  route: string;
  icon?: string;
  children?: NavItem[];
  isActive?: boolean;
}

interface NavbarState {
  items: NavItem[];
  isOpen: boolean;
  activeItemId: string | null;
}
```

**Responsibilities:**
- ✓ Define data models
- ✓ No framework dependencies
- ✓ Pure business logic contracts
- ✓ Reusable across different UI frameworks

---

### 2. **Infrastructure Layer**
**Location:** `src/app/shared/ui/navbar/infrastructure/navbar.service.ts`

Implements state management and business logic using Angular services and signals.

**Key Features:**
- Reactive state management with Angular signals
- Computed properties for derived state
- Methods for state mutations
- No direct DOM manipulation

**Available Methods:**
```typescript
initializeNavbar(items: NavItem[]): void
toggleNavbar(): void
openNavbar(): void
closeNavbar(): void
setActiveItem(itemId: string): void
addNavItem(item: NavItem): void
removeNavItem(itemId: string): void
getState(): NavbarState
```

**Computed Signals:**
- `navItems` - Current navigation items
- `isNavbarOpen` - Navbar visibility state
- `activeItemId` - Currently active item ID

---

### 3. **Presentation Layer**
**Location:** `src/app/shared/ui/navbar/presentation/`

Handles UI rendering and user interactions.

**Components:**
- `navbar.component.ts` - Component logic
- `navbar.component.html` - Template
- `navbar.component.css` - Styling

**Features:**
- ✓ Responsive mobile hamburger menu
- ✓ Nested sub-menus support
- ✓ Active state indicators
- ✓ Event emission for parent communication
- ✓ Full TypeScript type safety

---

## 📁 Project Structure

```
src/app/shared/ui/navbar/
│
├── domain/
│   └── navbar.model.ts                 # Business entities
│
├── infrastructure/
│   ├── navbar.service.ts               # State management
│   └── navbar.service.spec.ts          # Service tests
│
├── presentation/
│   ├── navbar.component.ts             # UI component
│   ├── navbar.component.html           # Template
│   ├── navbar.component.css            # Styles
│   └── navbar.component.spec.ts        # Component tests
│
├── index.ts                            # Barrel export
└── README.md                           # Feature documentation
```

---

## 🚀 Getting Started

### Step 1: Import the Component
```typescript
import { NavbarComponent, NavItem, NavbarService } from './shared/ui/navbar';
```

### Step 2: Define Navigation Items
```typescript
navItems: NavItem[] = [
  {
    id: 'home',
    label: 'Home',
    route: '/home',
    icon: '🏠'
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
  }
];
```

### Step 3: Add to Template
```html
<app-navbar 
  [navItems]="navItems"
  [logoText]="'My App'"
  (itemSelected)="onItemSelected($event)"
  (navbarToggled)="onNavbarToggled($event)">
</app-navbar>

<router-outlet></router-outlet>
```

### Step 4: Handle Events
```typescript
onItemSelected(item: NavItem): void {
  console.log('Selected:', item.label);
  // Navigate or update state as needed
}

onNavbarToggled(isOpen: boolean): void {
  console.log('Navbar is:', isOpen ? 'open' : 'closed');
}
```

---

## 🎨 Styling & Customization

### CSS Variables Available

```css
--gray-900:      Dark background (#1a1a1a)
--gray-700:      Secondary background (#333)
--gray-400:      Text color (#e0e0e0)
--bright-blue:   Active link color (#4f94ff)
```

### Override in Your Styles

```css
:host {
  --gray-900: #000;
  --bright-blue: #0066ff;
}
```

### Responsive Breakpoints

```css
Mobile:  ≤ 768px    (Hamburger menu)
Tablet:  769-1024px (Optimized spacing)
Desktop: > 1024px   (Full navbar)
```

---

## 🔄 State Management Flow

```
User Interaction (Click)
        ↓
NavbarComponent (Presentation)
        ↓
NavbarService (Infrastructure)
        ↓
Signal Update (Domain Data)
        ↓
Computed Signals Reactively Update
        ↓
Component Template Re-renders
```

---

## ✅ Features

- [x] Clean Architecture Pattern
- [x] Standalone Angular Component (v22+)
- [x] Reactive State with Signals
- [x] TypeScript Type Safety
- [x] Responsive Design
- [x] Nested Sub-menus
- [x] Active State Tracking
- [x] Event Emission
- [x] Mobile Hamburger Menu
- [x] Comprehensive Tests
- [x] Barrel Exports
- [x] Documentation

---

## 🧪 Testing

Run tests using:
```bash
npm test
```

### Test Coverage

**NavbarService Tests** (`navbar.service.spec.ts`)
- ✓ Service creation and initialization
- ✓ Navbar toggle functionality
- ✓ Active item management
- ✓ Add/remove nav items
- ✓ Computed signals reactivity

**NavbarComponent Tests** (`navbar.component.spec.ts`)
- ✓ Component initialization
- ✓ User interaction handling
- ✓ Event emission
- ✓ Active state detection
- ✓ Mobile behavior

---

## 🔐 Best Practices Implemented

### 1. **Separation of Concerns**
- Domain layer is framework-agnostic
- Infrastructure handles state management
- Presentation focuses on UI and UX

### 2. **Dependency Injection**
- NavbarService injected into component
- Testable and mockable
- Single responsibility per class

### 3. **Reactive Programming**
- Angular signals for reactive state
- Computed properties for derived data
- No manual subscription management in component

### 4. **Type Safety**
- Full TypeScript interfaces
- Strong typing across all layers
- No `any` types

### 5. **Testability**
- Unit tests for service logic
- Unit tests for component behavior
- Mock-friendly architecture

---

## 🚀 Future Enhancements

- [ ] **Authentication Integration**: User dropdown menu
- [ ] **Search Feature**: Global search in navbar
- [ ] **Notifications**: Badge with notification count
- [ ] **Theme Switching**: Light/dark mode support
- [ ] **Keyboard Navigation**: Full accessibility support
- [ ] **Analytics**: Track navigation usage
- [ ] **Breadcrumbs**: Show current page path
- [ ] **Animations**: Smooth transitions and effects

---

## 📖 Usage Example in App Component

```typescript
@Component({
  selector: 'app-root',
  imports: [RouterOutlet, NavbarComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  navItems: NavItem[] = [
    { id: 'home', label: 'Home', route: '/home', icon: '🏠' },
    { id: 'about', label: 'About', route: '/about' },
    { id: 'contact', label: 'Contact', route: '/contact' }
  ];

  onNavItemSelected(item: NavItem): void {
    console.log('Navigated to:', item.label);
  }

  onNavbarToggled(isOpen: boolean): void {
    console.log('Navbar state:', isOpen);
  }
}
```

---

## 🎯 Clean Architecture Benefits

1. **Maintainability**: Clear structure makes code easier to understand and modify
2. **Testability**: Each layer can be tested independently
3. **Reusability**: Domain models can be used across different UI layers
4. **Flexibility**: Easy to swap implementations without changing interfaces
5. **Scalability**: Grows with your application without code decay
6. **Team Collaboration**: Clear boundaries make it easier for teams to work together

---

## 📞 Support

For issues or questions:
1. Check the test files for usage examples
2. Review inline documentation
3. See responsive CSS breakpoints for styling guidance
4. Refer to Angular signals documentation for advanced usage

---

**Last Updated:** 2026-07-01
**Angular Version:** 22.0.0
**TypeScript Version:** 6.0.2
