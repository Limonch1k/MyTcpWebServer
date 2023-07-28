using MyController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI
{
    public class CustomDI
    {
        private List<KeyValuePair<int, Type>> TypeTransiet { get; set; } = new();

        private List<KeyValuePair<int, Type>> TypeScoped { get; set; } = new();

        private List<KeyValuePair<int, Type>> TypeSingleton { get; set; } = new();


        public void Add<Entity>() where Entity : class, new()
        {
            Type type = typeof(Entity);
            TypeTransiet.Add(new KeyValuePair<int, Type>(0, type));
            //object obj = Activator.CreateInstance(type);
        }




        public void AddTransiet<Entity>(object obj)
        {
            Type type = typeof(Entity);
            TypeTransiet.Add(new KeyValuePair<int, Type>(0, type));
        }

        internal ClassType GetImplementation<ClassType>() where ClassType : class 
        {
            foreach (var t in TypeTransiet.Select(x => x.Value)) 
            {
                if (typeof(ClassType) == t) 
                {
                    return (Activator.CreateInstance(typeof(ClassType)) as ClassType);
                }
            }

            foreach (var t in TypeScoped.Select(x => x.Value)) 
            {
                if (typeof(ClassType) == t)
                {
                    return (Activator.CreateInstance(typeof(ClassType)) as ClassType);
                }
            }

            /*foreach (var t in TypeSingleton.Where(x => x.Key == 0).Select(x => x.Value));
            {
                if (typeof(ClassType) == t)
                {
                    return (Activator.CreateInstance(typeof(ClassType)) as ClassType);
                }
            }*/

            return null;
        }

        internal object GetImplementation(Type t)
        {
            return Activator.CreateInstance(t);
        }

        internal List<ClassType> GetImplementations<ClassType>(Type t) where ClassType : class
        {
            List<ClassType> list = new List<ClassType>();
            if (TypeTransiet.Where(x => x.Value.IsAssignableTo(t)).Select(x => x.Value).Count() is not 0) 
            {
                var length = TypeTransiet.Where(x => x.Value.IsAssignableTo(t)).Select(x => x.Value).Count();
                var types = TypeTransiet.Where(x => x.Value.IsAssignableTo(t)).Select(x => x.Value).ToArray();
                foreach (var type in types) 
                {
                    list.Add(Activator.CreateInstance(type) as ClassType);
                }
                return list;
            }

            return null;
        }

        public void AddScoped(object obj) 
        {
        }

        public void AddSingleton(object obj) 
        {
        }

    }
}
