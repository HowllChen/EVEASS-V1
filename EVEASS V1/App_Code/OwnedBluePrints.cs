using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace EVEASS_V1
{
    partial class OwnedBluePrints
    {
        /// <summary>
        /// 从 ESI 获取所有角色拥有的蓝图信息
        /// </summary>
        /// <returns></returns>
        public static List<OwnedBluePrints> OwnedBluePrintsOfAllCharacters()
        {
            var esiOwnedBluePrints = new List<OwnedBluePrints>();    // ESI 取得的 OwnedBluePrints 数据

            var dc = new DCEVEDataContext();

            // ESI获取所有角色和军团拥有的蓝图
            foreach (var c in dc.Characters)
            {
                // TODO: Z 多线程处理 OwnedBluePrints.Update() -> Characters.GetBluePrints() 
                esiOwnedBluePrints.AddRange(c.GetBluePrints());
            }

            return esiOwnedBluePrints;
        }

        /// <summary>
        /// 更新软件跟踪的所有角色和军团拥有的蓝图信息
        /// </summary>
        /// <returns>若更新成功则返回 True, 应 SubmitChange() </returns>
        public static bool Update()
        {
            DCEVEDataContext dc = new DCEVEDataContext();
            // TODO: First OwnedBluePrints.Update() 执行前, 应该先根据数据库蓝图更新时间, 将从上次更新到目前为止的工业历史更新进数据库蓝图的属性中.()
            DateTime updateTime = DateTime.Now;     // 统一更新时间
            var esiOwnedBluePrints = OwnedBluePrintsOfAllCharacters();    // ESI 取得的 OwnedBluePrints 数据

            var dbOwnedBluePrints = dc.OwnedBluePrints.ToList();   // 数据库上的 OwnedBluePrints 数据 -> 必须调用 ToList() 方法, 否则每次都会重新重数据库获取数据
            
            // 第一次遍历先将 ItemID 匹配得上且基本属性没有改变的蓝图对象的数据更新
            OwnedBluePrints bluePrintDB;
            for (int i = esiOwnedBluePrints.Count - 1; i >= 0; i--)
            {
                // 查找数据库中对应 ItemID 的蓝图数据 (p.UpdateTime != updateTime 条件筛选掉已更新的数据)
                bluePrintDB = dbOwnedBluePrints.SingleOrDefault(p => p.UpdateTime != updateTime && p.ItemID == esiOwnedBluePrints[i].ItemID);

                // 若没有对应的数据, 则留待下次遍历处理
                if (bluePrintDB == null)
                    continue;

                // 虽然 ItemID 一样, 需通过确认蓝图 BluePrintID 属性是否相同来确认是否是同一个物品
                if (bluePrintDB.BluePrintID != esiOwnedBluePrints[i].BluePrintID)
                {
                    // 若不是同一个物品, 理论上也是将数据的字段更新一下就可以, 但若出现这个问题则属以外情况.
                    // continue;
                    // _updateAllPropertyFromESIData 中没有更新 BluePrintID

                    // 理论上不可能存在这种情况, 若出现,先用一个 MessageBox 提示
                    MessageBox.Show(string.Format("OwnedBluePrints.Update() 更新错误, 数据 ItemID:{0} 能匹配到, 但是物品类型不一样. ", bluePrintDB.ItemID));

                    // 出现错误, 更新失败
                    return false;
                }

                // 若是蓝图基本属性匹配不上
                if (bluePrintDB.TimeEfficiency != esiOwnedBluePrints[i].TimeEfficiency
                    || bluePrintDB.MaterialEfficiency != esiOwnedBluePrints[i].MaterialEfficiency
                    || bluePrintDB.Quantity != esiOwnedBluePrints[i].Quantity
                    || bluePrintDB.Runs != esiOwnedBluePrints[i].Runs)
                {
                    // 若是同一个物品, 但是物品基本属性变了, 理论上在用户确认后更新物品基本属性即可, 但必须尽量排除, 因为属于可以完全避免出现的情况
                    // TODO: Z 完善工业线程控制后需取消注释 OwnedBluePrints.Update() -> 蓝图基本属性不同时弹窗提示
                    // MessageBox.Show(string.Format("蓝图:{0}, ItemID:{1} 的数据库基本数据与ESI匹配不上.", bluePrintDB.BluePrints.invTypes.Name, bluePrintDB.ItemID), "注意");

                    continue;
                }

                // 用 ESI 蓝图的数据更新数据库蓝图的一般数据
                bluePrintDB._updateGeneralPropertyFromESIData(esiOwnedBluePrints[i]);

                // 统一设置数据更新时间
                bluePrintDB.UpdateTime = updateTime;

                // 更新完数据库蓝图数据后将数据从 ESI 蓝图列表中对应的对象删除
                esiOwnedBluePrints.Remove(esiOwnedBluePrints[i]);
            }

            // 若临时表中没有元素, 且数据库中的数据都已更新过, 则更新完成
            if (esiOwnedBluePrints.Count == 0 && dbOwnedBluePrints.Where(p => p.UpdateTime != updateTime).Count() == 0)
            {
                return true;
            }
            else
            {
                // 否则, 则必须进行用户确认

                // 新建用户确认窗体
                ESIAndDBConfirm winConfirm = new ESIAndDBConfirm();

                // 表头赋值
                winConfirm.DBListViewColumnHeaders = ListViewColumnHeaders();
                winConfirm.ESIListViewColumnHeaders = ListViewColumnHeaders();
                
                // 获取数据库数据中和 ESI 数据匹配不上, 无法更新的数据
                var dbUnMatchBluePrints = dbOwnedBluePrints.Where(p => p.UpdateTime != updateTime);

                // 若有, 则添加到确认窗体的显示列表中
                if (dbUnMatchBluePrints.Count() != 0)
                {
                    foreach (var bp in dbUnMatchBluePrints)
                    {
                        winConfirm.DBListViewItemCollection.Add(bp.ListViewItem());
                    }
                }

                // 若 ESI 数据中还有没更新到数据库的, 也显示到确认窗体中
                if (esiOwnedBluePrints.Count != 0)
                {
                    foreach (var bp in esiOwnedBluePrints)
                    {
                        winConfirm.ESIListViewItemCollection.Add(bp.ListViewItem());
                    }
                }

                // 确认窗体载入数据
                winConfirm.LoadListViewData();

                // 若用户确认变更
                if (winConfirm.ShowDialog() == DialogResult.OK)
                {
                    // 关掉窗体
                    winConfirm.Close();

                    // 先遍历 ESI 数据将 ItemID 相同得数据更新到数据库
                    for (int i = esiOwnedBluePrints.Count - 1; i >= 0; i--)
                    {
                        // 查找数据库中对应 ItemID 的蓝图数据 (p.UpdateTime != updateTime 条件筛选掉已更新的数据)
                        bluePrintDB = dbOwnedBluePrints.SingleOrDefault(p => p.UpdateTime != updateTime && p.ItemID == esiOwnedBluePrints[i].ItemID);

                        if (bluePrintDB != null)
                        {
                            // 更新数据库对象除了 ItemID 和 BluePrintID 以外的所有字段
                            bluePrintDB._updateAllPropertyFromESIData(esiOwnedBluePrints[i]);
                            bluePrintDB.UpdateTime = updateTime;

                            // 更新完数据库蓝图数据后将数据从 ESI 蓝图列表中对应的对象删除
                            esiOwnedBluePrints.RemoveAt(i);
                        }
                    }

                    // 若此时数据库中还有未更新到的蓝图数据, 则说明游戏中是没有该数据了, 要从数据库中删除
                    dbUnMatchBluePrints = dbOwnedBluePrints.Where(p => p.UpdateTime != updateTime);
                    if (dbUnMatchBluePrints.Count() != 0)
                        dc.OwnedBluePrints.DeleteAllOnSubmit(dbUnMatchBluePrints);

                    // 若 ESI 数据还没有匹配完, 剩下的就直接插入了
                    if (esiOwnedBluePrints.Count() != 0)
                        dc.OwnedBluePrints.InsertAllOnSubmit(esiOwnedBluePrints);

                    dc.SubmitChanges();

                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// 用ESI数据更新蓝图的全部属性(ItemID,BluePrintID除外)
        /// </summary>
        /// <param name="esiBluePrint"></param>
        private void _updateAllPropertyFromESIData(OwnedBluePrints esiBluePrint)
        {
            // 更新基本属性
            //BluePrintID = esiBluePrint.BluePrintID;
            TimeEfficiency = esiBluePrint.TimeEfficiency;
            MaterialEfficiency = esiBluePrint.MaterialEfficiency;
            Quantity = esiBluePrint.Quantity;
            Runs = esiBluePrint.Runs;

            // 更新一般属性
            _updateGeneralPropertyFromESIData(esiBluePrint);
        }

        /// <summary>
        /// 用 ESI 获取的蓝图数据更新数据库中的一般属性
        /// </summary>
        /// <param name="esiBluePrint">ESI数据</param>
        private void _updateGeneralPropertyFromESIData(OwnedBluePrints esiBluePrint)
        {
            Owner = esiBluePrint.Owner;
            CharacterOwned = esiBluePrint.CharacterOwned;
            LocationID = esiBluePrint.LocationID;
            LocationFlag = esiBluePrint.LocationFlag;
        }

        /// <summary>
        /// 将蓝图转换成一个要显示的 ListViewItem
        /// </summary>
        /// <returns></returns>
        public ListViewItem ListViewItem()
        {
            ListViewItem lvi = new ListViewItem();

            // 蓝图名称
            if (BluePrints == null)
                lvi.Text = invTypes.GetInvTypes(BluePrintID).Name;
            else
                lvi.Text = BluePrints.invTypes.Name;

            lvi.SubItems.Add(ID.ToString());
            lvi.SubItems.Add(ItemID.ToString());
            lvi.SubItems.Add(BluePrintID.ToString());
            lvi.SubItems.Add(MaterialEfficiency.ToString());
            lvi.SubItems.Add(TimeEfficiency.ToString());
            lvi.SubItems.Add(Quantity.ToString());
            lvi.SubItems.Add(Runs.ToString());

            // 如果是角色拥有则显示角色名称, 否则直接显示"军团"
            if (CharacterOwned)
            {
                if (Characters == null)
                    lvi.SubItems.Add(Characters.GetCharacters(Owner).CharacterName);
                else
                    lvi.SubItems.Add(Characters.CharacterName);
            }
            else
                lvi.SubItems.Add("军团");

            lvi.SubItems.Add(LocationID.ToString());
            lvi.SubItems.Add(LocationFlag.ToString());
            lvi.SubItems.Add(UpdateTime.ToString());
            lvi.SubItems.Add(Cost.ToString());

            return lvi;
        }

        /// <summary>
        /// 创建 ListView 的列显示效果
        /// </summary>
        /// <returns></returns>
        public static List<ColumnHeader> ListViewColumnHeaders()
        {
            List<ColumnHeader> columnHeaders = new List<ColumnHeader>();

            ColumnHeader ch = new ColumnHeader();
            ch.Text = "名称";
            ch.TextAlign = HorizontalAlignment.Center;
            columnHeaders.Add(ch);

            ch = new ColumnHeader();
            ch.Text = "ID";
            ch.TextAlign = HorizontalAlignment.Center;
            columnHeaders.Add(ch);

            ch = new ColumnHeader();
            ch.Text = "ItemID";
            ch.TextAlign = HorizontalAlignment.Center;
            columnHeaders.Add(ch);

            ch = new ColumnHeader();
            ch.Text = "BluePrintID";
            ch.TextAlign = HorizontalAlignment.Center;
            columnHeaders.Add(ch);

            ch = new ColumnHeader();
            ch.Text = "ME";
            ch.TextAlign = HorizontalAlignment.Center;
            columnHeaders.Add(ch);

            ch = new ColumnHeader();
            ch.Text = "TE";
            ch.TextAlign = HorizontalAlignment.Center;
            columnHeaders.Add(ch);

            ch = new ColumnHeader();
            ch.Text = "Quantity";
            ch.TextAlign = HorizontalAlignment.Center;
            columnHeaders.Add(ch);

            ch = new ColumnHeader();
            ch.Text = "Runs";
            ch.TextAlign = HorizontalAlignment.Center;
            columnHeaders.Add(ch);

            ch = new ColumnHeader();
            ch.Text = "拥有者";
            ch.TextAlign = HorizontalAlignment.Center;
            columnHeaders.Add(ch);

            ch = new ColumnHeader();
            ch.Text = "LocationID";
            ch.TextAlign = HorizontalAlignment.Center;
            columnHeaders.Add(ch);

            ch = new ColumnHeader();
            ch.Text = "LocationFlag";
            ch.TextAlign = HorizontalAlignment.Center;
            columnHeaders.Add(ch);

            ch = new ColumnHeader();
            ch.Text = "UpdateTime";
            ch.TextAlign = HorizontalAlignment.Center;
            columnHeaders.Add(ch);

            ch = new ColumnHeader();
            ch.Text = "Cost";
            ch.TextAlign = HorizontalAlignment.Center;
            columnHeaders.Add(ch);

            return columnHeaders;
        }

        public override string ToString()
        {
            return BluePrints.invTypes.Name + " " + MaterialEfficiency.ToString() + " " + TimeEfficiency.ToString();
        }
    }
}
