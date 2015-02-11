﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpMCRewrite.Enums;

namespace SharpMCRewrite.Blocks
{
	class BlockAir : Block
	{
		internal BlockAir() : base(0)
		{
			IsReplacible = true;
			IsSolid = false;
		}
	}
}
