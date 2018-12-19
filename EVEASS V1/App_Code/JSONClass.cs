using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EVEASS_V1.ESI_Code
{
    // TODO:其他 ESI Class 的类型跟 ESI 网站上面确认.

    #region Characters

    public enum TokenType
    {
        RefreshToken,
        AuthorizationCode
    }

    public class ESITokenData
    {
        [JsonProperty("access_token")]
        public string AccessToken;

        [JsonProperty("token_type")]
        public string TokenType;

        [JsonProperty("expires_in")]
        public int ExpiresIn;

        [JsonProperty("refresh_token")]
        public string RefreshToken;
    }

    public class ESICharacterVerificationData
    {
        [JsonProperty("CharacterID")]
        public int CharacterID;
    }

    public class ESICharacterPublicData
    {
        [JsonProperty("name")]
        public string CharacterName;

        [JsonProperty("corporation_id")]
        public int CorporationID;
    }

    public class ESIBluePrint
    {
        [JsonProperty("item_id")]
        public long ItemID;

        [JsonProperty("type_id")]
        public int TypeID;

        [JsonProperty("location_id")]
        public long LocationID;

        [JsonProperty("location_flag")]
        public string LocationFlag;

        [JsonProperty("quantity")]
        public int Quantity;

        [JsonProperty("time_efficiency")]
        public int TimeEfficiency;

        [JsonProperty("material_efficiency")]
        public int MaterialEfficiency;

        [JsonProperty("runs")]
        public int Runs;
    }

    #endregion

    #region Industry

    public class ESIIndustryJob
    {
        [JsonProperty("activity_id")]           public int ActivityID;
        [JsonProperty("blueprint_id")]          public long BluePrintItemID;
        [JsonProperty("blueprint_location_id")] public long BluePrintLocationID;
        [JsonProperty("blueprint_type_id")]     public int BluePrintTypeID;
        [JsonProperty("completed_character_id")]public int CompletedCharacterID;
        [JsonProperty("completed_date")]        public string CompletedDate;
        [JsonProperty("cost")]                  public double Cost;
        [JsonProperty("duration")]              public int Duration;
        [JsonProperty("end_date")]              public string EndDate;
        [JsonProperty("facility_id")]           public long FacilityID;
        [JsonProperty("installer_id")]          public int InstallerID;
        [JsonProperty("job_id")]                public int JobID;
        [JsonProperty("licensed_runs")]         public int LicensedRuns;
        [JsonProperty("output_location_id")]    public long OutputLocationID;
        [JsonProperty("pause_date")]            public string PauseDate;
        [JsonProperty("probability")]           public float Probability;
        [JsonProperty("product_type_id")]       public int ProductTypeID;
        [JsonProperty("runs")]                  public int Runs;
        [JsonProperty("start_date")]            public string StartDate;
        [JsonProperty("station_id")]            public long StationID;
        [JsonProperty("status")]                public string Status;
        //  [ active(进行中), cancelled(取消), delivered(已交付), paused(暂停), ready(可交付), reverted ]
        [JsonProperty("successful_runs")]       public int SuccessfulRuns;
    }

    #endregion

    #region Market

    public class ESIMarketOrder
    {
        [JsonProperty("duration")] public int Duration;        // 订单持续时间
        [JsonProperty("is_buy_order")] public bool IsBuyOrder;     // 是否是购买订单
        [JsonProperty("issued")] public string Issued;       // 创建时间
        [JsonProperty("location_id")] public long LocationID;     // 订单位置
        [JsonProperty("min_volume")] public int MinVolume;       // 最小成交量
        [JsonProperty("order_id")] public long OrderID;        // 订单ID
        [JsonProperty("price")] public decimal Price;     // 订单价格
        [JsonProperty("range")] public string Range;       // 订单范围
        [JsonProperty("system_id")] public int SystemID;     // 订单星系ID
        [JsonProperty("type_id")] public int TypeID;       // 订单物品类型 ID
        [JsonProperty("volume_remain")] public int VolumeRemain;     // 订单剩余数量
        [JsonProperty("volume_total")] public int VolumeTotal;       // 订单总数量
    }

    public class ESIMarketPrice
    {
        [JsonProperty("type_id")] public int TypeID;       // 订单物品类型 ID
        [JsonProperty("adjusted_price")] public decimal AdjustedPrice;     // 建议价
        [JsonProperty("average_price")] public decimal AveragePrice;     // 平均价
    }

    #endregion

    #region Skill

    public class ESICharacterSkillList
    {
        [JsonProperty("skills")]
        public ESICharacterSkill[] SkillList;

        [JsonProperty("total_sp")]
        public int TotalSP;

        [JsonProperty("unallocated_sp")]
        public int UnAllocatedSP;
    }

    public class ESICharacterSkill
    {
        [JsonProperty("skill_id")]
        public int SkillID;

        [JsonProperty("skillpoints_in_skill")]
        public int SkillPoints;

        [JsonProperty("trained_skill_level")]
        public int TrainedSkillLevel;

        [JsonProperty("active_skill_level")]
        public int ActiveSkillLevel;
    }

    #endregion

    public class ESIError
    {
        [JsonProperty("error")]
        public string ErrorText;

        [JsonProperty("sso_status")]
        public int SSOStatus;

        [JsonProperty("timeout")]
        public int TimeOut;
    }
}
