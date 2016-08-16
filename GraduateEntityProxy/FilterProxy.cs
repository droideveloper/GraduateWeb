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
using System.Runtime.Serialization;
using GraduateEntity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduateEntityProxy {

    [DataContract(Name = "Filter")]
    public class FilterProxy {

        [DataMember]
        public int FilterID { get; set; }
        [DataMember]
        public string FilterType { get; set; }
        [DataMember]
        public string FilterValue { get; set; }

        public FilterProxy(Filter filter) {
            this.FilterID = filter.FilterID;
            this.FilterType = filter.FilterType;
            this.FilterValue = filter.FilterValue;
        }

        public static List<FilterProxy> FromFilters(List<Filter> filters) {
            return filters.IsNullOrEmpty() ? null : filters.Select(f => new FilterProxy(f)).ToList();
        }
    }
}
