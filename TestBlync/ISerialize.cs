using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBlync
{
   
        public interface ISerialize
        {
            byte[] SerializeObject(object Object);
            T DeserializeObject<T>(byte[] bytes) where T : class;
        }
}

