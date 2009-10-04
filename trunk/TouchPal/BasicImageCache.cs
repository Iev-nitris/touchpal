/*
 * This file is part of the TouchPal project hosted on Google Code
 * (http://code.google.com/p/touchpal). See the accompanying license.txt file for 
 * applicable licenses.
 */

/*
 * (c) Copyright Craig Courtney 2009 All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace TouchPal
{
    class BasicImageCache : IImageCache
    {
        private Dictionary<string, Image> imageCache;

        public BasicImageCache()
        {
            imageCache = new Dictionary<string, Image>();
        }

        public void Clear()
        {
            imageCache.Clear();
        }

        public Image getImage(string filename)
        {

            if (filename == null || "".Equals(filename))
                return null;

            if (imageCache.ContainsKey(filename))
                return imageCache[filename];

            Image newImage = null;
            if (File.Exists(filename))
            {
                TouchPal.Debug("Loading Image:" + filename);
                newImage = Image.FromFile(filename);
                imageCache.Add(filename, newImage);
            }
            else
            {
                TouchPal.Warn("Missing Image:" + filename);
            }

            return newImage;
        }
    }
}
