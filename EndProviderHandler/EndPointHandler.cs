using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ObjectPool;
using MyController;

namespace EndPoint
{
    public class EndPointHandler
    {
        private List<KeyValuePair<string, Delegate>> EndPointList { get; set; } = new();

        private List<KeyValuePair<string, ParameterInfo[]>> EndPointParamList { get; set; } = new();

        private List<KeyValuePair<string, Type>> Controllers { get; set; } = new();

        public int Add<BaseController>(string route) where BaseController : MyControllerBase 
        {

            Controllers.Add(new KeyValuePair<string, Type>(route,typeof(BaseController)));

            return 0;
        }

        public Type ReturnControllerType(string route) 
        {
            var type = Controllers.Where(x => x.Key.Equals(route)).Select(x => x.Value).FirstOrDefault();

            return type;
        }

        public string ReturnRoute(Type myControllerType) 
        {
            var str = Controllers.Where(x => x.Value == myControllerType).Select(x => x.Key).FirstOrDefault();
            return str;
        }

        internal int Add(string route, MyControllerBase controller) 
        {
            try
            {
                Type controllerType = controller.GetType();
                var b = route.Split('/');
                MethodInfo? info = controllerType.GetMethods().Where(x =>               
                {
                    try
                    {
                        if (x.Name.Equals(b[^1]) || x.GetCustomAttribute<RouteAttribute>().Template.Equals(route))
                        {
                            return true;
                        }
                    }
                    catch (Exception e) 
                    {
                        return false;
                    }
                    return false;
                }
                ).FirstOrDefault();

                var param_info = info.GetParameters();
                EndPointParamList.Add(new KeyValuePair<string, ParameterInfo[]>(route, param_info));



                Type delegateType = Expression.GetDelegateType(info.GetParameters().Select(pi => pi.ParameterType).Concat( new[] {info.ReturnType }).ToArray());
                if (info is null)
                {
                    return 1;
                }
                Delegate a = Delegate.CreateDelegate(delegateType, controller, info);
                
                EndPointList.Add(new KeyValuePair<string, Delegate>(route, a));
            }
            finally 
            {
            }
            
            return 0;
        }

        internal int Delete(string route) 
        {
            EndPointList.RemoveAll(x => x.Key.Equals(route));
            EndPointParamList.RemoveAll(x => x.Key.Equals(route));
            return 0;
        }

        internal bool HasMethodAttribute<CheckAttribute>(string route, MyControllerBase controller)
        {
            try
            {
                Type controllerType = controller.GetType();
                var b = route.Split('/');

                MethodInfo? info = controllerType.GetMethods().Where(x =>
                {
                    try
                    {
                        var t = x.GetCustomAttribute<RouteAttribute>().Template;
                        if (x.Name.Equals(b[^1]) || x.GetCustomAttribute<RouteAttribute>().Template.Equals(route))
                        {
                            return true;
                        }
                    }
                    catch (Exception e) 
                    {
                        return false;
                    }
                    finally
                    {
                       
                    }
                    return false;
                }
                ).FirstOrDefault();

                return info.GetCustomAttributes().Where(x => x.GetType().IsAssignableTo(typeof(CheckAttribute))).Any();

            }
            finally 
            {
            }
            return false;
        }

        public object? InvokeMethod(string route, Dictionary<string,string> args) 
        {
            var paramList = EndPointParamList.Where(x => (x.Key.Equals(route) && x.Value.Length == args.Count())
            && (x.Value.Where(s => 
            {
                var type = s.ParameterType;
                TypeConverter converter = TypeDescriptor.GetConverter(type);
                foreach (var arg in args) 
                {
                    if (converter.IsValid((arg).Value as string)) 
                    {
                        return true;
                    }
                }

                return false;
            }).Count() == args.Count())).FirstOrDefault().Value;


            List<object> obj_list = new();
            foreach (var param in paramList) 
            {
                obj_list.Add(args.Where(x => x.Key.Equals(param.Name)).Select(x => x.Value).FirstOrDefault());              
            }

            object[] objects = obj_list.ToArray();
            int i = 0;
            foreach (var param in paramList) 
            {
                try
                {
                    objects[i] = Convert.ChangeType(objects[i], param.ParameterType);
                }
                catch (Exception e)
                {

                }
                i++;
            }       
            var obj = EndPointList.Where(x => x.Key.Equals(route)).Select(x => x.Value).Where(x => x.GetMethodInfo().GetParameters().Length == args.Count())
                .FirstOrDefault().DynamicInvoke(objects);
            return obj;
        }
    }
}
 