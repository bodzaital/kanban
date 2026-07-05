import { computed, inject, Injectable, Signal, signal, WritableSignal } from '@angular/core';
import { ActivatedRouteSnapshot, CanDeactivate, GuardResult, MaybeAsync, RouterStateSnapshot } from '@angular/router';

export interface DirtyPage {
	isDirty: WritableSignal<boolean>;
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
		if (!component.isDirty()) return true;

		return confirm(component.onDirtyMessage);
	}
}