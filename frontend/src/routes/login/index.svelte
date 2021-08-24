<script lang="ts">
	import { goto } from '$app/navigation';

	let email: string = '';
	let password: string = '';

	interface LoginRequest {
		email: string;
		password: string;
	}

	const login = async () => {
		const request: LoginRequest = { email, password };
		const response = await fetch('http://localhost:5000/login', {
			method: 'POST',
			headers: {
				'Content-Type': 'application/json'
			},
			body: JSON.stringify(request)
		});

		if (response.ok) {
			var payload = await response.json();
			localStorage.setItem('token', payload.accessToken);
			goto('/dashboard');
		}
	};
</script>

<section class="bg-base-300 text-base-content">
	<div class="flex h-screen">
		<div class="m-auto max-w-lg w-full">
			<div class="flex w-full flex-col ">
				<h1 class="text-center ">Login to your account</h1>
				<form on:submit|preventDefault={login}>
					<div class="flex flex-col py-1 space-y-3">
						<input
							bind:value={email}
							class="input input-bordered"
							type="text"
							id="email"
							autocomplete="email"
							placeholder="Email"
						/>
						<input
							bind:value={password}
							class="input input-bordered"
							type="text"
							id="password"
							autocomplete="password"
							placeholder="Password"
						/>
						<h1>Don't have an account? <a class="link-primary" href="/signup">Sign up</a></h1>
					</div>

					<button class="btn btn-primary btn-block">Login</button>
				</form>
			</div>
		</div>
	</div>
</section>
