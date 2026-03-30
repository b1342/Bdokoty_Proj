import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

/**
 * Main app shell layout.
 * Will contain the top navigation bar and footer once those screens are designed.
 * All feature routes are rendered inside the <router-outlet>.
 */
@Component({
  selector: 'app-main-layout',
  imports: [RouterOutlet],
  template: `
    <div class="min-h-screen bg-neutral-50 text-neutral-900">
      <!-- navbar will be added here -->
      <main>
        <router-outlet />
      </main>
      <!-- footer will be added here -->
    </div>
  `,
})
export class MainLayout {}
