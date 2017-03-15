#if !Info
using LiveSplit.Model;
using LiveSplit.UI.Components;
using System;
using System.Reflection;
namespace LiveSplit.Epistory {
	public class HollowKnightFactory : IComponentFactory {
		public string ComponentName { get { return "Epistory Autosplitter v" + this.Version.ToString(); } }
		public string Description { get { return "Autosplitter for Epistory"; } }
		public ComponentCategory Category { get { return ComponentCategory.Control; } }
		public IComponent Create(LiveSplitState state) { return new EpistoryComponent(); }
		public string UpdateName { get { return this.ComponentName; } }
		public string UpdateURL { get { return "https://raw.githubusercontent.com/ShootMe/LiveSplit.Epistory/master/"; } }
		public string XMLURL { get { return this.UpdateURL + "Components/LiveSplit.Epistory.Updates.xml"; } }
		public Version Version { get { return Assembly.GetExecutingAssembly().GetName().Version; } }
	}
}
#endif