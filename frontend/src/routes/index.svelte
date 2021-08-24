<script context="module" lang="ts">
	export const prerender = true;
</script>

<script lang="ts">
</script>

<svelte:head>
	<title>Home</title>
</svelte:head>

<section class="bg-base-300 text-base-content">
	<div class="flex h-screen">
		<div class="m-auto">
			<h2 class="mb-6 text-3xl mt-2 font-extrabold tracking-tight sm:text-4xl sm:leading-none">
				Hey stranger. Welcome to
				<span class="relative mt-2  inline-block px-2 py-2">
					<div class="absolute inset-0 transform -skew-x-13 -skew-y-3 bg-accent" />
					<span class="relative">uptimey 🚀</span>
				</span>
			</h2>
			<h2 class="text-2xl mt-6 text-center">
				Would you like to <a class="link-primary" href="/login">login</a> or
				<a class="link-primary" href="/signup">create an account</a> ?
			</h2>
		</div>
	</div>
</section>
