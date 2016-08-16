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
    [DataContract]
    public class CreateProxy {

        [DataMember]
        public AccountProxy Account { get; set; }
        [DataMember]
        public InfoProxy Person { get; set; }
        [DataMember]
        public SocialProxy Social { get; set; }
        [DataMember]
        public List<GraduationInfoProxy> Graduations { get; set; }
        [DataMember]
        public CapInfoProxy Cap { get; set; }
        [DataMember]
        public ExchangeInfoProxy Exchange { get; set; }
        [DataMember]
        public WorkplaceInfoProxy Workplace { get; set; }
    }
}
