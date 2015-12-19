namespace Shared
{
    public static class ActorPaths
    {
        public static readonly string ActorSystem = "myservice";
        public static readonly ActorMetaData ClusterHelperActor = new ActorMetaData("clusterhelper", "akka://myservice/user/clusterhelper");
        public static readonly ActorMetaData ClusterStatusActor = new ActorMetaData("clusterstatus", "akka://myservice/user/clusterstatus");
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

