import { Component, input, linkedSignal, output } from "@angular/core";

@Component({
  selector: 'app-error',
  template: `
    @if(errorMessage()) {
      <div class="px-4 py-3 mb-6 text-red-700 bg-red-100 rounded-lg border border-red-400">
        <div class="flex">
          <svg xmlns="http://www.w3.org/2000/svg" class="mr-2 w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          <span>{{ errorMessageToDisplay() }}</span>
        </div>
        <button
          (click)="clear()"
          class="mt-1 text-sm underline"
        >
          Dismiss
        </button>
      </div>
    }
  `,
})
export class ErrorComponent {
  errorMessage = input<string | null>(null);
  errorMessageToDisplay = linkedSignal(this.errorMessage);
  clearError = output();
  clear() {
    this.clearError.emit();
  }

}
