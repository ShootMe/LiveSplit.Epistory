state("Epistory", "1.0") {
	bool InGameMenu : "mono.dll", 0x294B88, 0x20, 0x250, 0x20, 0x30, 0x4c;
	bool GamePaused : "mono.dll", 0x294B88, 0x20, 0x250, 0x18, 0xb7;
	bool InMainMenu : "mono.dll", 0x294B88, 0x20, 0x250, 0x18, 0xb5;
	bool IsEnding : "mono.dll", 0x294B88, 0x20, 0x250, 0x18, 0xb6;
	string200 Scene : "mono.dll", 0x294B88, 0x20, 0x250, 0x18, 0x38, 0x14;
	string200 SceneToLoad : "mono.dll", 0x294B88, 0x20, 0x250, 0x18, 0xa0, 0x14;
}
startup {
	settings.Add("Burning Hollow");
	settings.Add("Drowning Halls");
	settings.Add("Creation City");
	settings.Add("Shattered Isles");
}
init {
	version = "1.0";
	vars.currentSplit = 0;
	print("Size: " + modules.First().ModuleMemorySize.ToString());
}
start {
	timer.IsGameTimePaused = true;
	return old.Scene == "Scene_main_menu" && current.SceneToLoad != "Scene_main_menu";
}
reset {
	return current.SceneToLoad == "Scene_main_menu";
}
split {
	if(vars.currentSplit == 0) {
		if(current.Scene == "Scene_Dungeon_Fire") {
			vars.currentSplit++;
		}
	} else if(vars.currentSplit == 1) {
		if(current.SceneToLoad == "Scene_Hub") {
			vars.currentSplit++;
			return true;
		}
	} else if(vars.currentSplit == 2) {
		if(current.Scene == "Scene_Dungeon_WaterSpring") {
			vars.currentSplit++;
		}
	} else if(vars.currentSplit == 3) {
		if(current.SceneToLoad == "Scene_Hub") {
			vars.currentSplit++;
			return true;
		}
	} else if(vars.currentSplit == 4) {
		if(current.Scene == "Scene_Dungeon_LightCity") {
			vars.currentSplit++;
		}
	} else if(vars.currentSplit == 5) {
		if(current.SceneToLoad == "Scene_Hub") {
			vars.currentSplit++;
			return true;
		}
	} else if(vars.currentSplit == 6) {
		if(current.Scene == "Scene_Dungeon_WindIsland") {
			vars.currentSplit++;
		}
	} else if(vars.currentSplit == 7) {
		if(current.SceneToLoad == "Scene_Hub") {
			vars.currentSplit++;
			return true;
		}
	} else if(vars.currentSplit == 8) {
		if(current.Scene == "Scene_Dungeon_Desert") {
			vars.currentSplit++;
		}
	} else if(vars.currentSplit == 9) {
		if(current.IsEnding) {
			vars.currentSplit++;
			return true;
		}
	}
	return false;
}
update {
	//print("Scene: " + current.Scene.ToString() + "\nSceneToLoad: " + current.SceneToLoad.ToString() + 
	//"\nGamePaused: " + current.GamePaused + "\nInMainMenu: " + current.InMainMenu + 
	//"\nInGameMenu: " + current.InGameMenu);
}
isLoading {
	return (current.GamePaused || current.InMainMenu) && !current.InGameMenu;
}
exit {
	timer.IsGameTimePaused = true;
}