import { Component, input } from '@angular/core';
import { TicketDetailResponse } from '../../../transfers/ticketTransfers';
import { RouterLink } from "@angular/router";

@Component({
	selector: 'app-column-ticket',
	imports: [RouterLink],
	templateUrl: './column-ticket.html',
	styleUrl: './column-ticket.css',
})
export class ColumnTicket {
	public ticket = input.required<TicketDetailResponse>();

	protected link() {
		return `ticket/${this.ticket().id}`;
	}

	protected hasDescription() {
		return this.ticket().description.length > 0;
	}
}
