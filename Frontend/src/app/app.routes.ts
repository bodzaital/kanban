import { RouterConfigOptions, Routes } from '@angular/router';
import { Columns } from './page/columns/columns';
import { NotFound } from './page/not-found/not-found';
import { DirtyPageGuard } from './service/dirtyPageGuard';
import { Ticket } from './page/ticket/ticket';

export const routes: Routes = [
	{ path: "", component: Columns },
	{ path: "ticket/:id", component: Ticket, canDeactivate: [DirtyPageGuard] },
	{ path: "**", component: NotFound }
];

export const routerConfig: RouterConfigOptions = {
	canceledNavigationResolution: "computed"
};