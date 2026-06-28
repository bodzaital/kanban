import { Routes } from '@angular/router';
import { Columns } from './page/columns/columns';
import { NotFound } from './page/not-found/not-found';
import { Ticket } from './page/ticket/ticket';

export const routes: Routes = [
	{ path: "", component: Columns },
	{ path: "ticket/:id", component: Ticket },
	{ path: "**", component: NotFound }
];
