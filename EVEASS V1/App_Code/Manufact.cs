using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVEASS_V1.App_Code
{
    public class Manufact
    {
        public Manufact(BluePrints bp)
        {
            BluePrints = bp;
        }

        private void _initializeAllProperties()
        {
            _componentsList = new List<industryActivityMaterials>();
            _rawMaterialsList = new List<industryActivityMaterials>();
            _manufactFee = 0;
        }

        private BluePrints BluePrints;

        // TODO: F材料等级及设施减免未考虑, 参考一下外国佬 APP 的做法

        #region Runs

        private int _runs = 1;

        public int Runs
        {
            get
            {
                return _runs;
            }
            set
            {
                if (_runs != value)
                {
                    _runs = value;

                    // 将所有属性设为初始值, 则获取时会重新更新
                    _initializeAllProperties();
                }
            }
        }

        #endregion

        #region ComponentsList

        private List<industryActivityMaterials> _componentsList = new List<industryActivityMaterials>();

        /// <summary>
        /// 获取一个值, 表示制造需要的组件清单
        /// </summary>
        public List<industryActivityMaterials> ComponentsList
        {
            get
            {
                // 若字段为被设为空(没有组件), 或未初始化, 则初始化
                if (_componentsList != null & _componentsList.Count == 0)
                    _updateComponentsList();

                return _componentsList;
            }
        }

        /// <summary>
        /// 初始化蓝图组件清单
        /// </summary>
        private void _updateComponentsList()
        {
            // 若是 T1 则没有组件清单
            if (BluePrints.MetaGroupID == 1)
            {
                _componentsList = null;
                return;
            }

            // 清空原有材料清单
            _componentsList.Clear();

            // 将蓝图的材料清单直接转化成组件清单
            _componentsList.AddRange(_creatIndustryActivityMaterialsList());
        }

        /// <summary>
        /// 将数据库对象的材料清单转化成自建的对象清单
        /// </summary>
        /// <returns></returns>
        private List<industryActivityMaterials> _creatIndustryActivityMaterialsList()
        {
            List<industryActivityMaterials> list = new List<industryActivityMaterials>();

            industryActivityMaterials iam;
            foreach (var m in BluePrints.industryActivityMaterials.Where(p => p.activityID == 1))
            {
                // 克隆对象
                iam = m.Clone();
                iam.quantity = (int)Math.Ceiling(iam.quantity * (1 - 0.01 * BluePrints.MaterialEfficiency) * _runs);

                list.Add(iam);
            }

            return list;
        }

        public decimal TotalComponentsCost
        {
            get
            {
                if (ComponentsList == null)
                    return 0;

                return ComponentsList.Sum(p => p.quantity * p.invTypeMaterial.MarketPriceOfLocation.MinSellPrice);
            }
        }

        #endregion

        #region RawMaterialsList
        // TOFO: Z 暂时假设最多只有两层制造

        private List<industryActivityMaterials> _rawMaterialsList = new List<industryActivityMaterials>();
        public List<industryActivityMaterials> RawMaterialsList
        {
            get
            {
                if (_rawMaterialsList.Count == 0)
                    _updateRawMaterialsList();

                return _rawMaterialsList;
            }
        }

        private void _updateRawMaterialsList()
        {
            if (ComponentsList == null)
            {
                // 直接将蓝图的材料清单转化成材料清单
                _rawMaterialsList.AddRange(_creatIndustryActivityMaterialsList());
            }
            else
            {
                BluePrints materialBluePrint;
                foreach (var c in ComponentsList)
                {
                    // 获取组件材料的制造蓝图
                    materialBluePrint = c.invTypeMaterial.ManufactBluePrint;

                    // 判断组件是否可制造
                    // 且材料蓝图的产品市场售价大于制造成本
                    // TODO: 未考虑单线日最低利润 Manufact._updateRawMaterialsList()
                    if (c.invTypeMaterial.Manufactable && materialBluePrint.Manufact.ProductValue >= materialBluePrint.Manufact.TotalCost)
                    {
                        // 向上取整设置组件制造需要的流程数
                        materialBluePrint.Manufact.Runs = (int)Math.Ceiling((decimal)c.quantity / materialBluePrint.Manufact.ProductQuantity);

                        // 统计组件材料
                        _rawMaterialsList.AddMaterial(materialBluePrint.Manufact.RawMaterialsList);

                        // 计算组件制造费用
                        ComponentManufactFee += materialBluePrint.Manufact.ManufactFee;
                    }
                    else
                    {
                        // 若组件材料不可制造, 则说明为原材料, 直接添加
                        // 若组件制造成本大于产品售价, 也直接采购添加材料
                        _rawMaterialsList.AddMaterial(c);
                    }
                }
            }
        }

        #endregion

        public decimal TotalCost
        {
            get
            {
                return 0;
            }
        }

        #region ManufactFee

        private decimal _manufactFee = 0;

        public decimal ManufactFee
        {
            get
            {
                if (_manufactFee == 0)
                {
                    // TODO: 制造费用
                }
                return _manufactFee;
            }
        }

        public decimal ComponentManufactFee { get; private set; } = 0;

        #endregion

        #region MarketFee

        public decimal MarketFee
        {
            get;
        }

        #endregion

        #region Product

        private invTypes _product;

        /// <summary>
        /// 获取一个值, 表示制造产品
        /// </summary>
        public invTypes Product
        {
            get
            {
                if (_product == null)
                    _product = BluePrints.industryActivityProducts.Single(p => p.activityID == 1).invTypes;

                return _product;
            }
        }

        private int _quantity = -1;

        /// <summary>
        /// 获取一个值, 表示制造产品数量
        /// </summary>
        public int ProductQuantity
        {
            get
            {
                if (_quantity == -1)
                    _quantity = BluePrints.industryActivityProducts.Single(p => p.activityID == 1).quantity;

                return _quantity;
            }
        }

        /// <summary>
        /// 获取一个值, 表示制造成品的价值
        /// </summary>
        public decimal ProductValue
        {
            get
            {
                return Product.MarketPriceOfLocation.MinSellPrice * ProductQuantity;
            }
        }

        #endregion
    }
}
