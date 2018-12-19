using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using EVEASS_V1.ESI_Code;

namespace EVEASS_V1
{
    partial class invTypes
    {
        /// <summary>
        /// 获取 invTypes 对象, 传入DC实例方便后期更新数据
        /// </summary>
        /// <param name="typeID"></param>
        /// <param name="dc"></param>
        /// <returns></returns>
        public static invTypes GetInvTypes(int typeID)
        {
            DCEVEDataContext dc = new DCEVEDataContext();
            return dc.invTypes.Single(p => p.typeID == typeID);
        }

        #region Name

        /// <summary>
        /// 默认返回中文名称，没有中文名称返回英文名称
        /// </summary>
        public string Name
        {
            get
            {
                if (typeNameCN != null)
                    return typeNameCN;

                return typeName;
            }
        }

        #endregion

        #region Manufactable

        private bool _manufactable = true;

        /// <summary>
        /// 获取一个值, 表示物品是否可制造
        /// </summary>
        public bool Manufactable
        {
            get
            {
                // 若为 false 则已判断过不可以制造
                if (_manufactable)
                {
                    if (_getManufactBluePrint() == null)
                        _manufactable = false;
                }

                return _manufactable;
            }
        }

        private BluePrints _manufactBluePrint = null;

        /// <summary>
        /// 获取一个值, 表示物品的制造蓝图
        /// </summary>
        public BluePrints ManufactBluePrint
        {
            get
            {
                if (Manufactable)
                    return _manufactBluePrint;
                else
                    return null;
            }
        }

        /// <summary>
        /// 获取该产品的制造蓝图
        /// </summary>
        /// <returns></returns>
        private BluePrints _getManufactBluePrint()
        {
            if (!_manufactable)
                return null;

            if (_manufactBluePrint != null)
                return _manufactBluePrint;

            DCEVEDataContext dc = new DCEVEDataContext();

            // 查找相应的生产记录
            var iap = dc.industryActivityProducts.SingleOrDefault(p => p.activityID == 1 && p.productTypeID == typeID);

            if (iap != null)
                return _manufactBluePrint = iap.BluePrints;
            else
                return null;
        }

        #endregion

        #region MarketPriceOfLocation

        private MarketPrice _marketPriceOfLocation;

        /// <summary>
        /// 获取一个值, 表示与当前位置对应的价格数据
        /// </summary>
        public MarketPrice MarketPriceOfLocation
        {
            get
            {
                // 若 _marketPriceOfLocation 为空, 或者位置对应不正确, 则更新一下
                if (_marketPriceOfLocation == null || _marketPriceOfLocation.LocationID != Main.MarketLocation.LocationID)
                    _marketPriceOfLocation = MarketPrice.SingleOrDefault(p => p.LocationID == Main.MarketLocation.LocationID);

                _checkMarketPrice(Main.MarketLocation);

                Console.WriteLine("----物品[{0}]的最低出售价为:{1}", Name, _marketPriceOfLocation.MinSellPrice);
                return _marketPriceOfLocation;
            }
        }

        /// <summary>
        /// TypeID 和信号灯字典, 记录物品更新时的信号灯
        /// </summary>
        private static Dictionary<int, ManualResetEvent> TypeIDSignalPairs = new Dictionary<int, ManualResetEvent>();

        /// <summary>
        /// 获取物品在某个位置的价格数据
        /// </summary>
        /// <param name="locationID"></param>
        /// <returns></returns>
        private void _checkMarketPrice(Locations locations)
        {
            // 记录是否更新中
            bool updating;

            // 检查信号灯
            ManualResetEvent signal = _getMarketPriceUpdateSignal(locations, out updating);

            // 若信号灯非空, 则说明数据需要更新
            if (signal != null)
            {
                if (updating)
                {
                    // 若数据已在更新中, 则直接等数据更新完毕
                    Console.WriteLine("----等待物品[{0}]更新价格", Name);
                    signal.WaitOne();
                    Console.WriteLine("----等到物品[{0}]更新价格", Name);
                }
                else
                {
                    // 若数据还没更新, 则执行更新操作
                    Console.WriteLine("----更新物品[{0}]的价格", Name);
                    UpdateMarketPrice(locations);

                    // 更新完通过信号灯通知
                    signal.Set();

                    // 删除对应的信号灯
                    // _getMarketPriceUpdateSignal(locations, out updating);

                    Console.WriteLine("----完成物品[{0}]的价格更新", Name);
                }

                _marketPriceOfLocation = (new DCEVEDataContext()).MarketPrice.Single(p => p.LocationID == locations.LocationID && p.TypeID == typeID);
            }
        }

        /// <summary>
        /// 从 ESI 获取价格数据更新本地数据库
        /// </summary>
        /// <param name="location"></param>
        private void UpdateMarketPrice(Locations location)
        {
            DCEVEDataContext dc = new DCEVEDataContext();

            MarketPrice mp;

            if (_marketPriceOfLocation == null)
            {
                // 若该字段为空则说明数据库还没有此数据, 新建一条
                mp = new MarketPrice() { TypeID = typeID, LocationID = location.LocationID, ExpiresDateTime = DateTime.MinValue };
                dc.MarketPrice.InsertOnSubmit(mp);
            }
            else
            {
                // 若不为空则从新的 DataContext 获取该数据
                mp = dc.MarketPrice.Single(p => p.TypeID == typeID && p.LocationID == location.LocationID);
            }

            // 数据过期时间
            DateTime expriesDateTime = new DateTime();

            // 从 ESI 获取价格数据
            IEnumerable<ESIMarketOrder> marketOrders = ESI.GetMarketOrders(mp.TypeID, location.RegionID, ref expriesDateTime);

            // 仅获取对应位置的数据
            marketOrders = marketOrders.Where(p => p.LocationID == mp.LocationID);

            if (marketOrders.Where(p => p.IsBuyOrder).Count() == 0)
                mp.MaxBuyPrice = 0;
            else
                mp.MaxBuyPrice = marketOrders.Where(p => p.IsBuyOrder).Max(p => p.Price);

            // TODO: 当售价为 0 时, 即没有出售单时, 应该做些处理
            // 设置 PriceAsProduct 和 PriceAsMaterial 属性, 根据不同情况做不同处理
            if (marketOrders.Where(p => !p.IsBuyOrder).Count() == 0)
                mp.MinSellPrice = 0;
            else
                mp.MinSellPrice = marketOrders.Where(p => !p.IsBuyOrder).Min(p => p.Price);

            // 数据过期时间
            mp.ExpiresDateTime = expriesDateTime;

            // 更新数据库
            dc.SubmitChanges();
        }

        /// <summary>
        /// 检查价格数据状态, 如果更新中或者需要更新则返回信号灯, 数据正常则直接返回空值
        /// </summary>
        /// <param name="locations">位置</param>
        /// <param name="updating">需要更新则返回为 true, 否则为 false</param>
        /// <returns></returns>
        private ManualResetEvent _getMarketPriceUpdateSignal(Locations locations, out bool updating)
        {
            // 将该代码段加锁, 确保一次只有一个线程在执行此代码段
            // 即一次只有一个线程在操作信号灯字典
            lock (TypeIDSignalPairs)
            {
                // 检查是否有对应位置价格数据, 且该数据是否已过期
                if (_marketPriceOfLocation == null || _marketPriceOfLocation.ExpiresDateTime.AddMinutes(Main.ESIDateUpdateTimeSpan) <= DateTime.Now)
                {
                    // 若没有对应数据, 或者数据已过期, 则需要更新

                    ManualResetEvent signal;

                    // 通过信号灯字典获取信号灯看该类型的数据是否在更新中
                    if (!TypeIDSignalPairs.TryGetValue(typeID, out signal))
                    {
                        // 若找不到信号灯, 则说明数据不是在更新中

                        updating = false;       // 不是更新中

                        // 新建信号灯并添加到信号灯字典中
                        TypeIDSignalPairs.Add(typeID, signal = new ManualResetEvent(false));

                        // 返回信号灯
                        return signal;
                    }
                    else
                    {
                        // 若找到信号灯, 则说明数据正在更新中
                        updating = true;        // 更新中

                        // 直接返回信号灯
                        return signal;
                    }
                }
                else
                {
                    // 若数据为正确数据, 则看看是否有过期信号灯, 有则移除
                    if (TypeIDSignalPairs.ContainsKey(typeID))
                        TypeIDSignalPairs.Remove(typeID);

                    updating = false;       // 不是更新中

                    return null;        // 返回空值
                }
            }
        }

        #endregion
    }
}
