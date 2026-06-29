import { Component, input, OnInit, signal } from '@angular/core';
import { TicketApi } from '../../../api/ticket.api';
import { TicketDetailResponse } from '../../../transfers/ticketTransfers';
import { RouterLink } from "@angular/router";

@Component({
	selector: 'app-column-ticket',
	imports: [RouterLink],
	templateUrl: './column-ticket.html',
	styleUrl: './column-ticket.css',
})
export class ColumnTicket implements OnInit {
	public id = input<string>();

	protected ticket = signal<TicketDetailResponse | undefined>(undefined);

	constructor(private api: TicketApi) { }

	public ngOnInit(): void {
		this.api.getTicket(this.id()!).subscribe({
			next: (response) => this.ticket.set(response.body!),
			error: (error) => console.error(error)
		});
	}
}
