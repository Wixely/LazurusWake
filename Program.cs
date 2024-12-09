using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LazurusWake
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Connect to Docker using the Unix socket
            DockerClient client = new DockerClientConfiguration(
                new Uri("unix:///var/run/docker.sock"))
                .CreateClient();

            // Label to filter
            string labelKey = "LazurusWake";

            // Find and start containers
            Console.WriteLine($"Searching for stopped containers with label '{labelKey}'...");
            
            while(true)
            {
                try
                {
                    StartContainersWithLabel(client, labelKey).Wait();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error getting docker info '{ex}'");
                    break;
                }
                Thread.Sleep(1000 * 60);
            }

            Console.WriteLine($"Lazurus Wake stopping!");
            client.Dispose();
        }

        static async Task StartContainersWithLabel(DockerClient client, string labelKey)
        {
            // List containers with the specified label
            var containers = await client.Containers.ListContainersAsync(new ContainersListParameters
            {
                All = true, // Include stopped containers
                Filters = new Dictionary<string, IDictionary<string, bool>>
                {
                    { "label", new Dictionary<string, bool> { { labelKey, true } } }
                }
            });

            // Loop through containers and start stopped ones
            foreach (var container in containers)
            {
                if (container.State == "exited") // Check if container is stopped
                {
                    Console.WriteLine($"Starting container {container.ID} ({string.Join(", ", container.Names)})...");

                    bool started = await client.Containers.StartContainerAsync(container.ID, new ContainerStartParameters());
                    if (started)
                    {
                        Console.WriteLine($"Container {container.ID} started successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to start container {container.ID}.");
                    }
                }
            }
        }

    }
}
