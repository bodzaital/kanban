import { Component, OnInit, signal } from '@angular/core';
import { ColumnApi } from '../../../api/column.api';
import { ColumnDetailResponse } from '../../../transfers/columnTransfers';
import { Column } from "../../component/column/column";

@Component({
	selector: 'app-columns',
	imports: [Column],
	templateUrl: './columns.html',
	styleUrl: './columns.css'
})
export class Columns implements OnInit {
	protected columns = signal<ColumnDetailResponse[]>([]);

	constructor(private api: ColumnApi) { }

	public ngOnInit(): void {
		this.api.getColumnsOrdered().subscribe({
			next: (response) => this.columns.set(response.body!),
			error: (error) => this.handleError("getColumnsOrdered", error)
		});
	}

	private handleError(handler: string, error: any) {
		alert(`Error during ${handler}. Check console.`);
		console.error(handler, error);
	}
}
