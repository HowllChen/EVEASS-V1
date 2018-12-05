using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EVEASS_V1.ESI_Code
{
    #region CharacterDataClass

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
        public long CharacterID;
    }

    public class ESICharacterPublicData
    {
        [JsonProperty("name")]
        public string CharacterName;

        [JsonProperty("corporation_id")]
        public int CorporationID;
    }

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

    #region BluePrint

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
