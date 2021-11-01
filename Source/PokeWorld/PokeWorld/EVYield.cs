using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using System.Xml;

namespace PokeWorld
{
    public class EVYield
    {
        public StatDef stat;
        public int value;
        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "stat", xmlRoot.Name);
            value = ParseHelper.FromString<int>(xmlRoot.FirstChild.Value);
        }
    }
}
