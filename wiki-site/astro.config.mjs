// @ts-check
import { defineConfig } from 'astro/config';
import starlight from '@astrojs/starlight';

// Sidebar mirrors .wiki/_Sidebar.md (import script skips that file).
const luaTrack = [
	{ label: 'Lua 01 · First Mod', slug: 'guidebook-lua-01-first-mod' },
	{ label: 'Lua 02 · Events', slug: 'guidebook-lua-02-events' },
	{ label: 'Lua 03 · Timers + Storage', slug: 'guidebook-lua-03-timers-storage' },
	{ label: 'Lua 04 · UI', slug: 'guidebook-lua-04-ui' },
	{ label: 'Lua 05 · Systems', slug: 'guidebook-lua-05-systems' },
	{ label: 'Lua · Complete Project', slug: 'guidebook-lua-complete-project' },
];
const csharpTrack = [
	{ label: 'C# 01 · Setup', slug: 'guidebook-csharp-01-setup' },
	{ label: 'C# 02 · Lifecycle', slug: 'guidebook-csharp-02-lifecycle' },
	{ label: 'C# 03 · UI', slug: 'guidebook-csharp-03-ui' },
	{ label: 'C# 04 · Patches', slug: 'guidebook-csharp-04-patches' },
	{ label: 'C# 05 · Saves + Shop', slug: 'guidebook-csharp-05-saves-shop' },
	{ label: 'C# · Complete Project', slug: 'guidebook-csharp-complete-project' },
	{ label: 'C# 06 · Deploy + Debug', slug: 'guidebook-csharp-06-deploy-debug' },
];
const jsTrack = [
	{ label: 'JS 01 · Setup', slug: 'guidebook-js-01-setup' },
	{ label: 'JS 02 · Project', slug: 'guidebook-js-02-project' },
];
const modellingTrack = [
	{ label: 'Modelling Overview', slug: 'guidebook-modelling-overview' },
	{ label: 'OBJ + Blender', slug: 'guidebook-modelling-obj-blender' },
	{ label: 'Shop Item', slug: 'guidebook-modelling-shop-item' },
	{ label: 'Static Item', slug: 'guidebook-modelling-static-item' },
	{ label: 'Troubleshooting', slug: 'guidebook-modelling-troubleshooting' },
];

// https://astro.build/config
export default defineConfig({
	integrations: [
		starlight({
			title: 'gregCore Wiki',
			description: 'Guidebook, Core reference, Hooks and Glossary for the gregCore Data Center mod framework.',
			social: [{ icon: 'github', label: 'GitHub', href: 'https://github.com/mleem97/gregCore' }],
			sidebar: [
				{ label: 'Home', slug: 'index' },
				{
					label: 'Guidebook',
					items: [
						{ label: 'Guidebook', slug: 'guidebook' },
						{ label: 'Prerequisites', slug: 'guidebook-prerequisites' },
						{ label: 'Lua track', items: luaTrack },
						{ label: 'C# track', items: csharpTrack },
						{ label: 'JS track', items: jsTrack },
						{ label: 'Modelling track', items: modellingTrack },
						{ label: 'Debugging', slug: 'guidebook-debugging' },
						{ label: 'Computer UI', slug: 'guidebook-computer-ui' },
						{ label: 'Porting Matrix', slug: 'guidebook-porting-matrix' },
						{ label: 'Release', slug: 'guidebook-release' },
						{ label: 'Next Languages', slug: 'guidebook-next-languages' },
					],
				},
				{
					label: 'Players',
					items: [
						{ label: 'Getting Started', slug: 'player-getting-started' },
						{ label: 'Installation', slug: 'player-installation' },
						{ label: 'Mod Users Guide', slug: 'player-mod-users-guide' },
					],
				},
				{
					label: 'Core',
					items: [
						{ label: 'Overview', slug: 'core-overview' },
						{ label: 'Events', slug: 'core-events' },
						{ label: 'Save Engine', slug: 'core-save-engine' },
					],
				},
				{
					label: 'Developers',
					items: [
						{ label: 'Getting Started', slug: 'developer-getting-started' },
						{ label: 'Environment', slug: 'developer-environment' },
						{ label: 'First Lua Mod', slug: 'developer-first-lua-mod' },
						{ label: 'First C# Mod', slug: 'developer-first-csharp-mod' },
						{ label: 'Timers + Coroutines', slug: 'developer-timers-coroutines' },
						{ label: 'Events + Hooks', slug: 'developer-events-hooks-guide' },
						{ label: 'Data Storage', slug: 'developer-data-storage' },
						{ label: 'UI Panels + HUD', slug: 'developer-ui-panels-hud' },
						{ label: 'Shop Items', slug: 'developer-shop-items' },
						{ label: 'Hardware IDs + Inventory', slug: 'developer-hardware-ids-inventory' },
						{ label: 'Harmony + IL2CPP', slug: 'developer-harmony-il2cpp' },
						{ label: 'Scripting Bridges', slug: 'developer-scripting-bridges' },
						{ label: 'Native Co-op', slug: 'developer-native-coop' },
						{ label: 'Best Practices', slug: 'developer-best-practices' },
						{ label: 'Publishing', slug: 'developer-publishing' },
					],
				},
				{
					label: 'Reference',
					items: [
						{ label: 'Hooks Reference', slug: 'hooks-reference' },
						{ label: 'Glossary', slug: 'glossary' },
						{ label: 'FAQ + Troubleshooting', slug: 'faq-troubleshooting' },
					],
				},
			],
		}),
	],
});
