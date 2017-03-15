#if !Info
using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
#endif
using System;
using System.Collections.Generic;
using System.IO;
namespace LiveSplit.Epistory {
#if !Info
	public class EpistoryComponent : IComponent {
		public TimerModel Model { get; set; }
		private string lastScene = null;
#else
	public class EpistoryComponent {
#endif
		public string ComponentName { get { return "Epistory Autosplitter"; } }
		public IDictionary<string, Action> ContextMenuControls { get { return null; } }
		internal static string[] keys = { "CurrentSplit", "State", "SceneName", "SceneToLoad", "Paused", "InMenu", "InGameMenu", "Deaths", "ActiveWordCount" };
		private EpistoryMemory mem;
		private int currentSplit = -1, state = 0, lastLogCheck = 0;
		private bool hasLog = false;
		private Dictionary<string, string> currentValues = new Dictionary<string, string>();
		private static string LOGFILE = "_Epistory.log";
		public EpistoryComponent() {
			mem = new EpistoryMemory();
			foreach (string key in keys) {
				currentValues[key] = "";
			}
		}

		public void GetValues() {
			if (!mem.HookProcess()) { return; }

#if !Info
			if (Model != null) {
				HandleSplits();
			}
#endif

			LogValues();
		}
#if !Info
		private void HandleSplits() {
			bool shouldSplit = false;

			string scene = mem.GetCurrentScene();
			string sceneToLoad = mem.GetSceneToLoad();

			if (currentSplit == -1) {
				shouldSplit = lastScene == "Scene_main_menu" && sceneToLoad != "Scene_main_menu" && !string.IsNullOrEmpty(sceneToLoad);
			} else if (Model.CurrentState.CurrentPhase == TimerPhase.Running) {
				if (currentSplit == 0) {
					if (scene == "Scene_Dungeon_Fire") {
						state++;
					} else if (state == 1) {
						shouldSplit = sceneToLoad == "Scene_Hub";
					}
				} else if (currentSplit == 1) {
					if (scene == "Scene_Dungeon_WaterSpring") {
						state++;
					} else if (state == 1) {
						shouldSplit = sceneToLoad == "Scene_Hub";
					}
				} else if (currentSplit == 2) {
					if (scene == "Scene_Dungeon_LightCity") {
						state++;
					} else if (state == 1) {
						shouldSplit = sceneToLoad == "Scene_Hub";
					}
				} else if (currentSplit == 3) {
					if (scene == "Scene_Dungeon_WindIsland") {
						state++;
					} else if (state == 1) {
						shouldSplit = sceneToLoad == "Scene_Hub";
					}
				}

				Model.CurrentState.IsGameTimePaused = (mem.GetGamePaused() || mem.GetInMenu()) && !mem.GetInGameMenu();
			}
			
			lastScene = scene;

			HandleSplit(shouldSplit, sceneToLoad == "Scene_main_menu");
		}
		private void HandleSplit(bool shouldSplit, bool shouldReset = false) {
			if (shouldReset) {
				if (currentSplit >= 0) {
					Model.Reset();
				}
			} else if (shouldSplit) {
				if (currentSplit < 0) {
					Model.Start();
				} else {
					Model.Split();
				}
			}
		}
#endif
		private void LogValues() {
			if (lastLogCheck == 0) {
				hasLog = File.Exists(LOGFILE);
				lastLogCheck = 300;
			}
			lastLogCheck--;

			if (hasLog || !Console.IsOutputRedirected) {
				string prev = "", curr = "";
				foreach (string key in keys) {
					prev = currentValues[key];

					switch (key) {
						case "CurrentSplit": curr = currentSplit.ToString(); break;
						case "State": curr = state.ToString(); break;
						case "SceneName": curr = mem.GetCurrentScene(); break;
						case "SceneToLoad": curr = mem.GetSceneToLoad(); break;
						case "Paused": curr = mem.GetGamePaused().ToString(); break;
						case "InMenu": curr = mem.GetInMenu().ToString(); break;
						case "InGameMenu": curr = mem.GetInGameMenu().ToString(); break;
						case "Deaths": curr = mem.GetDeathCount().ToString(); break;
						case "ActiveWordCount": curr = mem.ActiveWordCount().ToString(); break;
						default: curr = ""; break;
					}

					if (!prev.Equals(curr)) {
						WriteLogWithTime(key + ": ".PadRight(16 - key.Length, ' ') + prev.PadLeft(25, ' ') + " -> " + curr);

						currentValues[key] = curr;
					}
				}
			}
		}
		private void WriteLog(string data) {
			if (hasLog || !Console.IsOutputRedirected) {
				if (!Console.IsOutputRedirected) {
					Console.WriteLine(data);
				}
				if (hasLog) {
					using (StreamWriter wr = new StreamWriter(LOGFILE, true)) {
						wr.WriteLine(data);
					}
				}
			}
		}
		private void WriteLogWithTime(string data) {
#if !Info
			WriteLog(DateTime.Now.ToString(@"HH\:mm\:ss.fff") + (Model != null && Model.CurrentState.CurrentTime.RealTime.HasValue ? " | " + Model.CurrentState.CurrentTime.RealTime.Value.ToString("G").Substring(3, 11) : "") + ": " + data);
#else
			WriteLog(DateTime.Now.ToString(@"HH\:mm\:ss.fff") + ": " + data);
#endif
		}

#if !Info
		public void Update(IInvalidator invalidator, LiveSplitState lvstate, float width, float height, LayoutMode mode) {
			if (Model == null) {
				Model = new TimerModel() { CurrentState = lvstate };
				Model.InitializeGameTime();
				Model.CurrentState.IsGameTimePaused = true;
				lvstate.OnReset += OnReset;
				lvstate.OnPause += OnPause;
				lvstate.OnResume += OnResume;
				lvstate.OnStart += OnStart;
				lvstate.OnSplit += OnSplit;
				lvstate.OnUndoSplit += OnUndoSplit;
				lvstate.OnSkipSplit += OnSkipSplit;
			}

			GetValues();
		}

		public void OnReset(object sender, TimerPhase e) {
			currentSplit = -1;
			state = 0;
			Model.CurrentState.IsGameTimePaused = true;
			WriteLog("---------Reset----------------------------------");
		}
		public void OnResume(object sender, EventArgs e) {
			WriteLog("---------Resumed--------------------------------");
		}
		public void OnPause(object sender, EventArgs e) {
			WriteLog("---------Paused---------------------------------");
		}
		public void OnStart(object sender, EventArgs e) {
			currentSplit = 0;
			state = 0;
			Model.CurrentState.IsGameTimePaused = false;
			WriteLog("---------New Game-------------------------------");
		}
		public void OnUndoSplit(object sender, EventArgs e) {
			currentSplit--;
			state = 0;
		}
		public void OnSkipSplit(object sender, EventArgs e) {
			currentSplit++;
			state = 0;
		}
		public void OnSplit(object sender, EventArgs e) {
			currentSplit++;
			state = 0;
		}
		public Control GetSettingsControl(LayoutMode mode) { return null; }
		public void SetSettings(XmlNode document) { }
		public XmlNode GetSettings(XmlDocument document) { return document.CreateElement("Settings"); }
		public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion) { }
		public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion) { }
#endif
		public float HorizontalWidth { get { return 0; } }
		public float MinimumHeight { get { return 0; } }
		public float MinimumWidth { get { return 0; } }
		public float PaddingBottom { get { return 0; } }
		public float PaddingLeft { get { return 0; } }
		public float PaddingRight { get { return 0; } }
		public float PaddingTop { get { return 0; } }
		public float VerticalHeight { get { return 0; } }
		public void Dispose() { }
	}
}