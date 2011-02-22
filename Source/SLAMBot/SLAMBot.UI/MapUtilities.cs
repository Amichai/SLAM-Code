using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLAMBot.Engine;
using System.Drawing;

namespace SLAMBot.UI {
	static class MapUtilities {
		///<summary>Reads an EnvironmentMap from an image.  The HSL brightness of each pixel becomes the value at each cell.</summary>
		public static EnvironmentMap FromBitmap(Bitmap image) {
			var retVal = new EnvironmentMap();

			for (int x = 0; x < image.Width; x++) {
				for (int y = 0; y < image.Height; y++) {
					int mapY = image.Height - y - 1;	//In Bitmaps, Y goes downward; in the map, it goes upward
					var color = image.GetPixel(x, y);
					var brightness = color.GetBrightness();
					retVal[x, mapY] = 1 - brightness;
				}
			}

			return retVal;
		}
	}
}
