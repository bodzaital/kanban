import { Component, input } from '@angular/core';

@Component({
	selector: 'app-ticket',
	imports: [],
	templateUrl: './ticket.html',
	styleUrl: './ticket.css',
})
export class Ticket {
	public id = input.required<string>();
}
