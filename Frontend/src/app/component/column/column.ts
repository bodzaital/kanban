import { Component, model, output } from '@angular/core';
import { ColumnDetailResponse } from '../../../transfers/columnTransfers';
import { ColumnTicket } from "../column-ticket/column-ticket";
import { CdkDragDrop, CdkDropList, CdkDrag } from '@angular/cdk/drag-drop';

@Component({
	selector: 'app-column',
	imports: [ColumnTicket, CdkDropList, CdkDrag],
	templateUrl: './column.html',
	styleUrl: './column.css',
})
export class Column {
	public column = model.required<ColumnDetailResponse>();
	public ticketMoved = output<CdkDragDrop<string, any, any>>();

	protected drop(e: any) {
		this.ticketMoved.emit(e);
	}
}
