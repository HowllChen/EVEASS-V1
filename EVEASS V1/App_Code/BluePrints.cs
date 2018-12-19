using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EVEASS_V1.App_Code;

namespace EVEASS_V1
{
    partial class BluePrints
    {
        private OwnedBluePrints _mappedBluePrint;

        /// <summary>
        /// 获取或设置一个值, 表示蓝图对象映射的用户蓝图
        /// </summary>
        public OwnedBluePrints MappedBluePrint
        {
            get
            {
                // 筛选空闲的蓝图中材料值最高的
                if (_mappedBluePrint == null)
                    _mappedBluePrint = OwnedBluePrints.OrderByDescending(p => p.MaterialEfficiency).FirstOrDefault(p => !p.IsUsing);

                return _mappedBluePrint;
            }

            set
            {
                _mappedBluePrint = value;
            }
        }

        public int MaterialEfficiency
        {
            get
            {
                if (MappedBluePrint == null)
                    return 0;
                else
                    return MappedBluePrint.MaterialEfficiency;
            }
        }

        public int TimeEfficiency
        {
            get
            {
                if (MappedBluePrint == null)
                    return 0;
                else
                    return MappedBluePrint.TimeEfficiency;
            }
        }

        #region Manufact

        private Manufact _manufact;

        /// <summary>
        /// 制造
        /// </summary>
        public Manufact Manufact
        {
            get
            {
                // 初始化
                if (_manufact == null)
                    _manufact = new Manufact(this);

                return _manufact;
            }
        }

        #endregion
    }
}
