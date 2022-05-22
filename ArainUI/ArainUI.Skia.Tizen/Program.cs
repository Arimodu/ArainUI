using Tizen.Applications;
using Uno.UI.Runtime.Skia;

namespace ArainUI.Skia.Tizen
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var host = new TizenHost(() => new ArainUI.App(), args);
            host.Run();
        }
    }
}
