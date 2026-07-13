using System.Collections.Generic;

namespace Celeste.Mod.CelesteNet.Server.API
{
    public class APISettings : CelesteNetServerModuleSettings
    {
        public bool Enabled { get; set; } = false;

        public bool UseSqliteFallback { get; set; } = false;

        public string ApiBase { get; set; } = "https://celestenet.0x0a.de/api";

        public string InstanceNameForUserAgent { get; set; } = "Instance Name";

        public List<string> InstanceAdmins { get; set; } = [];

        public List<string> InstanceMods { get; set; } = [];
    }
}
