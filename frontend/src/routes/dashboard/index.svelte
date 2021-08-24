<script lang="ts">
	import type { ReportModel } from '$lib/ReportModel';
	import ReportTable from '$lib/ReportTable.svelte';
	import type { UserWebsiteModel } from '$lib/UserWebsiteModel';
	import WebsiteTable from '$lib/WebsiteTable.svelte';
	import { onMount } from 'svelte';

	let reports: ReportModel[] = [];
	let websites: UserWebsiteModel[] = [];
	let websiteUrl: string;

	onMount(async () => {
		await getWebsites();
	});

	interface MonitorWebsiteRequest {
		url: string;
	}

	const getWebsites = async () => {
		const response = await fetch('http://localhost:5000/user/sites', {
			method: 'GET',
			headers: {
				Authorization: `Bearer ${localStorage.getItem('token')}`
			}
		});

		if (response.ok) {
			var payload = await response.json();
			reports = payload.reports;
			websites = payload.websites;
		}
	};

	const monitorWebsite = async () => {
		const request: MonitorWebsiteRequest = { url: websiteUrl };
		const response = await fetch('http://localhost:5000/user/sites', {
			method: 'POST',
			headers: {
				'Content-Type': 'application/json',
				Authorization: `Bearer ${localStorage.getItem('token')}`
			},
			body: JSON.stringify(request)
		});

		if (response.ok) {
			var payload = await response.json();
			reports = payload.reports;
			websites = payload.websites;
		}
	};
</script>

<section class="bg-base-300 h-screen text-base-content">
	<div class="flex flex-col">
		<div class="flex m-auto pt-40 flex-col max-w-3xl w-full">
			<h2 class="flex flex-row mb-6 text-4xl mt-2 font-extrabold tracking-tight">
				<span class="text-center relative mt-3 inline-block px-2 py-2">
					<div class="absolute inset-0 transform -skew-x-13 -skew-y-3 bg-accent" />
					<span class="text-center uppercase tracking-widest relative">uptimey 🚀</span>
				</span>
			</h2>
			<div class="flex m-auto bg-base-200 rounded-lg w-full mt-10 p-4 flex-col">
				<div class="flex flex-row">
					<h2 class="font-bold tracking-tight text-2xl">
						{websites.length} website being monitored
					</h2>
					<div class="flex justify-end flex-1">
						<label for="add-modal" class="btn btn-primary modal-button btn-sm">Add</label>
						<input type="checkbox" id="add-modal" class="modal-toggle" />
						<div class="modal">
							<div class="modal-box">
								<p>Feel free to monitor any website you like!</p>
								<input
									bind:value={websiteUrl}
									placeholder="http://www.google.com/"
									class="mt-2 input w-full input-bordered"
								/>
								<div class="modal-action">
									<label for="add-modal" class="btn btn-outline">Close</label>
									<label on:click={monitorWebsite} for="add-modal" class="btn btn-primary"
										>Accept</label
									>
								</div>
							</div>
						</div>
					</div>
				</div>
				<WebsiteTable {websites} />
			</div>
			<div class="flex m-auto bg-base-200 rounded-lg w-full mt-10 p-4 flex-col">
				<div class="flex flex-row">
					<h2 class="font-bold tracking-tight text-2xl">Last reports</h2>
				</div>
				<ReportTable {reports} />
			</div>
		</div>
	</div>
</section>
