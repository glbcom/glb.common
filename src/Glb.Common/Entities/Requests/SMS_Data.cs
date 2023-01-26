using Glb.Common.Enums;

namespace Glb.Common.Entities.Requests;
    public abstract class SMS_DataBase
    {
        public required string Body { get; set; }
        public  required string[] ReceiverNumbers { get; set; }
    }

    public class SMS_Data:SMS_DataBase{

    }
    public class SMS_DataMoursel : SMS_Data
    {
        public MourselSMS_Type MessageType { get; set; }
      
    }