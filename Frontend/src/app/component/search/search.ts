import { Component } from '@angular/core';

@Component({
	selector: 'app-search',
	imports: [],
	templateUrl: './search.html',
	styleUrl: './search.css',
	host: {
		"(document:keydown)": "keyDown($event)",
	}
})
export class Search {
	public keyDown(event: KeyboardEvent) {
		if (event.key == "k" && this.isCtrlOrMetaKey(event)) {
			event.preventDefault();
			console.log("searching!");
		}
	}

	protected placeholder() {
		return this.getPlatform() == "apple"
			? "⌘K"
			: "CTRL+K"
	}

	protected getPlatform() {
		const applePlatforms = ["mac", "iphone", "ipod", "ipad"];
		const platform = navigator.platform.toLowerCase();

		const isApplePlatform = applePlatforms.filter((x) => platform.startsWith(x)).length > 0;

		return isApplePlatform
			? "apple"
			: "generic";
	}

	private isCtrlOrMetaKey(event: KeyboardEvent) {
		return this.getPlatform() == "apple"
			? event.metaKey
			: event.ctrlKey;
	}
}
