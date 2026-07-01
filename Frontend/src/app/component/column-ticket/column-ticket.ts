import { Component, input, OnInit, signal } from '@angular/core';
import { TicketDetailResponse } from '../../../transfers/ticketTransfers';
import { CdkDrag } from '@angular/cdk/drag-drop';

@Component({
	selector: 'app-column-ticket',
	imports: [CdkDrag],
	templateUrl: './column-ticket.html',
	styleUrl: './column-ticket.css',
})
export class ColumnTicket {
	public ticket = input.required<TicketDetailResponse>();
}
