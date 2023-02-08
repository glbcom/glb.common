using System;
using System.Linq;
using System.Threading.Tasks;
using Glb.Common.Interfaces;
using Glb.Common.Entities.Requests;
using Glb.Common.Entities.Responses;
using Glb.Common.Settings;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Collections.Generic;

namespace Glb.Common.GlbServices
{


    public class SMS_Service : ISMS_Service
    {
        private SMS_Settings smsSettings;


        public SMS_Service(IOptions<SMS_Settings> smsSettings)
        {
            this.smsSettings = smsSettings.Value;
        }

        private async Task<T?> PostAsync<T>(SMS_ServiceProvider smsProvider, string requestUri, HttpContent? httpContent = null)
        {
            using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
            {
                HttpResponseMessage response = await client.PostAsync(smsProvider.BaseAddress.ToString() + requestUri, httpContent);
                Task<string> Str = response.Content.ReadAsStringAsync();
                var resResult = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(Str.Result);
                return resResult;
            }
        }

        public async Task<SMS_Response> SendMourseSMSAsync(SMS_DataMoursel smsMourselData)
        {
            string SenderId;
            SMS_Response Resp = new SMS_Response();

            SMS_ServiceProvider? mourselServiceProvider = smsSettings.ServiceProviders.FirstOrDefault(p => p.ServiceProvider == Enums.ServiceProvider.Moursel);
            if (mourselServiceProvider == null)
            {
                Resp.Code = ((int)System.Net.HttpStatusCode.Forbidden).ToString();
                Resp.Message = string.Format("Service provider not set");
                return Resp;
            }
            try
            {
                string[]? Result = await this.PostAsync<string[]>(mourselServiceProvider, "/ClientLogIn?userName=" + mourselServiceProvider.Username + "&passWord=" + mourselServiceProvider.Password);

                if (Result == null || Result.Length == 0)
                {
                    Resp.Code = ((int)System.Net.HttpStatusCode.Forbidden).ToString();
                    Resp.Message = string.Format("Invalid username or password");
                    return Resp;
                }


                switch (Result[0])
                {


                    case "-1":
                        {
                            Resp.Code = "-1";
                            Resp.Message = string.Format("Invalid username or password");
                            return Resp;
                        }
                    case "-2":
                        {

                            Resp.Code = "-2";
                            Resp.Message = string.Format("Inactive user");
                            return Resp;
                        }
                    case "-3":
                        {
                            Resp.Code = "-3";
                            Resp.Message = string.Format("Server Error");
                            return Resp;
                        }
                }


                string[]? SenderIDs = await this.PostAsync<string[]>(mourselServiceProvider, "/getSenderID?logID=" + Result[0]);


                if (SenderIDs == null || SenderIDs.Length == 0)
                {
                    Resp.Code = ((int)System.Net.HttpStatusCode.NotFound).ToString(); ;
                    Resp.Message = string.Format("Error Retrieving Sender ID's username:{0}", mourselServiceProvider.Username);
                    return Resp;
                }

                if (mourselServiceProvider.SenderID != null && mourselServiceProvider.SenderID.Trim().Length > 0 && !SenderIDs.Contains(mourselServiceProvider.SenderID))
                {
                    Resp.Code = ((int)System.Net.HttpStatusCode.NotFound).ToString();
                    Resp.Message = string.Format("Invalid Sender ID:{0} ", mourselServiceProvider.SenderID);
                    return Resp;
                }
                else if (mourselServiceProvider.SenderID == null || mourselServiceProvider.SenderID.Trim().Length == 0)
                    SenderId = SenderIDs[0];
                else
                    SenderId = mourselServiceProvider.SenderID;


                int Code = await this.PostAsync<int>(mourselServiceProvider, "/sendSMScs?logID=" + Result[0] + "&smsMessage=" + smsMourselData.Body + "&ReceiverNumbers=" + string.Join("~", smsMourselData.ReceiverNumbers) + "&MessageType=" + ((int)smsMourselData.MessageType).ToString() + "&getSenderID=" + SenderId + "&sendDate=0&multiBatch=0&isLastBatch=0");

                switch (Code)
                {
                    case 0:
                        {
                            Resp.Code = "0";
                            Resp.Message = string.Format("Wrong data");
                            return Resp;

                        }
                    case -1:
                        {
                            Resp.Code = "-1";
                            Resp.Message = string.Format("Session timed out or wrong session");
                            return Resp;
                        }
                    case -2:
                        {
                            Resp.Code = "-2";
                            Resp.Message = string.Format("No receivers");
                            return Resp;

                        }
                    case -3:
                        {
                            Resp.Code = "-3";
                            Resp.Message = string.Format("Not enough balance");
                            return Resp;
                        }
                    case -4:
                        {
                            Resp.Code = "-4";
                            Resp.Message = string.Format("Invalid receivers");
                            return Resp;
                        }
                    case -5:
                        {

                            Resp.Code = "-5";
                            Resp.Message = string.Format("Unspecified error");
                            return Resp;
                        }
                    case -6:
                        {
                            Resp.Code = "-6";
                            Resp.Message = string.Format("Message > 960 characters");
                            return Resp;
                        }
                    case > 0:
                        {
                            Resp.Code = ((int)System.Net.HttpStatusCode.OK).ToString();
                            Resp.Success = true;
                            Resp.Message = string.Format("Message Sent Successfully");
                            return Resp;
                        }
                    default:
                        {
                            Resp.Code = ((int)System.Net.HttpStatusCode.BadRequest).ToString();
                            Resp.Success = false;
                            Resp.Message = string.Format("Unknown Error occured");
                            return Resp;
                        }
                }
            }
            catch (Exception Ex)
            {
                Resp.Code = ((int)System.Net.HttpStatusCode.Forbidden).ToString();
                Resp.Message = string.Format(Ex.Message);
                return Resp;
            }
        }

        public async Task<SMS_Response> SendAsync(SMS_Data smsData, Enums.ServiceProvider? smsServiceProvider = null)
        {
            SMS_ServiceProvider? serviceProvider;
            SMS_Response Resp = new SMS_Response();
            SMS_DataMoursel smsDataMoursel = new SMS_DataMoursel
            {
                Body = smsData.Body,
                ReceiverNumbers = smsData.ReceiverNumbers,
                MessageType = Enums.MourselSMS_Type.Latin
            };

            if (smsServiceProvider == null)
            {
                serviceProvider = smsSettings.ServiceProviders.FirstOrDefault<SMS_ServiceProvider>();
            }
            else
            {
                serviceProvider = smsSettings.ServiceProviders.FirstOrDefault(srvProvider => srvProvider.ServiceProvider == smsServiceProvider);
            }

            if (serviceProvider != null && serviceProvider.ServiceProvider == Enums.ServiceProvider.Moursel)
            {
                return await this.SendMourseSMSAsync(smsDataMoursel);
            }
            else
            {
                Resp.Code = ((int)System.Net.HttpStatusCode.NotFound).ToString();
                Resp.Message = "SMS service provider not configured";
                return Resp;
            }
        }
    }

    public partial class Extensions
    {
        #region  "MobileNumber"
        public static string? ToShortMobileNumber(this string MobileNumber, string Separator = "", string[]? MobileCodes = null)
        {
            if (MobileCodes == null)
                MobileCodes = new string[] { "3", "70", "71", "76", "78", "79", "81" };
            string mobilenumber = MobileNumber;
            mobilenumber = System.Text.RegularExpressions.Regex.Replace(MobileNumber, "[^0-9]", "");
            if (mobilenumber.Length < 7)
                return null;
            string BaseNumber = mobilenumber.Substring(mobilenumber.Length - 6);
            mobilenumber = mobilenumber.Replace(BaseNumber, "");
            string? Code = MobileCodes.ToList().Where(_code => mobilenumber.EndsWith((int.Parse(_code)).ToString())).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(Code))
                return null;
            mobilenumber = string.Format("{0}{1}{2}", Code.PadLeft(2, '0'), Separator, BaseNumber);
            return mobilenumber;
        }
        public static string? ToLongMobileNumber(this string MobileNumber, string[]? MobileCodes = null)
        {
            string? shortmobilenumber = ToShortMobileNumber(MobileNumber, MobileCodes: MobileCodes);
            if (!string.IsNullOrWhiteSpace(shortmobilenumber))
                return "+961" + int.Parse(shortmobilenumber).ToString();
            return null;
        }
        #endregion
    }
}
