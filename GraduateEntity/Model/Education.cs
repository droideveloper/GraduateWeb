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
using System.Runtime.Serialization;

namespace GraduateEntity {

	public class Education {

		public int EducationID { get; set; }
		public int PersonID { get; set; }

		public DateTime CreateDate { get; set; }
		public DateTime UpdateDate { get; set; }

		//FK_Education_Graduation
		public virtual ICollection<Graduation> Graduations { get; set; }

		//FK_Education_Exchange
		public virtual Exchange Exchange { get; set; }

		//FK_Education_Cap
		public virtual Cap Cap { get; set; }
	}
}

