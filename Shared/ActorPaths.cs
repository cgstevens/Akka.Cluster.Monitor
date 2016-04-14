namespace Shared
{
    public static class ActorPaths
    {
        public static readonly string ActorSystem = "myservice";
        public static readonly ActorMetaData JobMasterActor = new ActorMetaData("jobmaster", "/user/jobmaster");
        public static readonly ActorMetaData ClusterHelperActor = new ActorMetaData("clusterhelper", "/user/clusterhelper");
        public static readonly ActorMetaData ClusterStatusActor = new ActorMetaData("clusterstatus", "/user/clusterstatus");
        public static readonly ActorMetaData JobTaskerActor = new ActorMetaData("jobtasker", "/user/jobtasker");
        public static readonly ActorMetaData ClusterManagerActor = new ActorMetaData("clustermanager", "/user/clustermanager");
    }

    public class ActorMetaData
    {
        public ActorMetaData(string name, string path)
        {
            Name = name;
            Path = path;
        }

        public string Name { get; private set; }
        public string Path { get; private set; }
    }
}

