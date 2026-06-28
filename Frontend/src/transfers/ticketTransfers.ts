export interface TicketCreateRequest {
	title: string,
	description?: string
}

export interface TicketUpdateRequest {
	position?: number,
	title?: string,
	description?: string,
	columnId?: string
}

export interface TicketStructureRequest {
	id: string
}

export interface TicketSimpleResponse {
	id: string,
	position: number,
	title: string,
	number: string
}

export interface TicketDetailResponse {
	id: string,
	number: string,
	position: number,
	title: string,
	description: string,
	columnId: string
}