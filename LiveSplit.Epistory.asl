state("Epistory", "1.0") {
	bool InGameMenu : "mono.dll", 0x294B88, 0x20, 0x250, 0x20, 0x30, 0x4c;
	bool GamePaused : "mono.dll", 0x294B88, 0x20, 0x250, 0x18, 0xb7;
	bool InMainMenu : "mono.dll", 0x294B88, 0x20, 0x250, 0x18, 0xb5;
	bool IsEnding : "mono.dll", 0x294B88, 0x20, 0x250, 0x18, 0xb6;
	int FinalWordObject : "mono.dll", 0x294B88, 0x20, 0x250, 0xa0, 0x28;
	int ActiveWordCount : "mono.dll", 0x294B88, 0x20, 0x250, 0xa0, 0x28, 0xa8, 0x18;
	string4 FinalWord1 : "mono.dll", 0x294B88, 0x20, 0x250, 0xa0, 0x28, 0xa8, 0x10, 0x20, 0x14;
	string8 FinalWord2 : "mono.dll", 0x294B88, 0x20, 0x250, 0xa0, 0x28, 0xa8, 0x10, 0x28, 0x14;
	string4 FinalWord3 : "mono.dll", 0x294B88, 0x20, 0x250, 0xa0, 0x28, 0xa8, 0x10, 0x30, 0x14;
	string200 Scene : "mono.dll", 0x294B88, 0x20, 0x250, 0x18, 0x38, 0x14;
	string200 SceneToLoad : "mono.dll", 0x294B88, 0x20, 0x250, 0x18, 0xa0, 0x14;
}
init {
	version = "1.0";
	vars.currentSplit = 0;
	vars.finalWordObject = 0;
}
start {
	vars.currentSplit = 0;
	vars.finalWordObject = 0;
	timer.IsGameTimePaused = true;
	return old.Scene == "Scene_main_menu" && current.SceneToLoad != "Scene_main_menu" && !string.IsNullOrEmpty(current.SceneToLoad);
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
			if(vars.finalWordObject == 0) {
				if (current.ActiveWordCount == 3) {
					if (current.FinalWord1 == "my" && current.FinalWord2 == "name" && current.FinalWord3 == "is") {
						vars.finalWordObject = current.FinalWordObject;
					}
				}
			} else {
				bool end = memory.ReadValue<int>((IntPtr)vars.finalWordObject + 0xec) == 3;
				if(end) {
					vars.currentSplit++;
				}
				return end;
			}
		}
	}
	return false;
}
update {
}
isLoading {
	return (current.GamePaused || current.InMainMenu) && !current.InGameMenu;
}
exit {
	timer.IsGameTimePaused = true;
}