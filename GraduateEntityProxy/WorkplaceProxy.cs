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
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraduateEntity;

namespace GraduateEntityProxy {

    [DataContract(Name = "Workplace")]
    public class WorkplaceProxy {

        [DataMember]
        public string OrganizationName { get; set; }
        [DataMember]
        public TitleProxy Title { get; set; }
        [DataMember]
        public DepartmentProxy Department { get; set; }
        [DataMember]
        public IndustryProxy Industry { get; set; }
        [DataMember]
        public CountryProxy Country { get; set; }
        [DataMember]
        public CityProxy City { get; set; }

        public WorkplaceProxy(Workplace workplace) {
            this.OrganizationName = workplace.OrganizationName;
            this.Title = workplace.Title == null ? null : new TitleProxy(workplace.Title);
            this.Department = workplace.Department == null ? null : new DepartmentProxy(workplace.Department);
            this.Industry = workplace.Industry == null ? null : new IndustryProxy(workplace.Industry);
            this.Country = workplace.Country == null ? null : new CountryProxy(workplace.Country);
            this.City = workplace.City == null ? null : new CityProxy(workplace.City);
        }
    }
}
