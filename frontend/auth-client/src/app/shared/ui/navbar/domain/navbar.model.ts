/**
 * Domain Model - Navbar
 * Defines the business entities and interfaces for the navbar feature
 */

export interface NavItem {
  id: string;
  label: string;
  route: string;
  icon?: string;
  children?: NavItem[];
  isActive?: boolean;
}

export interface NavbarState {
  items: NavItem[];
  isOpen: boolean;
  activeItemId: string | null;
}
