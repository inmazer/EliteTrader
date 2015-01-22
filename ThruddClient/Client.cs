using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using EliteTrader.EliteOcr.Enums;
using EliteTrader.EliteOcr.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ThruddClient
{

    /*
     * Add Station:
     * POST http://www.elitetradingtool.co.uk/Admin/Station HTTP/1.1
Host: www.elitetradingtool.co.uk
Connection: keep-alive
Content-Length: 306
Origin: http://www.elitetradingtool.co.uk
X-Requested-With: XMLHttpRequest
User-Agent: Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36
Content-Type: application/json
Referer: http://www.elitetradingtool.co.uk/Admin/
Accept-Encoding: gzip, deflate
Accept-Language: en-US,en;q=0.8,da;q=0.6,nb;q=0.4,sv;q=0.2
Cookie: __RequestVerificationToken=_PRcL13C7hVPAT0CWWimiF6EzWOo9BB_yhXcrOBvy0CU3gwcF7vAMs7YpvGqk-Dst3Us_G-5K7mLqxO5kkn-AlMNoZBoSvWYFzEj9u-l7y41; _ga=GA1.3.139030831.1419415951; .AspNet.ApplicationCookie=mcItAM_1Y-xcfDZKhF4il_YIcHJNkAmUvoYwrBDdZOODNBM7dLbzCDxuWmrFY_ixu5wy9ZuZfQWth0hJGA3-AF9ByKsDV1F2UdzEZppjBfJ2X25skEv3KhntccflVoCDVTUk4RiSKBQMkLHqApNge3l7tXlWwbfdMWsYphQ2q0qOixUADwHqZ2Dmv2Ee6CbkwYMXoF50v7sql7Bapem8n5KP54m2wk5nObGDvjIbdAhuhsc-eyDOqaXRAtVzFAA8feV_7dy0TjHIN3tvc38L8qxOHNZp-dz0LMsbvIGrpn8KkNwa6ykCTKt6wAz3nk9E03DHBC6VOsvQpaW0vVH_CAW1g-t8N5B0PkdhOHcYTSfxzQ9y2zRKgWbNujpU3n3-0Fezr3nGKkKBumpHFeocgacuWTyFNy-ZpcjCfiOUzaSRs4IV2pQqh7aGEQOdujcrmng9E7bDHVCVkuCiqFLxcWqxVzTuSiPZ4Hv61JIZlA7SndJIOFFPqwv_Jr73u3QlX4mJvyohI7V7bKFp8OvnEi429gAVr1ktD7Zq37vquDHnadeWUcVsMat3sCRzIkMVr7cJmLlRwgjaRuml8E2W3F22K26ovpFrFVQeJ5CgCVo

{"Id":"7738","SystemId":"47662","Name":"Ball Orbital","StationTypeId":"1","HasBlackmarket":false,"HasMarket":true,"HasOutfitting":true,"HasShipyard":true,"HasRepairs":true,"AllegianceId":"2","GovernmentId":"12","EconomyId":"2","SecondaryEconomyId":"4","DistanceFromJumpIn":"15","CopyMarketFromStationId":0}
     * 
     * 
     * HTTP/1.1 200 OK
Date: Sat, 17 Jan 2015 10:07:23 GMT
Server: Microsoft-IIS/8.5
Cache-Control: private, s-maxage=0
Content-Type: application/json; charset=utf-8
X-AspNetMvc-Version: 5.2
X-AspNet-Version: 4.0.30319
X-Powered-By: ASP.NET
Content-Length: 182
Connection: close

{"Result":true,"Message":"Station Added","RepResult":{"CommanderName":"inmazer","Reputation":38015,"ReputationNeeded":319961985,"Title":"Deadly","RankUp":false,"Badge":"deadly.png"}}
     * 
     */


    /*
     * Delete station:
     * POST http://www.elitetradingtool.co.uk/Admin/DeleteStation HTTP/1.1
Host: www.elitetradingtool.co.uk
Connection: keep-alive
Content-Length: 11
Origin: http://www.elitetradingtool.co.uk
X-Requested-With: XMLHttpRequest
User-Agent: Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36
Content-Type: application/json
Referer: http://www.elitetradingtool.co.uk/Admin/
Accept-Encoding: gzip, deflate
Accept-Language: en-US,en;q=0.8,da;q=0.6,nb;q=0.4,sv;q=0.2
Cookie: __RequestVerificationToken=_PRcL13C7hVPAT0CWWimiF6EzWOo9BB_yhXcrOBvy0CU3gwcF7vAMs7YpvGqk-Dst3Us_G-5K7mLqxO5kkn-AlMNoZBoSvWYFzEj9u-l7y41; _ga=GA1.3.139030831.1419415951; .AspNet.ApplicationCookie=EsaF8Qo9xmX3B9b8UOmz3qEjkRmuIb8-t3hW1dj779ECy64tbpu4NJgFblK2ZGM9s9LqEN1xGoSWaX5iYU4vJDjla7Cq3co7x6UWvVH1fbJetBiG0P5-qfZcF5iJLSzS7314pOgiNBSLMmIONboCj_4ChJUVXYGgdyhCrVW7Vit8sakrKOZiOR47jGm6qJmjK-_t4oapOWpl9aCTdZ0ll6mQkfCUYBKiAb48xPkJbtMqs2lRzC_a-hjUJLt-8JqpBTWV4Ug70TZkNekuB4c5Ol76Fc1qQUFA2eIXqXdFpzNlCYJA1he9WI0Oa4BVk8bMc-4TvnmDMCyV7E7pGD6o7PqMmX6pe47O9_cZJJa9jst1RZnxyFhT3M9pTEIXpKIg9KnriQ5EZHAnZaFvDfdoelKZ09Ru3ow20T_PoHYEgkCzLj7EVtV65ujVbT7fvBlRLApX7mj1m_haKKLzl8xVMGiHW2Fnv3ve7Lt4SeUPrPWYrGuPcV1-nJ0DdqitXAotG_6-c79MFWQcivxAWGovdmR5wF9J9BnSR3czSbKKOmbcYcKwlLlqB9lv3TQhp7ThQcrX7dqpVKZgZQnX6K-nMji-IKe1pWBOswpBrujJ8lQ

{"Id":7738}
     * 
     * 
     * HTTP/1.1 200 OK
Date: Sat, 17 Jan 2015 11:11:36 GMT
Server: Microsoft-IIS/8.5
Cache-Control: private, s-maxage=0
Content-Type: application/json; charset=utf-8
X-AspNetMvc-Version: 5.2
X-AspNet-Version: 4.0.30319
X-Powered-By: ASP.NET
Content-Length: 60
Connection: close

{"Result":true,"Message":"Station deleted","RepResult":null}
     */

    public class ClientConfig
    {
        public int Timeout { get; private set; }

        public ClientConfig(int timeout)
        {
            Timeout = timeout;
        }
    }

    public class Client : IDisposable
    {
        private const string ReqVerCookieName = "__RequestVerificationToken";
        private const string AuthCookieName = ".AspNet.ApplicationCookie";
        
        private static readonly Uri BaseUri = new Uri("http://www.elitetradingtool.co.uk/");
        private static readonly Uri LoginPageUri = new Uri(BaseUri, "Account/Login");
        private static readonly Uri LogoutPageUri = new Uri(BaseUri, "Account/LogOff");
        private static readonly Uri FindTradesUri = new Uri(BaseUri, "api/EliteTradingTool/FindTrades");

        private static readonly Uri AdminSearchUri = new Uri(BaseUri, "Admin/SelectorData");
        private static readonly Uri AdminSystemsUri = new Uri(BaseUri, "Admin/System");
        private static readonly Uri AdminSystemStationsUri = new Uri(BaseUri, "Admin/SystemStations");
        private static readonly Uri AdminStationCommoditiesUri = new Uri(BaseUri, "Admin/StationCommodities");
        private static readonly Uri AdminUpdateCommodityUri = new Uri(BaseUri, "Admin/UpdateCommodity");
        private static readonly Uri AdminConfirmCommodityUri = new Uri(BaseUri, "Admin/ConfirmStationCommodity");
        private static readonly Uri AdminGetSelectListsUri = new Uri(BaseUri, "Admin/GetSelectLists");
        private static readonly Uri AdminAddCommodityUri = new Uri(BaseUri, "Admin/AddEditStationCommodity");
        private static readonly Uri AdminDeleteCommodityUri = new Uri(BaseUri, "Admin/DeleteStationCommodity");

        private ConnectionInfo _connectionInfo;
        private readonly ClientConfig _clientConfig;

        private readonly ILogger _logger;

        static Client()
        {
            ServicePointManager.Expect100Continue = false;
        }

        public Client(ILogger logger, ClientConfig clientConfig = null)
        {
            _logger = logger;
            _clientConfig = clientConfig ?? new ClientConfig(60000);
        }

        public GetSelectListsResult GetSelectLists()
        {
            _logger.Log(string.Format("-------- GetSelectLists ---------"));
            
            return DoGetRequest<GetSelectListsResult>(AdminGetSelectListsUri);
        }

        public void DeleteCommodity(int stationCommodityId, EnumCommodityCategory category)
        {
            string str = string.Format("{{\"id\":{0},\"categoryId\":\"{1}\"}}", stationCommodityId, (int)category);

            byte[] bytes = Encoding.UTF8.GetBytes(str);

            _logger.Log(string.Format("-------- DeleteCommodity ---------"));

            DoPostRequest(AdminDeleteCommodityUri, bytes, "application/json", false);
        }

        public void AddCommodity(int stationId, EnumCommodityItemName commodity, int buy, int sell, EnumCommodityCategory category)
        {
            string str = string.Format("{{\"Id\":0,\"StationId\":{0},\"CommodityId\":\"{1}\",\"Buy\":\"{2}\",\"Sell\":\"{3}\",\"CategoryId\":\"{4}\"}}", stationId, (int)commodity, buy, sell, (int)category);

            byte[] bytes = Encoding.UTF8.GetBytes(str);

            _logger.Log(string.Format("-------- AddCommodity ---------"));

            DoPostRequest(AdminAddCommodityUri, bytes, "application/json", false);
        }

        public void ConfirmCommodity(int stationCommodityId, EnumCommodityCategory category)
        {
            string str = string.Format("{{\"id\":{0},\"categoryId\":\"{1}\"}}", stationCommodityId, (int)category);

            byte[] bytes = Encoding.UTF8.GetBytes(str);

            _logger.Log(string.Format("-------- ConfirmCommodity ---------"));

            DoPostRequest(AdminConfirmCommodityUri, bytes, "application/json", false);
        }

        public UpdateCommodityResponse UpdateCommodity(int stationCommodityId, EnumCommodityAction action, int value)
        {
            string str = string.Format("StationCommodityId={0}&Action={1}&Value={2}", stationCommodityId, action == EnumCommodityAction.Sell ? "sell" : "buy", value);

            byte[] bytes = Encoding.UTF8.GetBytes(str);

            _logger.Log(string.Format("-------- UpdateCommodity ---------"));

            string responseStr = DoPostRequest(AdminUpdateCommodityUri, bytes, "application/x-www-form-urlencoded; charset=UTF-8", true);

            return JsonConvert.DeserializeObject<UpdateCommodityResponse>(responseStr);
        }

        public List<AdminSearchResultItem> DoAdminSearchQuery(string query)
        {
            _logger.Log(string.Format("-------- DoAdminSearchQuery ---------"));

            return DoGetRequest<List<AdminSearchResultItem>>(AdminSearchUri, "query", query);
        }

        public SystemData GetSystemData(int systemId)
        {
            _logger.Log(string.Format("-------- GetSystemData ---------"));

            return DoGetRequest<SystemData>(AdminSystemsUri, "id", systemId.ToString());
        }

        public SystemStationsData GetSystemStationsData(int systemId)
        {
            _logger.Log(string.Format("-------- GetSystemStationsData ---------"));

            return DoGetRequest<SystemStationsData>(AdminSystemStationsUri, "id", systemId.ToString());
        }

        public StationCommoditiesResult GetStationCommodities(int stationId)
        {
            _logger.Log(string.Format("-------- GetStationCommodities ---------"));

            return DoGetRequest<StationCommoditiesResult>(AdminStationCommoditiesUri, new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("id", stationId.ToString()), new KeyValuePair<string, string>("categoryId", "0") });
        }

        public void FindTrades(FindTradesInfo findTradesInfo)
        {
            _logger.Log(string.Format("-------- FindTrades ---------"));

            DoPostRequest(FindTradesUri, findTradesInfo, false);
        }

        public void Login(ThruddCredentials credentials)
        {
            Login(credentials.Username, credentials.Password);
        }

        public void Login(string username, string password)
        {
            try
            {
                if (_connectionInfo != null)
                {
                    Logout();
                }

                _logger.Log(string.Format("-------- Login ---------"));
                _logger.Log(string.Format("Creating request for ({0})", LoginPageUri));
                HttpWebRequest firstRequest = WebRequest.CreateHttp(LoginPageUri);
                AddStandardHeaders(firstRequest);
                firstRequest.CookieContainer = new CookieContainer();
                firstRequest.Timeout = _clientConfig.Timeout;

                _logger.Log("Getting response");
                string reqVerToken;
                using (HttpWebResponse firstRequestResponse = (HttpWebResponse) firstRequest.GetResponse())
                {
                    _logger.Log(string.Format("Got status code ({0})", firstRequestResponse.StatusCode));
                    if (firstRequestResponse.StatusCode != HttpStatusCode.OK)
                    {
                        using (Stream stream = firstRequestResponse.GetResponseStream())
                        {
                            string streamContent = "<null>";
                            if (stream != null)
                            {
                                StreamReader sr = new StreamReader(stream, Encoding.UTF8);
                                streamContent = sr.ReadToEnd();
                            }

                            throw new Exception(
                                string.Format("First page load got status code ({0}). The stream content was:\r\n{1}",
                                    firstRequestResponse.StatusCode, streamContent));
                        }
                    }

                    Cookie reqVerCookie = firstRequestResponse.Cookies[ReqVerCookieName];
                    if (reqVerCookie == null)
                    {
                        throw new Exception(string.Format("Missing expected {0} cookie", ReqVerCookieName));
                    }


                    reqVerToken = reqVerCookie.Value;
                }

                _logger.Log("Successfully got request verification token");

                string loginPostStr = string.Format("Email={0}&Password={1}", HttpUtility.UrlEncode(username),
                    HttpUtility.UrlEncode(password));
                byte[] loginPostBytes = Encoding.ASCII.GetBytes(loginPostStr);

                _logger.Log(string.Format("Creating login post for ({0})", LoginPageUri));
                HttpWebRequest loginPost = WebRequest.CreateHttp(LoginPageUri);
                AddStandardHeaders(loginPost);
                loginPost.CookieContainer = new CookieContainer();
                loginPost.CookieContainer.Add(BaseUri, new Cookie(ReqVerCookieName, reqVerToken));
                loginPost.Method = "POST";
                loginPost.ContentType = "application/x-www-form-urlencoded";
                loginPost.ContentLength = loginPostBytes.Length;
                loginPost.AllowAutoRedirect = false;
                loginPost.Timeout = _clientConfig.Timeout;
                using (Stream loginPostRequestStream = loginPost.GetRequestStream())
                {
                    loginPostRequestStream.Write(loginPostBytes, 0, loginPostBytes.Length);
                }

                _logger.Log("Getting response");
                string authToken;
                using (HttpWebResponse loginResponse = (HttpWebResponse) loginPost.GetResponse())
                {
                    _logger.Log(string.Format("Got status code ({0})", loginResponse.StatusCode));
                    if (loginResponse.StatusCode != HttpStatusCode.OK && loginResponse.StatusCode != HttpStatusCode.Found)
                    {
                        using (Stream stream = loginResponse.GetResponseStream())
                        {
                            string streamContent = "<null>";
                            if (stream != null)
                            {
                                StreamReader sr = new StreamReader(stream, Encoding.UTF8);
                                streamContent = sr.ReadToEnd();
                            }

                            throw new Exception(
                                string.Format("Login got status code ({0}). The stream content was:\r\n{1}",
                                    loginResponse.StatusCode, streamContent));
                        }
                    }

                    Cookie authCookie = loginResponse.Cookies[AuthCookieName];
                    if (authCookie == null)
                    {
                        throw new Exception(string.Format("Did not get expected auth cookie ({0}) after login",
                            AuthCookieName));
                    }
                    authToken = authCookie.Value;
                }
                

                _logger.Log("Successfully got authentication token token");

                _connectionInfo = new ConnectionInfo(reqVerToken, authToken);

                _logger.Log("Login successful");
            }
            catch (Exception e)
            {
                _logger.Log(e.ToString(), EnumMessageType.Error);
                throw;
            }
        }

        public void Logout()
        {
            try
            {
                if (_connectionInfo == null)
                {
                    return;
                }

                _logger.Log("----- Logout -----");


                _logger.Log(string.Format("Creating dummy request for ({0})", BaseUri));
                HttpWebRequest dummyRequest = WebRequest.CreateHttp(BaseUri);
                AddStandardHeaders(dummyRequest);
                dummyRequest.CookieContainer = new CookieContainer();
                dummyRequest.CookieContainer.Add(BaseUri, new Cookie(ReqVerCookieName, _connectionInfo.ReqVerToken));
                dummyRequest.CookieContainer.Add(BaseUri, new Cookie(AuthCookieName, _connectionInfo.AuthToken));
                dummyRequest.Timeout = _clientConfig.Timeout;

                _logger.Log("Getting response");
                string logoutStr;
                using (HttpWebResponse dummyResponse = (HttpWebResponse) dummyRequest.GetResponse())
                {
                    _logger.Log(string.Format("Got status code ({0})", dummyResponse.StatusCode));
                    if (dummyResponse.StatusCode != HttpStatusCode.OK)
                    {
                        using (Stream stream = dummyResponse.GetResponseStream())
                        {
                            string streamContent = "<null>";
                            if (stream != null)
                            {
                                StreamReader sr = new StreamReader(stream, Encoding.UTF8);
                                streamContent = sr.ReadToEnd();
                            }

                            dummyResponse.Close();
                            throw new Exception(
                                string.Format(
                                    "Unexpected response status code to dummy request ({0}). The stream content was:\r\n{1}",
                                    dummyResponse.StatusCode, streamContent));
                        }
                    }
                    _logger.Log("Getting response stream");
                    using (Stream dummyStream = dummyResponse.GetResponseStream())
                    {
                        if (dummyStream == null)
                        {
                            throw new Exception(string.Format("Stream was null"));
                        }

                        _logger.Log("Parsing response stream looking for request verification token");
                        StreamReader dummyStreamReader = new StreamReader(dummyStream, Encoding.UTF8);
                        string line = null;
                        string logoutReqVerToken = null;
                        while ((line = dummyStreamReader.ReadLine()) != null)
                        {
                            if (line.Contains("logoutForm") && line.Contains("__RequestVerificationToken"))
                            {
                                int index = line.IndexOf("value=\"", StringComparison.Ordinal);
                                if (index < 0)
                                {
                                    throw new Exception(
                                        string.Format("Unable to properly parse logoutForm info. Value not found."));
                                }
                                line = line.Substring(index + 7);
                                int endIndex = line.IndexOf("\"", StringComparison.Ordinal);
                                logoutReqVerToken = line.Substring(0, endIndex);

                                break;
                            }
                        }
                        if (logoutReqVerToken == null)
                        {
                            throw new Exception(string.Format("Unable to get logout request verification token"));
                        }

                        _logger.Log("Successfully found request verification token");

                        logoutStr = string.Format("{0}={1}", ReqVerCookieName, logoutReqVerToken);
                    }
                }
                

                
                byte[] logoutBytes = Encoding.UTF8.GetBytes(logoutStr);

                _logger.Log(string.Format("Creating request for ({0})", LogoutPageUri));
                HttpWebRequest logoutRequest = WebRequest.CreateHttp(LogoutPageUri);
                AddStandardHeaders(logoutRequest);
                logoutRequest.CookieContainer = new CookieContainer();
                logoutRequest.CookieContainer.Add(BaseUri, new Cookie(ReqVerCookieName, _connectionInfo.ReqVerToken));
                logoutRequest.CookieContainer.Add(BaseUri, new Cookie(AuthCookieName, _connectionInfo.AuthToken));
                logoutRequest.Method = "POST";
                logoutRequest.ContentType = "application/x-www-form-urlencoded";
                logoutRequest.ContentLength = logoutBytes.Length;
                logoutRequest.AllowAutoRedirect = false;
                logoutRequest.Timeout = _clientConfig.Timeout;
                using (Stream findTradesRequestStream = logoutRequest.GetRequestStream())
                {
                    findTradesRequestStream.Write(logoutBytes, 0, logoutBytes.Length);
                }

                _logger.Log("Getting response");
                using (HttpWebResponse logoutResponse = (HttpWebResponse) logoutRequest.GetResponse())
                {
                    _logger.Log(string.Format("Got status code ({0})", logoutResponse.StatusCode));
                    if (logoutResponse.StatusCode != HttpStatusCode.OK &&
                        logoutResponse.StatusCode != HttpStatusCode.Found)
                    {
                        using (Stream stream = logoutResponse.GetResponseStream())
                        {
                            string streamContent = "<null>";
                            if (stream != null)
                            {
                                StreamReader sr = new StreamReader(stream, Encoding.UTF8);
                                streamContent = sr.ReadToEnd();
                            }

                            throw new Exception(
                                string.Format("Logout got status code ({0}). The stream content was:\r\n{1}",
                                    logoutResponse.StatusCode, streamContent));
                        }
                    }
                    _logger.Log("Logout completed");
                }
                
                //Console.WriteLine("Logout successfull");
            }
            catch (Exception e)
            {
                _logger.Log(e.ToString(), EnumMessageType.Error);
                //Console.WriteLine(e);
            }
            finally
            {
                _connectionInfo = null;
            }
        }

        private T DoGetRequest<T>(Uri uri)
        {
            return DoGetRequest<T>(uri, new List<KeyValuePair<string, string>>());
        }

        private T DoGetRequest<T>(Uri uri, string key, string value)
        {
            return DoGetRequest<T>(uri, new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>(key, value) });
        }

        private T DoGetRequest<T>(Uri uri, List<KeyValuePair<string, string>> parameters)
        {
            VerifyLoggedIn();

            string str = "";
            if (parameters.Count != 0)
            {
                foreach (KeyValuePair<string, string> pair in parameters)
                {
                    str += str.Length == 0 ? "?" : "&";
                    str += string.Format("{0}={1}", HttpUtility.UrlPathEncode(pair.Key), HttpUtility.UrlPathEncode(pair.Value));
                }
            }

            _logger.Log(string.Format("Creating request for ({0})", uri));
            HttpWebRequest request = WebRequest.CreateHttp(string.Format("{0}{1}", uri, str));
            AddStandardHeaders(request);
            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(BaseUri, new Cookie(ReqVerCookieName, _connectionInfo.ReqVerToken));
            request.CookieContainer.Add(BaseUri, new Cookie(AuthCookieName, _connectionInfo.AuthToken));
            request.Method = "GET";
            request.AllowAutoRedirect = false;
            request.Timeout = _clientConfig.Timeout;
            request.Accept = "application/json";

            string responseStr = GetResponse(request, true);

            return JsonConvert.DeserializeObject<T>(responseStr);
        }

        private void DoPostRequest(Uri uri, object o, bool readResponse)
        {
            byte[] bytes = Serialize(o);

            DoPostRequest(uri, bytes, "application/json", false);
        }

        private string DoPostRequest(Uri uri, byte[] bytes, string contentType, bool readResponse)
        {
            VerifyLoggedIn();

            _logger.Log(string.Format("Creating request for ({0})", uri));
            HttpWebRequest request = WebRequest.CreateHttp(uri);
            AddStandardHeaders(request);
            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(BaseUri, new Cookie(ReqVerCookieName, _connectionInfo.ReqVerToken));
            request.CookieContainer.Add(BaseUri, new Cookie(AuthCookieName, _connectionInfo.AuthToken));
            request.Method = "POST";
            request.ContentType = contentType;
            request.ContentLength = bytes.Length;
            request.AllowAutoRedirect = false;
            request.Timeout = _clientConfig.Timeout;
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            return GetResponse(request, readResponse);
        }


        private void AddStandardHeaders(HttpWebRequest request)
        {
            //request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36";
            //request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            //request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            //request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.5");
            //request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            //request.Referer = "http://www.elitetradingtool.co.uk/Admin/";
        }

        private string GetResponse(HttpWebRequest request, bool readResponse)
        {
            _logger.Log("Getting response");
            using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
            {
                _logger.Log(string.Format("Got status code ({0})", response.StatusCode));
                if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Found)
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        string streamContent = "<null>";
                        if (stream != null)
                        {
                            StreamReader sr = new StreamReader(stream, Encoding.UTF8);
                            streamContent = sr.ReadToEnd();
                        }

                        throw new Exception(
                            string.Format("Request got status code ({0}). The stream content was:\r\n{1}",
                                response.StatusCode, streamContent));
                    }
                }

                if (readResponse)
                {
                    _logger.Log("Reading response stream");
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        StreamReader sr = new StreamReader(responseStream);
                        return sr.ReadToEnd();
                    }
                }
            }
            return null;
        }

        private static byte[] Serialize(object o)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new StringEnumConverter { AllowIntegerValues = false});
            string str = JsonConvert.SerializeObject(o, Formatting.None, settings);
            return Encoding.UTF8.GetBytes(str);
        }

        public void Dispose()
        {
            Logout();
        }

        private void VerifyLoggedIn()
        {
            if (_connectionInfo == null)
            {
                throw new Exception(string.Format("Not logged in. Please login before calling any methods."));
            }
        }
    }
}
