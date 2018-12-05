using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace EVEASS_V1.ESI_Code
{
    public static class ESI
    {
        #region CONST

        private const string LocalHost = "127.0.0.1";
        private const string LocalPort = "12500";
        private const string ClientID = "c23892ca158945a7a0aed58b7529861b";
        private const string SecretKey = "reEyUvLbC105caLXEhVwniAVnpzBTFoNcrEYpJQy";
        private const string EncodeAppKey = "YzIzODkyY2ExNTg5NDVhN2EwYWVkNThiNzUyOTg2MWI6cmVFeVV2TGJDMTA1Y2FMWEVoVnduaUFWbnB6QlRGb05jckVZcEpReQ==";
        private const string ESITokenURL = "https://login.eveonline.com/oauth/token";
        private const string ESIAuthorizeURL = "https://login.eveonline.com/oauth/authorize";
        private const string ESIVerifyURL = "https://login.eveonline.com/oauth/verify";
        private const string ESIPublicURL = "https://esi.evetech.net/latest/";
        private const string DataSourceString = "?datasource=tranquility";
        private const string ScopesString = "esi-assets.read_assets.v1"
            + "%20esi-characters.read_standings.v1"
            + "%20esi-industry.read_character_jobs.v1"
            + "%20esi-characters.read_blueprints.v1"
            + "%20esi-assets.read_corporation_assets.v1"
            + "%20esi-corporations.read_blueprints.v1"
            + "%20esi-industry.read_corporation_jobs.v1"
            + "%20esi-skills.read_skills.v1";

        #endregion

        private static string _authStreamText;         // Web上回馈的信息
        private static TcpListener _tcpListener;       // 访问监听器
        //private static Characters _character;          // ESI 对应的角色

        public static bool CancelESISSOLogin = false;          // 取消登录  // TODO: 取消登录 

        #region GetESITokenData

        /// <summary>
        /// 获取 ESITokenData 数据
        /// </summary>
        /// <param name="token">访问密钥</param>
        /// <param name="tokenType">密钥类型, 默认为 RefreshToken</param>
        /// <returns></returns>
        public static ESITokenData GetESITokenData(string token, TokenType tokenType = TokenType.RefreshToken, bool retry = true)
        {
            byte[] response;
            WebClient wc = new WebClient();
            int errorCode = 0;
            string errorResponse = "";
            ESITokenData tokenDataToReturn = null;

            wc.Headers.Add(HttpRequestHeader.Authorization, string.Format("Basic {0}", EncodeAppKey));

            NameValueCollection postParameters = new NameValueCollection();

            switch (tokenType)
            {
                case TokenType.RefreshToken:
                    postParameters.Add("grant_type", "refresh_token");
                    postParameters.Add("refresh_token", token);
                    break;
                case TokenType.AuthorizationCode:
                    postParameters.Add("grant_type", "authorization_code");
                    postParameters.Add("code", token);
                    break;
                default:
                    break;
            }
            try
            {
                response = wc.UploadValues(ESITokenURL, "POST", postParameters);
                string data = Encoding.UTF8.GetString(response);
                tokenDataToReturn = JsonConvert.DeserializeObject<ESITokenData>(data);
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                    errorCode = (int)((HttpWebResponse)ex.Response).StatusCode;

                errorResponse = _getErrorResponseBody(ex);

                if (errorCode >= 500 && retry)
                {
                    Thread.Sleep(2000);
                    return GetESITokenData(token, tokenType, false);
                }

                MessageBox.Show("公共信息获取失败. 错误代码:" + errorCode.ToString() + ",错误信息:" + ex.Message + "-" + errorResponse);
            }
            catch (Exception ex)
            {
                MessageBox.Show("公共信息获取失败." + ex.Message);
            }

            return tokenDataToReturn;
        }

        /// <summary>
        /// 根据 RefreshToken 获取 AccessToken
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public static string GetAccessToken(string refreshToken)
        {
            return GetESITokenData(refreshToken).AccessToken;
        }

        #endregion

        #region GetAuthorizationToken

        public static string GetAuthorizationToken()
        {
            DateTime startTime = DateTime.Now;      // 记录开始时间, 防止窗口无限等待

            _authStreamText = "";       // 获取新数据前先把旧数据清除

            // 创建官网登录授权进程
            Thread authThread = new Thread(_getAuthorizationFromWeb);

            authThread.Start();

            do
            {
                // 等待超时
                if ((DateTime.Now - startTime).TotalSeconds > 60)
                {
                    MessageBox.Show("等待时间过长,必须在一分钟内完成登录.");
                    _tcpListener.Stop();
                    authThread.Abort();

                    return string.Empty;
                }
                else if (CancelESISSOLogin)
                {
                    MessageBox.Show("登录取消.");
                    _tcpListener.Stop();
                    authThread.Abort();

                    return string.Empty;
                }
                else if (!authThread.IsAlive)
                {
                    break;
                }
            } while (true);

            string[] arrAuthStreamText = _authStreamText.Split(' ');

            string code = "";
            foreach (var text in arrAuthStreamText)
            {
                if (text.Contains("/?code="))
                {
                    code = text.Substring(text.IndexOf("=") + 1);
                    break;
                }
            }

            return code;
        }

        /// <summary>
        /// 通过官网登录获取授权码
        /// </summary>
        private static void _getAuthorizationFromWeb()
        {
            string url = string.Format("{0}?response_type=code&redirect_uri=http://{1}:{2}&client_id={3}&scope={4}", ESIAuthorizeURL, LocalHost, LocalPort, ClientID, ScopesString);

            Process ieProcess = Process.Start("IEXPLORE.EXE", url);

            Socket mySocket;
            NetworkStream myStream;
            StreamReader myReader;
            StreamWriter myWriter;

            _tcpListener = new TcpListener(IPAddress.Parse(LocalHost), int.Parse(LocalPort));

            _tcpListener.Start();

            mySocket = _tcpListener.AcceptSocket();
            Debug.Print("After socket listen");
            myStream = new NetworkStream(mySocket);
            myReader = new StreamReader(myStream);
            myWriter = new StreamWriter(myStream);
            myWriter.AutoFlush = true;

            while (!myReader.EndOfStream)
            {
                _authStreamText += myReader.ReadLine() + "|";

                if (_authStreamText.Contains("code"))
                    break;
            }

            myWriter.Write("Login Successful! <\\br>You can close this window.");
            myWriter.Close();
            myReader.Close();
            myStream.Close();
            mySocket.Close();
            _tcpListener.Stop();
            ieProcess.CloseMainWindow();
        }

        #endregion

        #region GetCharacterData

        public static ESICharacterVerificationData GetESICharacterVerificationData(string accessToken)
        {
            WebClient wc = new WebClient();
            wc.Headers.Add(HttpRequestHeader.Authorization, string.Format("Bearer {0}", accessToken));
            var response = wc.DownloadString(ESIVerifyURL);

            int pageNum = string.IsNullOrEmpty(wc.ResponseHeaders["X-Pages"]) ? 1 : int.Parse(wc.ResponseHeaders["X-Pages"]);
            string expires = wc.ResponseHeaders["Expires"];

            if (pageNum > 1)
            {
                for (int i = 2; i <= pageNum; i++)
                {
                    string tempResponse = wc.DownloadString(ESIVerifyURL + "&page=" + i.ToString());
                    response += "," + tempResponse;
                }
            }

            return JsonConvert.DeserializeObject<ESICharacterVerificationData>(response);
        }

        /// <summary>
        /// 获取角色的公共信息
        /// </summary>
        /// <param name="characterID"></param>
        /// <returns></returns>
        public static ESICharacterPublicData GetCharacterPublicData(long characterID)
        {
            string response = _getPublicData(ESIPublicURL + "characters/" + characterID.ToString() + "/" + DataSourceString);

            return JsonConvert.DeserializeObject<ESICharacterPublicData>(response);
        }

        /// <summary>
        /// 获取角色的技能信息
        /// </summary>
        /// <param name="characterID"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static ESICharacterSkillList GetCharacterSkillList(long characterID, string accessToken)
        {
            string URL = string.Format("{0}characters/{1}/skills/{2}", ESIPublicURL, characterID, DataSourceString);

            string returnData = _getPrivateData(URL, accessToken);

            if (!string.IsNullOrEmpty(returnData))
                return JsonConvert.DeserializeObject<ESICharacterSkillList>(returnData);
            else
                return null;
        }

        #region GetBluePrintsData

        /// <summary>
        /// 获取角色或军团的蓝图信息
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="accessToken"></param>
        /// <param name="isCorporation"></param>
        /// <returns></returns>
        public static List<ESIBluePrint> GetBluePrints(long ID, string accessToken, bool isCorporation = false)
        {
            string URL = "";

            if (isCorporation)
                URL = string.Format("{0}corporations/{1}/blueprints/{2}", ESIPublicURL, ID, DataSourceString);
            else
                URL = string.Format("{0}characters/{1}/blueprints/{2}", ESIPublicURL, ID, DataSourceString);

            string returnData = _getPrivateData(URL, accessToken);

            if (!string.IsNullOrEmpty(returnData))
                return JsonConvert.DeserializeObject<List<ESIBluePrint>>(returnData);
            else
                return null;
        }

        #endregion

        #endregion

        #region GetESIData

        /// <summary>
        /// 获取ESI上的公共信息
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="bodyData"></param>
        /// <param name="retry"></param>
        /// <returns></returns>
        private static string _getPublicData(string URL, string bodyData = "", bool retry = true)
        {
            // 直接通过调用 AccessToken 为空的私有信息获取方法获取
            return _getPrivateData(URL, null, bodyData, retry);
        }

        /// <summary>
        /// 获取ESI上的私有信息
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="bodyData"></param>
        /// <param name="retry"></param>
        /// <returns></returns>
        private static string _getPrivateData(string URL,string accessToken = null, string bodyData = null, bool retry = true)
        {
            string response = "";
            WebClient wc = new WebClient();
            int errorCode = 0;
            string errorResponse = "";

            if (!string.IsNullOrEmpty(accessToken))
                wc.Headers.Add(HttpRequestHeader.Authorization, string.Format("Bearer {0}", accessToken));

            try
            {
                if (!string.IsNullOrEmpty(bodyData))
                    response = Encoding.UTF8.GetString(wc.UploadData(URL, Encoding.UTF8.GetBytes(bodyData)));
                else
                    response = wc.DownloadString(URL);

                WebHeaderCollection webHeaderCollection = wc.ResponseHeaders;
                int pages = -1;
                int.TryParse(webHeaderCollection["X-Pages"], out pages);

                //string expires = webHeaderCollection["Expires"];

                //if (!string.IsNullOrEmpty(expires))
                //{
                //    DateTime cachedate = DateTime.Parse(expires.Substring(expires.IndexOf(',') + 1)).ToLocalTime();   //.Replace("GMT", "")
                //}

                if (pages > 1)
                {
                    string tempResponse = "";
                    for (int i = 2; i <= pages; i++)
                    {
                        // TODO: 多线程?
                        tempResponse = wc.DownloadString(URL + "&page=" + i.ToString());
                        response = response.Substring(0, response.Length - 1) + "," + tempResponse.Substring(1);
                    }
                }

                return response;
            }
            catch (WebException ex)
            {
                errorCode = (int)((HttpWebResponse)ex.Response).StatusCode;
                errorResponse = _getErrorResponseBody(ex);

                if (errorResponse == "Character not in corporation" || errorResponse == "Character cannot grant roles")
                {
                    MessageBox.Show("该角色没有军团权限");
                    return null;
                }

                if (errorCode >= 500 && retry)
                {
                    Thread.Sleep(2000);
                    return _getPrivateData(URL, accessToken, bodyData, false);
                }

                MessageBox.Show("公共信息获取失败. 错误代码:" + errorCode.ToString() + ",错误信息:" + ex.Message + "-" + errorResponse);
            }
            catch (Exception ex)
            {
                MessageBox.Show("公共信息获取失败." + ex.Message);
            }

            return response;
        }

        #endregion

        #region Http

        /// <summary>
        /// 获取返回的Web信息的主体
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private static string _getErrorResponseBody(WebException ex)
        {
            try
            {
                string resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                ESIError errorData = JsonConvert.DeserializeObject<ESIError>(resp);

                if (errorData != null)
                    return errorData.ErrorText;
                else
                    return ex.Message;
            }
            catch (Exception)
            {
                return "Umknown Error";
            }
        }
        #endregion
    }
}
