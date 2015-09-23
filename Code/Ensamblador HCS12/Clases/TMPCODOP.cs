using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ensamblador_HCS12
{
    public class TMPCODOP
    {
        public string name;
        public int start;
        public int end;

        public void setTMPCODOP(string name,int start,int end)
        {
            this.name = name;
            this.start = start;
            this.end = end;
        }
        public void setName(string name)
        {
            this.name = name;
        }
        public void setStart(int start)
        {
            this.start = start;
        }
        public void setEnd(int end)
        {
            this.end = end;
        }


        public string getName()
        {
            return this.name;
        }
        public int getStart()
        {
            return this.start;
        }
        public int getEnd()
        {
            return this.end;
        }
    }
}
