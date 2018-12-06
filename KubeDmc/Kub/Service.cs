using k8s.Models;
using System.Collections.Generic;

namespace KubeDmc.Kub
{
    public class Service : Item
    {
        [Item("CLUSTER-IP")]
        public string ClusterIP { get; internal set; }

        public IList<string> ExternalIPs { get; internal set; }

        public IList<V1ServicePort> Ports { get; internal set; }

        [Item("TYPE")]
        public string ServiceType { get; internal set; }

        [Item("EXTERNAL-IP")]
        public string ExternalIP
        {
            get
            {
                if (this.ExternalIPs == null || this.ExternalIPs.Count == 0)
                    return "<none>";

                var firstIP = this.ExternalIPs[0];

                return firstIP;

            }
        }


        [Item("PORT(S)")]
        public string Port
        {
            get
            {
                if (this.Ports == null || this.Ports.Count == 0)
                    return "<none>";

                var firstPort = this.Ports[0];

                if (firstPort.NodePort.HasValue)
                    return $"{firstPort.Port}:{firstPort.NodePort}/{firstPort.Protocol}";
                else
                    return $"{firstPort.Port}/{firstPort.Protocol}";

            }
        }
    }
}
