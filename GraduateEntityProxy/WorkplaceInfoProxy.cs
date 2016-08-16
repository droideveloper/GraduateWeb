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

namespace GraduateEntityProxy {

    /*
        Account: { 
            UserName: account.UserName, 
            Password: md5(account.Password).toUpperCase() 
        }, 
        Person: person, 
        Social: social, 
        Graduations: toArray(graduations),//there cities here need to get rid of them
        Cap: cap,
        Exchange: exchange,
        Workplace: workplace ? workplace : null
     */
    [DataContract(Name = "Workplace")]
    public class WorkplaceInfoProxy {
        
        [DataMember]
        public string OrganizationName { get; set; }
        [DataMember]
        public int? Title { get; set; }
        [DataMember]
        public int? Department { get; set; }
        [DataMember]
        public int? Industry { get; set; }
        [DataMember]
        public int? Country { get; set; }
        [DataMember]
        public int? City { get; set; }
    }
}
