using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Tekla.Structures;
using Tekla.Structures.Model;
using TSM =  Tekla.Structures.Model;
using Tekla.Structures.Model.History;

namespace TeklaHistory
{
    public static class TechreteMethods
    {
        public static List<T> ToAList<T>(this System.Collections.IEnumerator enumerator)
        {
            var list = new List<T>();
            while (enumerator.MoveNext())
            {
                var current = (T)enumerator.Current;
                if (current != null)
                {
                    list.Add(current);
                }
            }
            return list;
        }

        public static List<Part> GetAllParts(this Model model, bool autofetch)
        {
            ModelObjectEnumerator.AutoFetch = autofetch;

            var types = new[] { typeof(Part) };

            var parts = model.GetModelObjectSelector().GetAllObjectsWithType(types).ToAList<Part>();

            return parts;
        }

        public static List<Assembly> GetAllAssemblies(this Model model, bool autofetch)
        {
            ModelObjectEnumerator.AutoFetch = autofetch;

            var types = new[] { typeof(Assembly) };

            var assemblies = model.GetModelObjectSelector().GetAllObjectsWithType(types).ToAList<Assembly>();

            return assemblies;
        }

        public static List<TSM.Connection> GetAllConnections(this Model model, bool autofetch)
        {
            ModelObjectEnumerator.AutoFetch = autofetch;

            var types = new[] { typeof(TSM.Connection) };

            var connections = model.GetModelObjectSelector().GetAllObjectsWithType(types).ToAList<TSM.Connection>();

            return connections;
        }
    }
}
