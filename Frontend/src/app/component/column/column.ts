import { Component, input } from '@angular/core';
import { ColumnDetailResponse } from '../../../transfers/columnTransfers';
import { ColumnTicket } from "../column-ticket/column-ticket";

@Component({
	selector: 'app-column',
	imports: [ColumnTicket],
	templateUrl: './column.html',
	styleUrl: './column.css',
})
export class Column {
	public data = input.required<ColumnDetailResponse>();
}
