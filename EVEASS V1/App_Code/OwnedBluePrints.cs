using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using System.Threading.Tasks;

namespace EVEASS_V1
{
    partial class OwnedBluePrints
    {
        /// <summary>
        /// 更新软件跟踪的所有角色和军团拥有的蓝图信息(提交数据库)
        /// </summary>
        public static List<OwnedBluePrints> Update()
        {
            List<OwnedBluePrints> tempOwnedBluePrints = new List<OwnedBluePrints>();
            DateTime updateTime = DateTime.Now;     // 统一更新时间

            // ESI获取所有角色和军团拥有的蓝图
            foreach (var c in Main.DC.Characters)
            {
                tempOwnedBluePrints.AddRange(c.GetBluePrints());
            }

            // 第一次遍历先将 ItemID 没有改变的蓝图对象的数据更新上去
            OwnedBluePrints bluePrintDB;
            for (int i = tempOwnedBluePrints.Count - 1; i >= 0; i--)
            {
                // 查找数据库中对应 ItemID 的蓝图数据 (p.UpdateTime != updateTime 条件筛选掉已更新的数据)
                bluePrintDB = Main.DC.OwnedBluePrints.SingleOrDefault(p => p.UpdateTime != updateTime && p.ItemID == tempOwnedBluePrints[i].ItemID);

                // 若没有对应的数据, 则留待下次遍历处理
                if (bluePrintDB == null)
                    continue;

                // 虽然 ItemID 一样, 需通过确认蓝图基本属性是否相同来确认是否是同一个物品
                if (bluePrintDB.BluePrintID != tempOwnedBluePrints[i].BluePrintID
                    || bluePrintDB.TimeEfficiency != tempOwnedBluePrints[i].TimeEfficiency
                    || bluePrintDB.MaterialEfficiency != tempOwnedBluePrints[i].MaterialEfficiency
                    || bluePrintDB.Quantity != tempOwnedBluePrints[i].Quantity
                    || bluePrintDB.Runs != tempOwnedBluePrints[i].Runs)
                {
                    // 若不是同一个物品, 创建一条新记录来代替, 更新 ItemID 值
                    // 处理代码
                    // continue;

                    // 理论上不可能存在这种情况, 若出现,先用一个 MessageBox 提示
                    MessageBox.Show(string.Format("OwnedBluePrints.Update() 更新错误, 数据 ItemID:{0} 一样, 但是物品不一样. ", bluePrintDB.ItemID));

                    return null;
                }

                // 用 ESI 蓝图的数据更新数据库蓝图的一般数据
                _updateGeneralPropertyFromESIData(bluePrintDB, tempOwnedBluePrints[i]);

                // 统一设置数据更新时间
                tempOwnedBluePrints[i].UpdateTime = updateTime;

                // 更新完数据库蓝图数据后将临时蓝图列表中对应的对象删除
                tempOwnedBluePrints.Remove(tempOwnedBluePrints[i]);
            }

            // 若临时表中还有元素
            if (tempOwnedBluePrints.Count != 0)
            {
                for (int i = tempOwnedBluePrints.Count - 1; i >= 0; i--)
                {
                    // 根据蓝图基本属性查找数据库中对应的蓝图数据 (p.UpdateTime != updateTime 条件筛选掉已更新的数据)
                    bluePrintDB = Main.DC.OwnedBluePrints.SingleOrDefault(p => p.UpdateTime != updateTime
                        && p.BluePrintID == tempOwnedBluePrints[i].BluePrintID
                        && p.TimeEfficiency == tempOwnedBluePrints[i].TimeEfficiency
                        && p.MaterialEfficiency == tempOwnedBluePrints[i].MaterialEfficiency
                        && p.Quantity == tempOwnedBluePrints[i].Quantity
                        && p.Runs == tempOwnedBluePrints[i].Runs);

                    // 若没有对应的数据, 则留待最后用户确认
                    if (bluePrintDB == null)
                        continue;

                    // 有则创建一个新对象, 以实现更新 ItemID
                    OwnedBluePrints bluePrintNew = new OwnedBluePrints()
                    {
                        // 基本属性赋值
                        ItemID = tempOwnedBluePrints[i].ItemID,
                        BluePrintID = tempOwnedBluePrints[i].BluePrintID,
                        TimeEfficiency = tempOwnedBluePrints[i].TimeEfficiency,
                        MaterialEfficiency = tempOwnedBluePrints[i].MaterialEfficiency,
                        Quantity = tempOwnedBluePrints[i].Quantity,
                        Runs = tempOwnedBluePrints[i].Runs
                    };

                    // 用 ESI 蓝图的数据更新数据库蓝图的一般属性
                    _updateGeneralPropertyFromESIData(bluePrintDB, tempOwnedBluePrints[i]);

                    // 统一设置数据更新时间
                    tempOwnedBluePrints[i].UpdateTime = updateTime;

                    // 将旧数据从数据库中删除, 新数据插入数据库中
                    Main.DC.OwnedBluePrints.DeleteOnSubmit(bluePrintDB);
                    Main.DC.OwnedBluePrints.InsertOnSubmit(bluePrintNew);
                    //Main.DC.SubmitChanges();
                    // TODO: 不直接提交变更, 流到最后统一提交, 看会不会出错

                    // 更新完数据库蓝图数据后将临时蓝图列表中对应的对象删除
                    tempOwnedBluePrints.Remove(tempOwnedBluePrints[i]);
                }
            }

            // 若临时表中没有元素, 且数据库中的数据都已更新过, 则更新完成
            if (tempOwnedBluePrints.Count == 0 && Main.DC.OwnedBluePrints.Where(p => p.UpdateTime != updateTime).Count() == 0)
            {
                return null;
            }
            else
            {
                // TODO: 有数据没有一一对应时的处理方法 应该返回一个列表, 表示新增的项和删除的项;
                List<OwnedBluePrints> confirmOBP = new List<OwnedBluePrints>();

                if (tempOwnedBluePrints.Count != 0)
                {
                    // 将每个元素的更新时间都统一设置
                    tempOwnedBluePrints.ForEach(p => p.UpdateTime = updateTime);
                    Main.DC.OwnedBluePrints.InsertAllOnSubmit(tempOwnedBluePrints);
                    confirmOBP.AddRange(tempOwnedBluePrints);
                }

                var deleteBluePrints = Main.DC.OwnedBluePrints.Where(p => p.UpdateTime != updateTime);
                if (deleteBluePrints.Count() != 0)
                {
                    Main.DC.OwnedBluePrints.DeleteAllOnSubmit(deleteBluePrints);
                    confirmOBP.AddRange(deleteBluePrints);
                }

                return confirmOBP;
            }
        }

        /// <summary>
        /// 用 ESI 获取的蓝图数据更新数据库中的一般属性
        /// </summary>
        /// <param name="dbObject">数据库数据</param>
        /// <param name="esiObject">ESI数据</param>
        private static void _updateGeneralPropertyFromESIData(OwnedBluePrints dbObject, OwnedBluePrints esiObject)
        {
            dbObject.Owner = esiObject.Owner;
            dbObject.CharacterOwned = esiObject.CharacterOwned;
            dbObject.LocationID = esiObject.LocationID;
            dbObject.LocationFlag = esiObject.LocationFlag;
        }
    }
}
