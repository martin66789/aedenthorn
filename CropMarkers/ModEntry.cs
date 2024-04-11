﻿using System;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;

namespace CropMarkers
{
	/// <summary>The mod entry point.</summary>
	public partial class ModEntry : Mod
	{
		internal static IMonitor SMonitor;
		internal static IModHelper SHelper;
		internal static ModConfig Config;

		internal static ModEntry context;

		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			Config = Helper.ReadConfig<ModConfig>();

			context = this;

			SMonitor = Monitor;
			SHelper = helper;

			Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;

			// Load Harmony patches
			try
			{
				Harmony harmony = new(ModManifest.UniqueID);

				harmony.Patch(
					original: AccessTools.Method(typeof(Crop), nameof(Crop.draw)),
					postfix: new HarmonyMethod(typeof(Crop_draw_Patch), nameof(Crop_draw_Patch.Postfix))
				);
			}
			catch (Exception e)
			{
				Monitor.Log($"Issue with Harmony patching: {e}", LogLevel.Error);
				return;
			}
		}

		private void GameLoop_GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
		{
			// get Generic Mod Config Menu's API (if it's installed)
			var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
			if (configMenu is null)
				return;

			// register mod
			configMenu.Register(
				mod: ModManifest,
				reset: () => Config = new ModConfig(),
				save: () => Helper.WriteConfig(Config)
			);

			configMenu.AddBoolOption(
				mod: ModManifest,
				name: () => SHelper.Translation.Get("GMCM.ModEnabled.Name"),
				getValue: () => Config.ModEnabled,
				setValue: value => Config.ModEnabled = value
			);
			configMenu.AddBoolOption(
				mod: ModManifest,
				name: () => SHelper.Translation.Get("GMCM.RequireKeyPress.Name"),
				getValue: () => Config.RequireKeyPress,
				setValue: value => Config.RequireKeyPress = value
			);
			configMenu.AddKeybindList(
				mod: ModManifest,
				name: () => SHelper.Translation.Get("GMCM.PressKeys.Name"),
				getValue: () => Config.PressKeys,
				setValue: value => Config.PressKeys = value
			);
			configMenu.AddNumberOption(
				mod: ModManifest,
				name: () => SHelper.Translation.Get("GMCM.OpacityPercent.Name"),
				getValue: () => Config.OpacityPercent,
				setValue: value => Config.OpacityPercent = value,
				min: 1,
				max: 100
			);
			configMenu.AddNumberOption(
				mod: ModManifest,
				name: () => SHelper.Translation.Get("GMCM.SizePercent.Name"),
				getValue: () => Config.SizePercent,
				setValue: value => Config.SizePercent = value
			);
			configMenu.AddNumberOption(
				mod: ModManifest,
				name: () => SHelper.Translation.Get("GMCM.OffsetX.Name"),
				getValue: () => Config.OffsetX,
				setValue: value => Config.OffsetX = value
			);
			configMenu.AddNumberOption(
				mod: ModManifest,
				name: () => SHelper.Translation.Get("GMCM.OffsetY.Name"),
				getValue: () => Config.OffsetY,
				setValue: value => Config.OffsetY = value
			);
		}
	}
}
