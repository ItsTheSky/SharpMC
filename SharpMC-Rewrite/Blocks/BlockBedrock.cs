﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpMCRewrite.Blocks
{
	class BlockBedrock : Block
	{
		public BlockBedrock() : base(7)
		{
			Durability = 60000;
		}
	}
}
