// Copyright 2014-2015 Aaron Stannard, Petabridge LLC
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
using System.Configuration;
using System.Linq;
using Akka.Actor;
using Akka.Configuration;
using Akka.Configuration.Hocon;
using ConfigurationException = Akka.Configuration.ConfigurationException;

namespace Akka.Cluster.Monitor.Actors
{
    public static class ActorSystemFactory
    {
        public static ActorSystem LaunchClusterManager(string systemName, string ipAddress = null, int? port = null)
        {
            var section = (AkkaConfigurationSection)ConfigurationManager.GetSection("akka");
            var akkaConfig = section.AkkaConfig;

            var clusterConfig = akkaConfig.GetConfig("akka.cluster");

            var clusterAddress = string.Format("akka.tcp://{0}@{1}:{2}/system/receptionist", systemName, ipAddress, port);

            var injectedClusterConfigString = @"akka.cluster.client.initial-contacts = [""" + clusterAddress + @"""]";

            var finalConfig = ConfigurationFactory.ParseString(
                injectedClusterConfigString)
                .WithFallback(ConfigurationFactory.ParseString(injectedClusterConfigString))
                .WithFallback(akkaConfig);

            return ActorSystem.Create(systemName, finalConfig);
        }

    }
}
