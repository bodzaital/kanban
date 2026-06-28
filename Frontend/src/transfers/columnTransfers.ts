export interface ColumnCreateRequest {
	name: string
}

export interface ColumnUpdateRequest {
	name?: string,
	position?: number
}

export interface ColumnDetailResponse {
	id: string,
	name: string,
	position: number,
	tickets: string[]
}