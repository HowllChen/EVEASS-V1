using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EVEASS_V1.ESI_Code;

namespace EVEASS_V1
{
    partial class UniversePrice
    {
        /// <summary>
        /// 更新系统价格
        /// </summary>
        /// <returns></returns>
        public static bool Update()
        {
            // TODO: 暂时每次启动时更新一次, 后期根据需要修改

            // 数据过期时间
            DateTime expiresIn = new DateTime();

            // 从 ESI 获取系统价格数据
            List<ESIMarketPrice> eSIMarketPrices = ESI.GetMarketPrices(ref expiresIn);

            // 如果获取不成功则返回
            if (eSIMarketPrices == null)
                return false;

            DCEVEDataContext dc = new DCEVEDataContext();
            dc.UniversePrice.DeleteAllOnSubmit(dc.UniversePrice);       // 清空所有旧数据
            dc.SubmitChanges();

            UniversePrice up;
            foreach (var mp in eSIMarketPrices)
            {
                // 判断物品类型数据库是否有跟踪, 无跟踪则跳过
                if (dc.invTypes.SingleOrDefault(t => t.typeID == mp.TypeID) == null)
                    continue;

                // 新建数据
                up = new UniversePrice();

                // 更新数据
                up.TypeID = mp.TypeID;
                up.AdjustedPrice = mp.AdjustedPrice;
                up.AveragePrice = mp.AveragePrice;
                up.ExpiresIn = expiresIn;

                dc.UniversePrice.InsertOnSubmit(up);
            }

            // 提交数据库
            dc.SubmitChanges();

            return true;
        }
    }
}
