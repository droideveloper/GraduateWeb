//
//  Copyright 2016  
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateEntityProxy {

    public static class Collections {

        public static bool IsNullOrEmpty<T>(this ICollection<T> array) {
            return array == null || array.Count <= 0;
        }

        public static ICollection<T> Filter<T>(this ICollection<T> array, Func<T, bool> filter) {
            if(array.IsNullOrEmpty()) {  return null; }
            return array.Where(x => filter(x))
                        .ToList();
        }

        public static ICollection<T> Sort<T, TSource>(this ICollection<T> array, Func<T, TSource> sort) {
            if(array.IsNullOrEmpty()) { return null; }
            return array.OrderBy(sort)
                        .ToList();
        }
    }
}
