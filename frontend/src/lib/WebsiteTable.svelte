<script lang="ts">
	import type { UserWebsiteModel } from './UserWebsiteModel';
	export let websites: UserWebsiteModel[] = [];

	const deleteWebsite = async (id: string) => {
		var authHeader = new Headers();
		authHeader.append('Authorization', `Bearer ${localStorage.getItem('token')}`);
		const response = await fetch(`http://localhost:5000/user/sites/${id}`, {
			method: 'DELETE',
			headers: authHeader
		});

		if (response.ok) {
		}
	};
</script>

<table class="mt-5 table w-full">
	<thead>
		<tr>
			<th>#</th>
			<th>Url</th>
			<th>Actions</th>
		</tr>
	</thead>
	<tbody>
		{#each websites as website, index}
			<tr>
				<th>{index + 1}</th>
				<th>{website.url}</th>
				<th>
					<label for="delete-modal" class="btn btn-error modal-button btn-sm">Delete</label>
					<input type="checkbox" id="delete-modal" class="modal-toggle" />
					<div class="modal">
						<div class="modal-box w-full">
							<p>Do you really want to stop monitoring this website?</p>

							<div class="modal-action">
								<label for="delete-modal" class="btn btn-outline">No</label>
								<label
									on:click={() => deleteWebsite(website.id)}
									for="delete-modal"
									class="btn btn-primary">Yes</label
								>
							</div>
						</div>
					</div>
				</th>
			</tr>
		{/each}
	</tbody>
</table>
