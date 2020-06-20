using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GameLogicTest
{
    public class TestBase
    {
        protected void CallPrivateMethod(object objectToCall, string methodName, object[] parameters)
        {
            try
            {
                var methodInfo = objectToCall.GetType().GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                methodInfo.Invoke(objectToCall, parameters);
            }
            catch(TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        protected T CallPrivateMethod<T>(object objectToCall, string methodName, object[] parameters)
        {
            var methodInfo = objectToCall.GetType().GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            T retVal;
            try
            {
                retVal = (T)methodInfo.Invoke(objectToCall, parameters);
            }
            catch(TargetInvocationException ex)
            {
                throw ex.InnerException;
            }

            return retVal;
        }

        public MemberType GetPrivateMember<MemberType>(string memberName, object obj)
        {
            var fieldInfo = GetFieldInfo(memberName, obj);
            return (MemberType)fieldInfo.GetValue(obj);
        }

        public void SetPrivateMember<MemberType>(string memberName, object obj, MemberType value)
        {
            var fieldInfo = GetFieldInfo(memberName, obj);
            fieldInfo.SetValue(obj, value);
        }

        private FieldInfo GetFieldInfo(string memberName, object obj)
        {
            var objType = obj.GetType();
            var fieldInfo = objType.GetField(memberName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            return fieldInfo;
        }
    }
}
