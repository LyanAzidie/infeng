using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iengine
{
    public class BC
    {
        private Expression _kb;
        private Expression _goal;

        public BC(Expression kb, Expression asked)
        {
            _kb = kb;
            _goal = asked;
        }
    }
}
