﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_BSR_S2_Calculator.Utility {
    public interface IFromConverter<Self> {
        public static abstract Self From(object obj);
    }
}
