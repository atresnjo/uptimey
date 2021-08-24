module.exports = {
	purge: {
		content: ['./src/**/*.{html,js,svelte,ts}'],
		options: {
			safelist: [/data-theme$/]
		}
	},
	theme: {
		extend: {
			colors: require('daisyui/colors')
		}
	},
	plugins: [require('daisyui'), require('@tailwindcss/typography')],
	daisyui: {
		styled: true,
		base: true,
		utils: true,
		themes: true,
		rtl: false
	}
};
