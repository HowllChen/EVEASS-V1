using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVEASS_V1
{
    partial class industryActivityMaterials
    {
        public industryActivityMaterials Clone()
        {
            return (industryActivityMaterials)MemberwiseClone();
        }
    }

    public static class ExtensionMethodIndustryActivityMaterials
    {
        /// <summary>
        /// 将一个材料并入当前材料列表中
        /// </summary>
        /// <param name="listMaterials"></param>
        /// <param name="iam"></param>
        public static void AddMaterial(this List<industryActivityMaterials> listMaterials, industryActivityMaterials iam)
        {
            var material = listMaterials.SingleOrDefault(m => m.materialTypeID == iam.materialTypeID);

            if (material == null)
                listMaterials.Add(iam.Clone());
            else
                material.quantity += iam.quantity;
        }

        /// <summary>
        /// 将一个材料列表并入当前材料列表中
        /// </summary>
        /// <param name="listMaterials"></param>
        /// <param name="materials"></param>
        public static void AddMaterial(this List<industryActivityMaterials> listMaterials, IEnumerable<industryActivityMaterials> materials)
        {
            foreach (var m in materials)
            {
                listMaterials.AddMaterial(m);
            }
        }
    }
}
