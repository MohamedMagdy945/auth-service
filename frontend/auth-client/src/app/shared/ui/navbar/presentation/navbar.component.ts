import { Component, ElementRef, ViewChild, AfterViewInit, HostListener, Renderer2 } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-nav',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.component.html'
})
export class NavComponent implements AfterViewInit {
  logoText = 'Aperture';
  userName = 'Mira K.';
  isAuthOpen = false;
  isMobileMenuOpen = false;

  @ViewChild('primaryNav') primaryNav!: ElementRef;
  @ViewChild('slidingPill') slidingPill!: ElementRef;

  constructor(private renderer: Renderer2) {}

  ngAfterViewInit() {
    // تشغيل الأنيميشن المبدئي على العنصر النشط بعد تحميل الـ View
    setTimeout(() => {
      const activeEl = this.primaryNav.nativeElement.querySelector('.nav-item.active');
      if (activeEl) this.movePill(activeEl);
    }, 50);
  }

  // حساب أبعاد وحركة الـ Pill الخلفي
  movePill(element: HTMLElement) {
    const pill = this.slidingPill.nativeElement;
    const parent = this.primaryNav.nativeElement;
    
    const elRect = element.getBoundingClientRect();
    const parentRect = parent.getBoundingClientRect();

    this.renderer.setStyle(pill, 'width', `${elRect.width}px`);
    this.renderer.setStyle(pill, 'transform', `translateX(${elRect.left - parentRect.left - 4}px)`);
  }

  resetPill() {
    const activeEl = this.primaryNav.nativeElement.querySelector('.nav-item.active');
    if (activeEl) this.movePill(activeEl);
  }

  toggleAuthDropdown(event: Event) {
    event.preventDefault();
    event.stopPropagation();
    this.isAuthOpen = !this.isAuthOpen;
  }

  toggleMobileMenu() {
    this.isMobileMenuOpen = !this.isMobileMenuOpen;
  }

  // إغلاق الدروبر داون لو ضغط المستخدم في أي مكان خارجي
  @HostListener('document:click', ['$event'])
  closeDropdowns(event: Event) {
    this.isAuthOpen = false;
  }

  @HostListener('window:resize')
  onResize() {
    this.resetPill();
  }
}