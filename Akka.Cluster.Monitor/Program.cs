using System;
using System.Windows.Forms;
using Akka.Actor;

namespace Akka.Cluster.Monitor
{
    static class Program
    {

        public static ActorSystem MyActorSystem;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }
    }
}
