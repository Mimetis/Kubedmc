using k8s;
using k8s.Models;
using KubeDmc.Kub;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KubeDmc
{
    public class KubService
    {
        public Kubernetes Kub { get; private set; }

        private const string ServiceAccountTokenKeyFilePath = "/var/run/secrets/kubernetes.io/serviceaccount/token";

    

        private KubService()
        {
        }



        // Singleton
        private static KubService current;
        public static KubService Current
        {
            get
            {
                if (current == null)
                    current = new KubService();

                return current;
            }
        }


        public bool TryToConnect()
        {
            try
            {
                KubernetesClientConfiguration config = null;

                var host = Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_HOST");
                var port = Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_PORT");
                var sap = File.Exists(ServiceAccountTokenKeyFilePath);

                // Can't create a config from service account path
                if (!sap || string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(port))
                {
                    config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
                }
                else
                {
                    config = KubernetesClientConfiguration.InClusterConfig();
                }

                this.Kub = new Kubernetes(config);
                return true;

            }
            catch (Exception)
            {
                var initialForegroundColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.Write("! ");
                Console.ForegroundColor = initialForegroundColor;

                Console.WriteLine("kubeconfig file not found.");

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("! ");
                Console.ForegroundColor = initialForegroundColor;

                Console.WriteLine("Service account token not found.");

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("! ");
                Console.ForegroundColor = initialForegroundColor;

                Console.WriteLine("Update your config to let kubedmc connect to your cluster.");

            }
            return false;
        }

        public IList<Namespace> GetNamespaces()
        {
            var namespaces = new List<Namespace>();
            var ns = this.Kub.ListNamespace().Items;
            foreach (var n in ns)
            {
                namespaces.Add(new Namespace
                {
                    Name = n.Metadata.Name,
                    Status = n.Status.Phase,
                    StartDateTime = n.Metadata.CreationTimestamp
                });
            }

            return namespaces;
        }

        internal IList<Node> GetNodes()
        {
            var nodeList = this.Kub.ListNode().Items;
            var nodes = new List<Node>();

            foreach (var n in nodeList)
            {
                var node = new Node();
                node.Name = n.Metadata.Name;
                node.StartDateTime = n.Metadata.CreationTimestamp;
                node.Status = n.Status.Conditions.FirstOrDefault(c => c.Type == "Ready")?.Status.ToLowerInvariant() == "true" ? "Ready" : "Not Ready";
                node.Version = n.Status.NodeInfo.KubeletVersion;
                node.Roles = n.Metadata.Labels.FirstOrDefault(l => l.Key.ToLower() == "kubernetes.io/role").Value;
                nodes.Add(node);
            }


            return nodes;
        }


        public IList<V1LoadBalancerIngress> GetLoadBalancers()
        {
            var loadBalancerService = KubService.Current.Kub.ListServiceForAllNamespaces()
               .Items
               .Where(s => s.Spec.Type == "LoadBalancer");

            return loadBalancerService.SelectMany(service => service.Status?.LoadBalancer?.Ingress).ToList();

        }


        public List<Deployment> GetDeployments(string ns)
        {
            var list = this.Kub.ListNamespacedDeployment1(ns).Items;
            var deployments = new List<Deployment>();
            foreach (var item in list)
            {
                deployments.Add(new Deployment
                {
                    Name = item.Metadata.Name,
                    StartDateTime = item.Metadata.CreationTimestamp,
                    Desired = item.Spec.Replicas.HasValue ? item.Spec.Replicas.Value : 0,
                    Current = item.Status.Replicas.HasValue ? item.Status.Replicas.Value : 0,
                    Available = item.Status.AvailableReplicas.HasValue ? item.Status.AvailableReplicas.Value : 0,
                    UpToDate = item.Status.ReadyReplicas.HasValue ? item.Status.ReadyReplicas.Value : 0,
                });
            }

            return deployments;
        }

        internal IEnumerable<Item> GetStatefulSets(string ns)
        {
            var list = this.Kub.ListNamespacedStatefulSet1(ns).Items;
            var stsList = new List<StatefulSet>();
            foreach (var item in list)
            {
                stsList.Add(new StatefulSet
                {
                    Name = item.Metadata.Name,
                    StartDateTime = item.Metadata.CreationTimestamp,
                    Desired = item.Spec.Replicas.HasValue ? item.Spec.Replicas.Value : 0,
                    Current = item.Status.Replicas
                });
            }

            return stsList;

        }


        internal IList<Service> GetServices(string ns)
        {
            var list = this.Kub.ListNamespacedService(ns).Items;
            var services = new List<Service>();

            foreach(var item in list)
            {
                
                services.Add(new Service
                {
                    Name = item.Metadata.Name,
                    StartDateTime = item.Metadata.CreationTimestamp,
                    ClusterIP = item.Spec.ClusterIP,
                    ExternalIPs = item.Status?.LoadBalancer?.Ingress?.Select(i => i.Ip.ToString()).ToList(),
                    Ports = item.Spec.Ports,
                    ServiceType = item.Spec.Type
                });
            }

            return services;
        }


        public IList<Pod> GetPods(string ns)
        {
            var list = this.Kub.ListNamespacedPod(ns);

            var pods = list?.Items?.Select(pod => new Pod
            {
                Name = pod.Metadata.Name,
                Status = pod.Status.Phase,
                StartDateTime = pod.Status.StartTime,
                ContainersReadyCount = pod.Status.ContainerStatuses.Count(cstatus => cstatus.Ready),
                ContainersCount = pod.Status.ContainerStatuses.Count,
                ContainerRestarts = pod.Status.ContainerStatuses.Sum(cstatus => cstatus.RestartCount)
            }
            ).ToList();

            return pods;
        }
    }
}
