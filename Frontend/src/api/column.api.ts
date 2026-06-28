import { Injectable } from '@angular/core';
import environment from '../environment.json';
import { HttpClient } from '@angular/common/http';
import { ColumnCreateRequest, ColumnDetailResponse, ColumnUpdateRequest } from '../transfers/columnTransfers';
import { TicketCreateRequest, TicketSimpleResponse } from '../transfers/ticketTransfers';

@Injectable({ providedIn: 'root' })
export class ColumnApi {
	private readonly baseUrl = environment.backend.baseUrl;

	constructor(private http: HttpClient) { }

	public createColumn(body: ColumnCreateRequest) {
		return this.http.post<ColumnDetailResponse>(
			`${this.baseUrl}/api/column`,
			body,
			{ observe: "response" }
		);
	}

	public deleteColumn(id: string, newColumnId: string) {
		return this.http.delete<void>(
			`${this.baseUrl}/api/column/${id}?moveTo=${newColumnId}`,
			{ observe: "response" }
		);
	}

	public createTicketInColumn(id: string, body: TicketCreateRequest) {
		return this.http.post<TicketSimpleResponse>(
			`${this.baseUrl}/api/column/${id}/ticket`,
			body,
			{ observe: "response" }
		);
	}

	public updateColumn(id: string, body: ColumnUpdateRequest) {
		return this.http.patch<ColumnDetailResponse>(
			`${this.baseUrl}/api/column/${id}`,
			body,
			{ observe: "response" }
		);
	}

	public getColumnsOrdered() {
		return this.http.get<ColumnDetailResponse[]>(
			`${this.baseUrl}/api/column`,
			{ observe: "response" }
		);
	}

	public getColumn(id: string) {
		return this.http.get<ColumnDetailResponse>(
			`${this.baseUrl}/api/column/${id}`,
			{ observe: "response" }
		);
	}
}