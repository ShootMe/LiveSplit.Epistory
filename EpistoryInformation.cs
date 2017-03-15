using System;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
namespace LiveSplit.Epistory {
	public partial class EpistoryInformation : Form {
		public EpistoryMemory Memory { get; set; }

		public static void Main(string[] args) {
			try {
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new EpistoryInformation());
			} catch (Exception ex) {
				Console.WriteLine(ex.ToString());
			}
		}

		public EpistoryInformation() {
			this.DoubleBuffered = true;
			InitializeComponent();
			Text = "Epistory Information " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
			Memory = new EpistoryMemory();
			Thread t = new Thread(UpdateLoop);
			t.IsBackground = true;
			t.Start();
		}

		private void UpdateLoop() {
			bool lastHooked = false;
			while (true) {
				try {
					bool hooked = Memory.HookProcess();
					if (hooked) {
						UpdateValues();
					}
					if (lastHooked != hooked) {
						lastHooked = hooked;
						this.Invoke((Action)delegate () { lblNote.Visible = !hooked; });
					}
					Thread.Sleep(12);
				} catch { }
			}
		}
		public void UpdateValues() {
			if (this.InvokeRequired) {
				this.Invoke((Action)UpdateValues);
			} else {
				lblCurrentScene.Text = "Scene: " + Memory.GetCurrentScene();

				float WPM = Memory.GetWPM();
				lblWPMValue.Text = WPM.ToString("0.00") + " (Max: " + Memory.GetHighestWPM().ToString("0.00") + ")";
				lblNoMissValue.Text = Memory.GetNoMissStreak().ToString("0") + " (Max: " + Memory.GetHighestNoMissStreak() + ")";
				lblMissedKeysValue.Text = Memory.GetMissedKeys().ToString("0") + " (Total: " + Memory.GetTotalMissedKeys() + ")";
				float difficulty = Memory.GetDifficulty();
				lblDifficultyValue.Text = difficulty.ToString("0.00") + " (Scaled: " + (1f + WPM * difficulty * (difficulty > 0 ? Memory.GetPositiveScaling() : Memory.GetNegativeScaling())).ToString("0.000") + ")";
				lblTotalDeathsKillsValue.Text = Memory.GetDeathCount().ToString() + " / " + Memory.GetKillsCount().ToString();
				lblTotalKeysWordsValue.Text = Memory.GetTotalCharactersTyped().ToString() + " / " + Memory.GetWordsTyped().ToString();
				lblNestsChestsValue.Text = Memory.GetNestsCount().ToString() + " / " + Memory.GetChestsCount().ToString();
				lblFragmentsValue.Text = Memory.GetFragmentsFound().ToString();
			}
		}
	}
}
