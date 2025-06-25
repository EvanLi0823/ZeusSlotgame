//using System;
//using System.Collections;
//using System.Collections.Generic;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using UnityEngine;
//
//public class RuleActionConverter :JsonConverter
//{
//    public override bool CanConvert(Type objectType)
//    {
//        return (objectType == typeof(IRuleAction));
//    }
//
//    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
//    {
//        JObject jo = JObject.Load(reader);
//        if (jo["RAType"].Value<int>() == (int)RuleActionType.FEATURE)
//        {
//            return jo.ToObject<FeatureRuleAction>(serializer);
//        }
//        if (jo["RAType"].Value<int>() == (int)RuleActionType.RTP)
//        {
//            return jo.ToObject<RtpRuleAction>(serializer);
//        }
//        if (jo["RAType"].Value<int>() == (int)RuleActionType.NEAR_MISS)
//        {
//            return jo.ToObject<NearMissRuleAction>(serializer);
//        }
//        return null;
//    }
//
//    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
//    {
//        throw new NotImplementedException();
//    }
//}
