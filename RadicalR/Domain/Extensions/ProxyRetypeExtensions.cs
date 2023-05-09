using Castle.DynamicProxy;
using Microsoft.OData.Edm;
using System.Uniques;

namespace RadicalR
{
    public static class ProxyRetypeExtensions
    {
        public static Type ProxyRetype(this Type type)
        {
            while (type.IsAssignableTo(typeof(IProxyTargetAccessor)))
                type = type.UnderlyingSystemType.BaseType;
            if (type == typeof(IEdmEntityType))
                return OpenClientRegistry.Mappings[((IEdmEntityType)type).FullTypeName()];
            return type;
        }

        public static Type ProxyRetype(this object obj)
        {
            return ProxyRetype(obj.GetType());
        }

        public static string ProxyRetypeName(this object obj)
        {
            return ProxyRetype(obj.GetType()).Name;
        }

        public static string ProxyRetypeFullName(this object obj)
        {
            return ProxyRetype(obj.GetType()).FullName;
        }

        public static uint ProxyRetypeKey32(this object obj)
        {
            return ProxyRetype(obj.GetType()).FullName.UniqueKey32();
        }

        public static uint ProxyRetypeKey32(this Type obj)
        {
            return ProxyRetype(obj).FullName.UniqueKey32();
        }

        public static ulong ProxyRetypeKey(this object obj)
        {
            return ProxyRetype(obj.GetType()).FullName.UniqueKey();
        }

        public static ulong ProxyRetypeKey(this Type obj)
        {
            return ProxyRetype(obj).FullName.UniqueKey();
        }

        public static bool IsEventType(this object obj)
        {
            return obj.GetType().IsAssignableTo(typeof(Event));
        }

        public static bool IsProper(this Type t)
        {
            if (t.IsAssignableTo(typeof(IProxyTargetAccessor)))
                return false;
            if (t == typeof(IEdmEntityType))
                return false;
            return true;
        }
    }
}