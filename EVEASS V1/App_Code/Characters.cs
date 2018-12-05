using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EVEASS_V1.ESI_Code;

namespace EVEASS_V1
{
    public partial class Characters
    {
        public static Characters SignCharcher()
        {
            Characters characters = new Characters();

            string authCode = ESI.GetAuthorizationToken();

            if (string.IsNullOrEmpty(authCode))
                return null;

            ESITokenData etd = ESI.GetESITokenData(authCode, TokenType.AuthorizationCode);

            // 若是无法获取 ESIToken 则返回空 
            if (etd == null)
                return null;

            // 将AccessToken过期时间提前5s, 防止细微时间错位
            characters.AccessTokenExpiredTime = DateTime.Now.AddSeconds(etd.ExpiresIn - 5);
            characters.AccessToken = etd.AccessToken;
            characters.RefreshToken = etd.RefreshToken;

            ESICharacterVerificationData cvd = ESI.GetESICharacterVerificationData(characters.AccessToken);
            characters.CharacterID = cvd.CharacterID;

            ESICharacterPublicData cpd = ESI.GetCharacterPublicData(characters.CharacterID);
            characters.CharacterName = cpd.CharacterName;
            characters.CorporationID = cpd.CorporationID;

            return characters;
        }

        /// <summary>
        /// 更新 AccessToken
        /// </summary>
        public string GetAccessToken()
        {

            if (DateTime.Now > AccessTokenExpiredTime)
            {
                // 更新 AccessToken
                ESITokenData etd = ESI.GetESITokenData(RefreshToken);

                // 将AccessToken过期时间提前5s, 防止细微时间错位
                AccessTokenExpiredTime = DateTime.Now.AddSeconds(etd.ExpiresIn - 5);

                AccessToken = etd.AccessToken;
            }

            return AccessToken;
        }

        /// <summary>
        /// 清空并重新获取角色的技能数据(提交数据库)
        /// </summary>
        /// <returns>是否更新成功</returns>
        public bool UpdateSkills()
        {
            // 通过ESI获取角色的技能信息
            ESICharacterSkillList csl = ESI.GetCharacterSkillList(CharacterID, GetAccessToken());

            if (csl == null)
                return false;

            // 清空原有技能数据
            Main.DC.CharacterSkills.DeleteAllOnSubmit(CharacterSkills);
            Main.DC.SubmitChanges();
            
            var targetSkillID = Main.DC.invTypes.Where(p => CONST.TargetSkillsGroupID.Contains(p.groupID)).Select(p => p.typeID);

            CharacterSkills skill;

            // 只提取有用的技能数据且处理角色数据
            foreach (var s in csl.SkillList)
            {
                // 若技能 ID 不在要存储的技能ID队列里面, 则跳过
                if (!targetSkillID.Contains(s.SkillID))
                    continue;

                // 若是要存储的技能, 则创建对应的数据记录
                skill = new CharacterSkills()
                {
                    CharacterID = CharacterID,
                    SkillID = s.SkillID,
                    Level = s.TrainedSkillLevel
                };

                CharacterSkills.Add(skill);
            }

            Main.DC.SubmitChanges();

            // 分析角色能力
            // 制造能力
            ManuCapability = (from s in Main.DC.industryActivitySkills
                                  join c in Main.DC.CharacterSkills
                                  on s.skillID equals c.SkillID
                                  where c.CharacterID == CharacterID && s.activityID == CONST.ManufactActivityID && c.Level >= s.level
                                  select s.skillID).Count();

            NumManufactureLine = 1;
            var targetSkill = CharacterSkills.SingleOrDefault(p => p.SkillID == 3387);     // 批量生产学
            if (targetSkill != null)
                NumManufactureLine += targetSkill.Level;

            targetSkill = CharacterSkills.SingleOrDefault(p => p.SkillID == 24625);         // 高级量产技术
            if (targetSkill != null)
                NumManufactureLine += targetSkill.Level;

            // 科研能力
            ReseCapability = (from s in Main.DC.industryActivitySkills
                               join c in Main.DC.CharacterSkills
                               on s.skillID equals c.SkillID
                               where c.CharacterID == CharacterID && CONST.ResearchActivitiesID.Contains(s.activityID) && c.Level >= s.level
                               select s.skillID).Count();

            NumResearchLine = 1;
            targetSkill = CharacterSkills.SingleOrDefault(p => p.SkillID == 3406);     // 实验室运作理论
            if (targetSkill != null)
                NumResearchLine += targetSkill.Level;

            targetSkill = CharacterSkills.SingleOrDefault(p => p.SkillID == 24624);         // 高级实验室运作理论
            if (targetSkill != null)
                NumResearchLine += targetSkill.Level;

            // 市场订单量
            if (Market)
            {
                NumMarketOrder = 5;
                targetSkill = CharacterSkills.SingleOrDefault(p => p.SkillID == 3443);     // 贸易学
                if (targetSkill != null)
                    NumMarketOrder += targetSkill.Level * 4;

                targetSkill = CharacterSkills.SingleOrDefault(p => p.SkillID == 3444);     // 零售技巧
                if (targetSkill != null)
                    NumMarketOrder += targetSkill.Level * 8;

                targetSkill = CharacterSkills.SingleOrDefault(p => p.SkillID == 16596);     // 批发技巧
                if (targetSkill != null)
                    NumMarketOrder += targetSkill.Level * 16;

                targetSkill = CharacterSkills.SingleOrDefault(p => p.SkillID == 18580);     // 商业巨头
                if (targetSkill != null)
                    NumMarketOrder += targetSkill.Level * 32;
            }

            Main.DC.SubmitChanges();

            return true;
        }

        /// <summary>
        /// 获取角色和军团(若有权限)拥有的蓝图列表
        /// 蓝图不单独更新, 所有角色批量更新, 防止有蓝图给予等现象
        /// </summary>
        /// <returns></returns>
        public List<OwnedBluePrints> GetBluePrints()
        {
            DateTime updateTime = DateTime.Now;

            // 通过ESI获取角色拥有的蓝图列表
            List<ESIBluePrint> lstBluePrint = ESI.GetBluePrints(CharacterID, GetAccessToken());

            // 若获取失败, 则返回空值
            if (lstBluePrint == null)
                return null;

            List<OwnedBluePrints> lstOwnedBluePrints = new List<OwnedBluePrints>();
            OwnedBluePrints ownedBluePrints;

            // 将ESI获得的数据转换为Linq对象
            foreach (var bp in lstBluePrint)
            {
                ownedBluePrints = new OwnedBluePrints()
                {
                    ItemID = bp.ItemID,
                    BluePrintID = bp.TypeID,
                    Owner = CharacterID,
                    CharacterOwned = true,
                    LocationID = bp.LocationID,
                    LocationFlag = bp.LocationFlag,
                    Quantity = bp.Quantity,
                    TimeEfficiency = bp.TimeEfficiency,
                    MaterialEfficiency = bp.MaterialEfficiency,
                    Runs = bp.Runs,
                    UpdateTime = updateTime
                };

                lstOwnedBluePrints.Add(ownedBluePrints);
            }

            // 如果角色有军团权限, 则获取军团拥有的蓝图
            if (Corporation)
            {
                lstBluePrint = ESI.GetBluePrints(CharacterID, GetAccessToken(), true);

                foreach (var bp in lstBluePrint)
                {
                    ownedBluePrints = new OwnedBluePrints()
                    {
                        ItemID = bp.ItemID,
                        BluePrintID = bp.TypeID,
                        Owner = CharacterID,
                        CharacterOwned = false,
                        LocationID = bp.LocationID,
                        LocationFlag = bp.LocationFlag,
                        Quantity = bp.Quantity,
                        TimeEfficiency = bp.TimeEfficiency,
                        MaterialEfficiency = bp.MaterialEfficiency,
                        Runs = bp.Runs,
                        UpdateTime = updateTime
                    };

                    lstOwnedBluePrints.Add(ownedBluePrints);
                }
            }

            return lstOwnedBluePrints;
        }
    }
}
