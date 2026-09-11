import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import {
  Router,
  RouterLink,
  RouterLinkActive
} from '@angular/router';
import { Auth } from '../../core/services/auth';

interface NavigationItem {
  label: string;
  icon: string;
  route: string;
}

@Component({
  selector: 'app-navbar',
  imports: [
    CommonModule,
    RouterLink,
    RouterLinkActive
  ],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css'
})
export class Navbar {
  private readonly authService = inject(Auth);
  private readonly router = inject(Router);

  isMobileMenuOpen = false;

  readonly currentUser = this.authService.getCurrentUser();

  readonly isAdmin =
    this.currentUser?.role.toLowerCase() === 'admin';

  readonly navigationItems: NavigationItem[] =
    this.isAdmin
      ? [
        {
          label: 'Dashboard',
          icon: '⌂',
          route: '/dashboard'
        },
        {
          label: 'Student Management',
          icon: '♙',
          route: '/admin/students'
        },
        {
          label: 'Fee Management',
          icon: '₨',
          route: '/admin/fees'
        },
        {
          label: 'Notifications',
          icon: '♢',
          route: '/notifications'
        }
      ]
      : [
        {
          label: 'My Profile',
          icon: '♙',
          route: '/profile'
        },
        {
          label: 'Hostel',
          icon: '▦',
          route: '/hostels'
        },
        {
          label: 'Lab Reservations',
          icon: '⌘',
          route: '/labs'
        },
        {
          label: 'Events',
          icon: '◉',
          route: '/events'
        },
        {
          label: 'Complaints',
          icon: '!',
          route: '/complaints'
        },
        {
          label: 'Certificates',
          icon: '▤',
          route: '/certificates'
        },
        {
          label: 'Fees & Payments',
          icon: '₨',
          route: '/fees'
        },
        {
          label: 'Notifications',
          icon: '♢',
          route: '/notifications'
        }
      ];

  get initials(): string {
    const name = this.currentUser?.fullName;

    if (!name) {
      return 'ST';
    }

    return name
      .split(' ')
      .filter(part => part.length > 0)
      .slice(0, 2)
      .map(part => part.charAt(0).toUpperCase())
      .join('');
  }

  toggleMobileMenu(): void {
    this.isMobileMenuOpen = !this.isMobileMenuOpen;
  }

  closeMobileMenu(): void {
    this.isMobileMenuOpen = false;
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}