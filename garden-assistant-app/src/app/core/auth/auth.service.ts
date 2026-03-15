import { Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';

interface TokenResponse {
  accessToken: string;
  refreshToken: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  readonly accessToken = signal<string | null>(null);
  private refreshToken: string | null = null;

  async initialize(): Promise<void> {
    const response = await fetch(`${environment.apiBaseUrl}/api/auth/token`);
    if (!response.ok) {
      return;
    }
    const tokens: TokenResponse = await response.json();
    this.accessToken.set(tokens.accessToken);
    this.refreshToken = tokens.refreshToken;
  }

  async refresh(): Promise<void> {
    if (!this.refreshToken) {
      this.clearTokens();
      return;
    }
    try {
      const response = await fetch(`${environment.apiBaseUrl}/api/auth/refresh`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ refreshToken: this.refreshToken })
      });
      if (!response.ok) {
        this.clearTokens();
        return;
      }
      const tokens: TokenResponse = await response.json();
      this.accessToken.set(tokens.accessToken);
      this.refreshToken = tokens.refreshToken;
    } catch {
      this.clearTokens();
    }
  }

  getAccessToken(): string | null {
    return this.accessToken();
  }

  createAuthFetch(): { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> } {
    const authService = this;
    return {
      async fetch(url: RequestInfo, init?: RequestInit): Promise<Response> {
        const token = authService.getAccessToken();
        const isAuthUrl = typeof url === 'string' && url.includes('/api/auth/');

        const headers = new Headers(init?.headers);
        if (token && !isAuthUrl) {
          headers.set('Authorization', `Bearer ${token}`);
        }

        const firstResponse = await window.fetch(url, { ...init, headers });

        if (firstResponse.status === 401 && !isAuthUrl) {
          await authService.refresh();
          const newToken = authService.getAccessToken();
          if (newToken) {
            headers.set('Authorization', `Bearer ${newToken}`);
            return window.fetch(url, { ...init, headers });
          }
        }

        return firstResponse;
      }
    };
  }

  private clearTokens(): void {
    this.accessToken.set(null);
    this.refreshToken = null;
  }
}
