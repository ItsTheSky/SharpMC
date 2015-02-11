﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpMCRewrite.Blocks
{
	class BlockFactory
	{
		public static Block GetBlockById(ushort id)
		{
			if (id == 46) return new BlockTNT();
			if (id == 0) return new BlockAir();
			if (id == 51) return new BlockFire();
			if (id == 7) return new BlockBedrock();
			return new Block(id);
		}
	}
}
