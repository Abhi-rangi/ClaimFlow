import { Component } from '@angular/core';
import { AuthService } from '@auth0/auth0-angular';
import { Observable, map } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'Claims Management Platform';
  
  isAuthenticated$: Observable<boolean>;
  user$: Observable<any>;
  userRoles$: Observable<string[]>;

  constructor(private authService: AuthService) {
    this.isAuthenticated$ = this.authService.isAuthenticated$;
    this.user$ = this.authService.user$;
    
    // TODO: TBD - Extract roles from user claims once Auth0 Action is configured
    // Currently roles are not available in the user object
    // For now, show empty roles
    this.userRoles$ = this.user$.pipe(
      map(user => {
        if (!user) return [];
        // Check both possible claim locations
        const roles = user['https://schemas.quickstarts.dev/roles'] || user['roles'] || [];
        return Array.isArray(roles) ? roles : [roles];
      })
    );
  }

  login(): void {
    this.authService.loginWithRedirect({
      appState: { target: '/' }
    });
  }

  logout(): void {
    this.authService.logout({
      logoutParams: {
        returnTo: window.location.origin
      }
    });
  }

  hasRole(role: string): Observable<boolean> {
    return this.userRoles$.pipe(
      map(roles => roles.includes(role))
    );
  }
}

