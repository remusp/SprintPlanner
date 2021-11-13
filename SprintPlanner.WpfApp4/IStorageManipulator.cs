using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SprintPlanner.WpfApp
{
    public interface IStorageManipulator
    {
        void Pull();
        void Flush();
    }
}
