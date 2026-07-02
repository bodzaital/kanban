import { computed, inject, Injectable, signal } from '@angular/core';
import { ActivatedRouteSnapshot, CanDeactivate, GuardResult, MaybeAsync, RouterStateSnapshot } from '@angular/router';

export interface DirtyPage {
	isDirty: boolean;
	onDirtyMessage: string;
}

@Injectable({ providedIn: 'root' })
export class DirtyPageGuard<T> implements CanDeactivate<DirtyPage> {
	canDeactivate(
		component: DirtyPage,
		currentRoute: ActivatedRouteSnapshot,
		currentState: RouterStateSnapshot,
		nextState: RouterStateSnapshot
	): MaybeAsync<GuardResult> {
		if (!component.isDirty) return true;

		return confirm(component.onDirtyMessage);
	}
}