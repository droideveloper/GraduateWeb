﻿//
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
    
    [DataContract(Name = "Social")]
    public class SocialProxy {

        [DataMember]
        public string Linkedin { get; set; }
        [DataMember]
        public string Twitter { get; set; }
        [DataMember]
        public string Facebook { get; set; }

        public SocialProxy(Social social) {
            this.Linkedin = social.Linkedin;
            this.Twitter = social.Twitter;
            this.Facebook = social.Facebook;
        }
    }
}
