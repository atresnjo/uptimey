<script lang="ts">
	import type { ReportModel } from './ReportModel';
	import dayjs from 'dayjs';
	export let reports: ReportModel[] = [];
</script>

<table class="mt-5 table w-full">
	<thead>
		<tr>
			<th>#</th>
			<th>Url</th>
			<th>Response time</th>
			<th>Last time checked</th>
			<th>Error</th>
		</tr>
	</thead>
	<tbody>
		{#each reports as report, index}
			<tr>
				<th>{index + 1}</th>
				<th>{report.url}</th>
				<th>{report.responseTime} ms</th>
				<th>{dayjs(report.dateChecked).format('DD/MM/YYYY')}</th>
				<th>{report.errorMessage === null ? 'No error! 😃' : report.errorMessage}</th>
			</tr>
		{/each}
	</tbody>
</table>
