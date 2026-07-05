import { Component, computed, effect, input, OnDestroy, OnInit, signal, WritableSignal } from '@angular/core';
import { TicketApi } from '../../../api/ticket.api';
import { TicketDetailResponse } from '../../../transfers/ticketTransfers';
import { DirtyPage } from '../../service/dirtyPageGuard';
import { RouterLink } from "@angular/router";

@Component({
	selector: 'app-ticket',
	imports: [RouterLink],
	templateUrl: './ticket.html',
	styleUrl: './ticket.css'
})
export class Ticket implements OnInit, DirtyPage {
	public id = input<string>();
	public isDirty = signal(false);
	public onDirtyMessage: string = "There are unsaved changes. Are you sure you want to leave?";
	
	protected originalTicket = signal<TicketDetailResponse | undefined>(undefined);
	protected ticket = signal<TicketDetailResponse | undefined>(undefined);
	
	protected title = signal<string>("");
	protected description = signal<string>("");

	constructor(private ticketApi: TicketApi) { }

	public ngOnInit(): void {
		this.ticketApi.getTicket(this.id()!).subscribe({
			next: (response) => {
				this.ticket.set(response.body!);
				this.originalTicket.set(response.body!);
			},
			error: (error) => console.error(error)
		});
	}

	protected save() {
		this.originalTicket.set(this.ticket());
		this.ticketApi.updateTicket(this.ticket()!.id, {
			title: this.ticket()!.title,
			description: this.ticket()!.description
		}).subscribe({
			next: () => this.isDirty.set(false)
		});
	}

	protected disableNewLine(event: KeyboardEvent) {
		if (event.key == "Enter") {
			event.preventDefault();
		}
	}

	protected setTitle(event: KeyboardEvent) {
		const textarea = <HTMLTextAreaElement>event.target;
		this.isDirty.set(true);

		this.ticket.update((x) => <TicketDetailResponse>{
			...x,
			title: textarea.value
		});
	}

	protected setDescription(event: KeyboardEvent) {
		const textarea = <HTMLTextAreaElement>event.target;
		this.isDirty.set(true);

		this.ticket.update((x) => <TicketDetailResponse>{
			...x,
			description: textarea.value
		});
	}
}
