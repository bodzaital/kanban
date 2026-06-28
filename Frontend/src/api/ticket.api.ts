import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import environment from '../environment.json';
import { TicketDetailResponse, TicketUpdateRequest } from '../transfers/ticketTransfers';

@Injectable({ providedIn: 'root' })
export class TicketApi {
	private readonly baseUrl = environment.backend.baseUrl;

	constructor(private http: HttpClient) { }

	public deleteTicket(id: string) {
		return this.http.delete<void>(
			`${this.baseUrl}/api/ticket/${id}`,
			{ observe: "response" }
		);
	}

	public updateTicket(id: string, body: TicketUpdateRequest) {
		return this.http.patch<TicketDetailResponse>(
			`${this.baseUrl}/api/ticket/${id}`,
			body,
			{ observe: "response" }
		);
	}

	public getTicket(id: string) {
		return this.http.get<TicketDetailResponse>(
			`${this.baseUrl}/api/ticket/${id}`,
			{ observe: "response" }
		);
	}
}