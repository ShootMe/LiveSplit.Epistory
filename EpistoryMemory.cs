using System;
using System.Collections.Generic;
using System.Diagnostics;
using LiveSplit.Memory;
namespace LiveSplit.Epistory {
	public enum ManagerOffset {
		CollectibleManager = -0x60,
		ComboManager = -0x58,
		DifficultyManager = -0x50,
		GameController = -0x48,
		HudManager = -0x40,
		InterfaceFader = -0x38,
		MagicManager = -0x30,
		RewardManager = -0x28,
		RootObject = -0x20,
		Player = -0x18,
		ScoreManager = -0x10,
		SkillManager = -0x8,
		StatisticsManager = 0,
		VoiceOverManager = 0x8,
		ControlsManager = 0x10,
		CurrentLevel = 0x18,
		FrustumCamera = 0x20,
		MainCamera = 0x28,
		ProfileManager = 0x30,
		TileManagerLayer = 0x38,
		TypingManager = 0x40
	}
	public partial class EpistoryMemory {
		private ProgramPointer gameObjectManager;
		public Process Program { get; set; }
		public bool IsHooked { get; set; } = false;
		private DateTime lastHooked;

		public EpistoryMemory() {
			gameObjectManager = new ProgramPointer(this, MemPointer.GameObjectManager) { AutoDeref = false };
			lastHooked = DateTime.MinValue;
		}

		public bool GetGamePaused() {
			return gameObjectManager.Read<bool>((int)ManagerOffset.GameController, 0xb7);
		}
		public bool GetInMenu() {
			return gameObjectManager.Read<bool>((int)ManagerOffset.GameController, 0xb5);
		}
		public bool GetInGameMenu() {
			return gameObjectManager.Read<bool>((int)ManagerOffset.HudManager, 0x30, 0x4c);
		}
		public string GetCurrentScene() {
			return gameObjectManager.Read((int)ManagerOffset.GameController, 0x38);
		}
		public string GetSceneToLoad() {
			return gameObjectManager.Read((int)ManagerOffset.GameController, 0xa0);
		}
		public float GetDifficulty() {
			return gameObjectManager.Read<float>((int)ManagerOffset.DifficultyManager, 0x8c);
		}
		public int GetMissedKeys() {
			return gameObjectManager.Read<int>((int)ManagerOffset.DifficultyManager, 0x94);
		}
		public int GetNoMissStreak() {
			return gameObjectManager.Read<int>((int)ManagerOffset.StatisticsManager, 0x6c);
		}
		public int GetHighestNoMissStreak() {
			return gameObjectManager.Read<int>((int)ManagerOffset.StatisticsManager, 0x8c);
		}
		public int GetTotalMissedKeys() {
			return gameObjectManager.Read<int>((int)ManagerOffset.StatisticsManager, 0x88);
		}
		public int GetDeathCount() {
			return gameObjectManager.Read<int>((int)ManagerOffset.StatisticsManager, 0x80);
		}
		public int GetKillsCount() {
			return gameObjectManager.Read<int>((int)ManagerOffset.StatisticsManager, 0x48);
		}
		public int GetNestsCount() {
			return gameObjectManager.Read<int>((int)ManagerOffset.StatisticsManager, 0x5c);
		}
		public int GetChestsCount() {
			return gameObjectManager.Read<int>((int)ManagerOffset.StatisticsManager, 0x60);
		}
		public int GetWordsTyped() {
			return gameObjectManager.Read<int>((int)ManagerOffset.StatisticsManager, 0x90);
		}
		public int GetTotalCharactersTyped() {
			return gameObjectManager.Read<int>((int)ManagerOffset.StatisticsManager, 0x98);
		}
		public float GetHighestWPM() {
			return gameObjectManager.Read<float>((int)ManagerOffset.StatisticsManager, 0x68);
		}
		public float GetPositiveScaling() {
			return gameObjectManager.Read<float>((int)ManagerOffset.DifficultyManager, 0x3c);
		}
		public float GetNegativeScaling() {
			return gameObjectManager.Read<float>((int)ManagerOffset.DifficultyManager, 0x40);
		}
		public int ActiveWordCount() {
			return gameObjectManager.Read<int>((int)ManagerOffset.TypingManager, 0x28, 0xa8, 0x18);
		}
		public string ActiveWord(int number) {
			return gameObjectManager.Read((int)ManagerOffset.TypingManager, 0x28, 0xa8, 0x10, 0x20 + (8 * (number - 1)), 0x14);
		}
		public int GetFragmentsFound() {
			IntPtr unlockedHead = (IntPtr)gameObjectManager.Read<uint>((int)ManagerOffset.CollectibleManager, 0x30);
			int unlockedSize = Program.Read<int>(unlockedHead, 0x38);
			int count = 0;
			for (int i = 0; i < unlockedSize; i++) {
				string key = Program.Read((IntPtr)Program.Read<uint>(unlockedHead, 0x20, 0x20 + (i * 8)));
				count += Program.Read<int>(unlockedHead, 0x28, 0x20 + (i * 4));
			}
			return count;
		}
		public float GetWPM() {
			int TypingSpeedSampleSecondCount = gameObjectManager.Read<int>((int)ManagerOffset.StatisticsManager, 0x40);
			int CharacterPerSecondArrayCount = gameObjectManager.Read<int>((int)ManagerOffset.StatisticsManager, 0x20, 0x28);
			if ((float)CharacterPerSecondArrayCount < (float)TypingSpeedSampleSecondCount / 10f) {
				return 0f;
			}

			int count = 0;
			int listTotal = 0;
			IntPtr linkedListHead = (IntPtr)gameObjectManager.Read<uint>((int)ManagerOffset.StatisticsManager, 0x20, 0x18);
			IntPtr linkedListNode = linkedListHead;
			while (linkedListNode != IntPtr.Zero) {
				listTotal += Program.Read<int>(linkedListNode, 0x28);
				count++;
				linkedListNode = (IntPtr)Program.Read<uint>(linkedListNode, 0x20);
				if (linkedListNode == linkedListHead) { break; }
			}
			return count == 0 ? 0 : (float)(listTotal * 60f) / (5f * (float)count);
		}
		public bool HookProcess() {
			if ((Program == null || Program.HasExited) && DateTime.Now > lastHooked.AddSeconds(1)) {
				lastHooked = DateTime.Now;
				Process[] processes = Process.GetProcessesByName("Epistory");
				Program = processes.Length == 0 ? null : processes[0];
				IsHooked = true;
			}

			if (Program == null || Program.HasExited) {
				IsHooked = false;
			}

			return IsHooked;
		}
		public void Dispose() {
			if (Program != null) {
				Program.Dispose();
			}
		}
	}
	public enum MemVersion {
		None,
		V1
	}
	public enum MemPointer {
		GameObjectManager
	}
	public class ProgramPointer {
		private static Dictionary<MemVersion, Dictionary<MemPointer, string>> funcPatterns = new Dictionary<MemVersion, Dictionary<MemPointer, string>>() {
			{MemVersion.V1, new Dictionary<MemPointer, string>() {
				{MemPointer.GameObjectManager, "41FFD34883C420488BC8B8????????488908833F0041BA????????488BCF4883EC2049BB????????????????41FFD34883C420488BC8B8????????488908B9????????4883EC20|-16" }
			}},
		};
		private IntPtr pointer;
		public EpistoryMemory Memory { get; set; }
		public MemPointer Name { get; set; }
		public MemVersion Version { get; set; }
		public bool AutoDeref { get; set; }
		private int lastID;
		private DateTime lastTry;
		public ProgramPointer(EpistoryMemory memory, MemPointer pointer) {
			this.Memory = memory;
			this.Name = pointer;
			this.AutoDeref = true;
			lastID = memory.Program == null ? -1 : memory.Program.Id;
			lastTry = DateTime.MinValue;
		}

		public IntPtr Value {
			get {
				GetPointer();
				return pointer;
			}
		}
		public T Read<T>(params int[] offsets) where T : struct {
			return Memory.Program.Read<T>(Value, offsets);
		}
		public byte[] ReadBytes(int count, params int[] offsets) {
			return Memory.Program.Read(Value, count, offsets);
		}
		public string Read(params int[] offsets) {
			if (!Memory.IsHooked) { return string.Empty; }

			bool is64bit = Memory.Program.Is64Bit();
			IntPtr p = IntPtr.Zero;
			if (is64bit) {
				p = (IntPtr)Memory.Program.Read<ulong>(Value, offsets);
			} else {
				p = (IntPtr)Memory.Program.Read<uint>(Value, offsets);
			}
			return Memory.Program.Read(p, is64bit);
		}
		public void Write(int value, params int[] offsets) {
			Memory.Program.Write(Value, value, offsets);
		}
		public void Write(long value, params int[] offsets) {
			Memory.Program.Write(Value, value, offsets);
		}
		public void Write(double value, params int[] offsets) {
			Memory.Program.Write(Value, value, offsets);
		}
		public void Write(float value, params int[] offsets) {
			Memory.Program.Write(Value, value, offsets);
		}
		public void Write(short value, params int[] offsets) {
			Memory.Program.Write(Value, value, offsets);
		}
		public void Write(byte value, params int[] offsets) {
			Memory.Program.Write(Value, value, offsets);
		}
		public void Write(bool value, params int[] offsets) {
			Memory.Program.Write(Value, value, offsets);
		}
		private void GetPointer() {
			if (!Memory.IsHooked) {
				pointer = IntPtr.Zero;
				Version = MemVersion.None;
				return;
			}

			if (Memory.Program.Id != lastID) {
				pointer = IntPtr.Zero;
				Version = MemVersion.None;
				lastID = Memory.Program.Id;
			}
			if (pointer == IntPtr.Zero && DateTime.Now > lastTry.AddSeconds(1)) {
				lastTry = DateTime.Now;
				pointer = GetVersionedFunctionPointer();
				if (pointer != IntPtr.Zero) {
					bool is64bit = Memory.Program.Is64Bit();
					pointer = (IntPtr)Memory.Program.Read<uint>(pointer);
					if (AutoDeref) {
						if (is64bit) {
							pointer = (IntPtr)Memory.Program.Read<ulong>(pointer);
						} else {
							pointer = (IntPtr)Memory.Program.Read<uint>(pointer);
						}
					}
				}
			}
		}
		private IntPtr GetVersionedFunctionPointer() {
			foreach (MemVersion version in Enum.GetValues(typeof(MemVersion))) {
				Dictionary<MemPointer, string> patterns = null;
				if (!funcPatterns.TryGetValue(version, out patterns)) { continue; }

				string pattern = null;
				if (!patterns.TryGetValue(Name, out pattern)) { continue; }

				IntPtr ptr = Memory.Program.FindSignatures(pattern)[0];
				if (ptr != IntPtr.Zero) {
					Version = version;
					return ptr;
				}
			}
			Version = MemVersion.None;
			return IntPtr.Zero;
		}
	}
}