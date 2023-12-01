using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Services.Analytics
{
    partial class AnalyticsServiceInstance
    {
        public void CustomData(string eventName, IDictionary<string, object> eventParams)
        {
            if (!ServiceEnabled)
            {
                return;
            }

            dataBuffer.PushStartEvent(eventName, DateTime.Now, null);

            foreach (var paramPair in eventParams)
            {
                /*
                 * Had a read of the performance of typeof - the two options were a switch on Type.GetTypeCode(paramType) or
                 * the if chain below. Although the if statement involves multiple typeofs, this is supposedly a fairly light
                 * operation, and the alternative switch option involved some messy/crazy cases for ints.
                 */
                var paramType = paramPair.Value.GetType();
                if (paramType == typeof(string))
                {
                    dataBuffer.PushString((string)paramPair.Value, paramPair.Key);
                }
                else if (paramType == typeof(int))
                {
                    dataBuffer.PushInt((int)paramPair.Value, paramPair.Key);
                }
                else if (paramType == typeof(long))
                {
                    dataBuffer.PushInt64((long)paramPair.Value, paramPair.Key);
                }
                else if (paramType == typeof(float))
                {
                    dataBuffer.PushFloat((float)paramPair.Value, paramPair.Key);
                }
                else if (paramType == typeof(double))
                {
                    dataBuffer.PushDouble((double)paramPair.Value, paramPair.Key);
                }
                else if (paramType == typeof(bool))
                {
                    dataBuffer.PushBool((bool)paramPair.Value, paramPair.Key);
                }
                else if (paramType == typeof(DateTime))
                {
                    dataBuffer.PushTimestamp((DateTime)paramPair.Value, paramPair.Key);
                }
                else
                {
                    Debug.LogError($"Unknown type found for key {paramPair.Key}, this value will not be included.");
                }
            }

            dataBuffer.PushEndEvent();
        }
    }
}
