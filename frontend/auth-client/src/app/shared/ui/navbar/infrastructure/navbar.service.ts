/**
 * Infrastructure Layer - Navbar Service
 * Manages navbar state and business logic
 */

import { Injectable, signal, computed } from '@angular/core';
import { NavItem, NavbarState } from '../domain/navbar.model';

@Injectable({
  providedIn: 'root'
})
export class NavbarService {
  private readonly navbarState = signal<NavbarState>({
    items: [],
    isOpen: false,
    activeItemId: null
  });

  // Computed signals for derived state
  readonly navItems = computed(() => this.navbarState().items);
  readonly isNavbarOpen = computed(() => this.navbarState().isOpen);
  readonly activeItemId = computed(() => this.navbarState().activeItemId);

  /**
   * Initialize navbar with items
   */
  initializeNavbar(items: NavItem[]): void {
    this.navbarState.update(state => ({
      ...state,
      items: items.map(item => ({ ...item, isActive: false }))
    }));
  }

  /**
   * Toggle navbar open/closed state
   */
  toggleNavbar(): void {
    this.navbarState.update(state => ({
      ...state,
      isOpen: !state.isOpen
    }));
  }

  /**
   * Open navbar
   */
  openNavbar(): void {
    this.navbarState.update(state => ({
      ...state,
      isOpen: true
    }));
  }

  /**
   * Close navbar
   */
  closeNavbar(): void {
    this.navbarState.update(state => ({
      ...state,
      isOpen: false
    }));
  }

  /**
   * Set active navigation item
   */
  setActiveItem(itemId: string): void {
    this.navbarState.update(state => ({
      ...state,
      activeItemId: itemId,
      items: state.items.map(item => ({
        ...item,
        isActive: item.id === itemId
      }))
    }));
  }

  /**
   * Add nav item
   */
  addNavItem(item: NavItem): void {
    this.navbarState.update(state => ({
      ...state,
      items: [...state.items, { ...item, isActive: false }]
    }));
  }

  /**
   * Remove nav item by id
   */
  removeNavItem(itemId: string): void {
    this.navbarState.update(state => ({
      ...state,
      items: state.items.filter(item => item.id !== itemId)
    }));
  }

  /**
   * Get current navbar state
   */
  getState(): NavbarState {
    return this.navbarState();
  }
}
