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
   
    [DataContract(Name = "Exchange")]
    public class ExchangeProxy {

        [DataMember]
        public int Year { get; set; }
        [DataMember]
        public UniversityProxy University { get; set; }
        [DataMember]
        public CityProxy City { get; set; }
        [DataMember]
        public CountryProxy Country { get; set; }

        public ExchangeProxy(Exchange exchange) {
            this.Year = exchange.Year;
            this.University = exchange.University == null ? null : new UniversityProxy(exchange.University);
            this.City = exchange.City == null ? null : new CityProxy(exchange.City);
            this.Country = exchange.Country == null ? null : new CountryProxy(exchange.Country);
        }
    }
}
