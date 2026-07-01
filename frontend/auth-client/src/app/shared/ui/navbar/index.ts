/**
 * Navbar Module Barrel Export
 * Exports all public APIs for the navbar feature
 */

// Domain exports
export * from './domain/navbar.model';

// Infrastructure exports
export { NavbarService } from './infrastructure/navbar.service';

// Presentation exports
export { NavbarComponent } from './presentation/navbar.component';
