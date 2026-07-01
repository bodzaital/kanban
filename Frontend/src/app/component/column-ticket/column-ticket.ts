import { Component, input } from '@angular/core';
import { TicketDetailResponse } from '../../../transfers/ticketTransfers';

@Component({
	selector: 'app-column-ticket',
	imports: [],
	templateUrl: './column-ticket.html',
	styleUrl: './column-ticket.css',
})
export class ColumnTicket {
	public ticket = input.required<TicketDetailResponse>();
}
