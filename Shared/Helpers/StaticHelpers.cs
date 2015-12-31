using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Helpers
{
    public static class StaticHelpers
    {
        public static T DeepClone<T>(this T a)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(stream, a);
                    stream.Position = 0;
                    return (T)formatter.Deserialize(stream);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
