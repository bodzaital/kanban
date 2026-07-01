import { Component, OnInit, signal } from '@angular/core';
import { ColumnApi } from '../../../api/column.api';
import { ColumnDetailResponse } from '../../../transfers/columnTransfers';
import { Column } from "../../component/column/column";
import { CdkDragDrop, CdkDropListGroup, moveItemInArray, transferArrayItem } from "@angular/cdk/drag-drop";
import { TicketApi } from '../../../api/ticket.api';

@Component({
	selector: 'app-columns',
	imports: [Column, CdkDropListGroup],
	templateUrl: './columns.html',
	styleUrl: './columns.css'
})
export class Columns implements OnInit {
	protected columns = signal<ColumnDetailResponse[]>([]);

	constructor(private columnApi: ColumnApi, private ticketApi: TicketApi) { }

	public ngOnInit(): void {
		this.columnApi.getColumnsOrdered().subscribe({
			next: (response) => this.columns.set(response.body!),
			error: (error) => console.error(error)
		});
	}
	
	protected dropped(event: CdkDragDrop<any, any, any>) {
		if (event.previousContainer === event.container) {
			const tickets = [...this.columns().find((x) => x.id == event.container.data)!.tickets];

			moveItemInArray(tickets, event.previousIndex, event.currentIndex);

			this.columns.update((columns) => {
				columns.find((x) => x.id == event.container.data)!.tickets = tickets;
				return [...columns]
			});

			this.ticketApi.updateTicket(tickets[event.currentIndex].id, {
				position: event.currentIndex
			}).subscribe();
		} else {
			const oldTickets = [...this.columns().find((x) => x.id == event.previousContainer.data)!.tickets];
			const newTickets = [...this.columns().find((x) => x.id == event.container.data)!.tickets];

			transferArrayItem(oldTickets, newTickets, event.previousIndex, event.currentIndex);

			this.columns.update((columns) => {
				columns.find((x) => x.id == event.previousContainer.data)!.tickets = oldTickets;
				return [...columns]
			});

			this.columns.update((columns) => {
				columns.find((x) => x.id == event.container.data)!.tickets = newTickets;
				return [...columns]
			});

			this.ticketApi.updateTicket(newTickets[event.currentIndex].id, {
				columnId: event.container.data,
				position: event.currentIndex
			}).subscribe();
		}
	}
}
