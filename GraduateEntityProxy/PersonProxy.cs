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

    [DataContract(Name = "Person")]
    public class PersonProxy {

        [DataMember]
        public int UserID { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string MiddleName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string IdentityNo { get; set; }
        [DataMember]
        public EducationProxy Education { get; set; }
        [DataMember]
        public WorkplaceProxy Workplace { get; set; }
        [DataMember]
        public SocialProxy Social { get; set; }

        public PersonProxy(Person person) {
            this.UserID = person.UserID;
            this.FirstName = person.FirstName;
            this.MiddleName = person.MiddleName;
            this.LastName = person.LastName;
            this.IdentityNo = person.IdentityNo;
            this.Social = person.Social == null ? null : new SocialProxy(person.Social);
            this.Workplace = person.Workplace == null ? null : new WorkplaceProxy(person.Workplace);
            this.Education = person.Education == null ? null : new EducationProxy(person.Education);
        }

        public static List<PersonProxy> FromPeople(List<Person> people) {
            return people.IsNullOrEmpty() ? null : people.Select(p => new PersonProxy(p)).ToList();
        }
    }
}
