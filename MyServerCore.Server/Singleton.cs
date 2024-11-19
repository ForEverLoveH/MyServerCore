using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServerCore.Server
{
    public class Singleton<T> where T : class ,new()
    {
        /// <summary>
        /// 
        /// </summary>
        private static T _instance;
        /// <summary>
        /// 
        /// </summary>
      
        /// <summary>
        /// 
        /// </summary>
        private static T Instance
        {
            get
            {
                 
                    if (_instance == null) _instance = new T();
                    return _instance;
                
            }
        }
        public static T GetInstance()=> Instance;
        
    }
}
