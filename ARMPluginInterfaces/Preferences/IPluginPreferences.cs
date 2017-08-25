using System;
using System.Collections.Generic;
using System.Text;

namespace ARMPluginInterfaces.Preferences
{
    public class PluginSettingsItem: Tuple<string,string>
    {
		public string Name { get { return this.Item1; } }
		public string Assembly { get { return this.Item2; } }

        public PluginSettingsItem(string name, string assembly): base(name,assembly)
        {

        }
    }

    public interface IPluginPreferences
    {
        ICollection<PluginSettingsItem> SettingsPlugins { get; }
    }
}