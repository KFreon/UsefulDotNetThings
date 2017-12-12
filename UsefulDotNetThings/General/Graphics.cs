using System;

namespace UsefulDotNetThings.General
{
    public static class Graphics
    {
        public struct RGBAColour
        {
            public int A, R, G, B;

            public RGBAColour(byte r, byte g, byte b, byte a)
            {
                R = r;
                G = g;
                B = g;
                A = a;
            }

            public RGBAColour(float r, float g, float b, float a)
            {
                R = (byte)(Clamp(r, 0f, 1f) * 255);
                G = (byte)(Clamp(g, 0f, 1f) * 255);
                B = (byte)(Clamp(b, 0f, 1f) * 255);
                A = (byte)(Clamp(a, 0f, 1f) * 255);
            }

            public override string ToString()
            {
                return $"R: {R}, G: {G}, B: {B}, A: {A}";
            }

            static float Clamp(float val, float lower, float upper)
            {
                if (val > upper)
                    return upper;

                if (val < lower)
                    return lower;

                return val;
            }
        }

        public struct ScRGBAColour
        {
            public float A, R, G, B;

            public ScRGBAColour(float r, float g, float b, float a)
            {
                R = r;
                G = g;
                B = g;
                A = a;
            }

            public ScRGBAColour(RGBAColour colour)
            {
                R = colour.R * 1f / 255f;
                G = colour.G * 1f / 255f;
                B = colour.B * 1f / 255f;
                A = colour.A * 1f / 255f;
            }
        }

        public static RGBAColour ScRGBToARGB(float scR, float scG, float scB, float scA)
        {
            if (scA < 0.0f)
            {
                scA = 0.0f;
            }
            else if (scA > 1.0f)
            {
                scA = 1.0f;
            }

            byte a = (byte)((scA * 255.0f) + 0.5f);
            byte r = ScRgbTosRgb(scR);
            byte g = ScRgbTosRgb(scG);
            byte b = ScRgbTosRgb(scB);

            return new RGBAColour(r, g, b, a);
        }

        public static ScRGBAColour ARGBToScRGBA(byte r, byte g, byte b, byte a)
        {
            float sca = (float)a / 255.0f;
            float scr = sRgbToScRgb(r);  // note that context is undefined and thus unloaded
            float scg = sRgbToScRgb(g);
            float scb = sRgbToScRgb(b);

            return new ScRGBAColour(scr, scg, scb, sca);
        }

        private static byte ScRgbTosRgb(float val)
        {
            if (!(val > 0.0))       // Handles NaN case too
            {
                return (0);
            }
            else if (val <= 0.0031308)
            {
                return ((byte)((255.0f * val * 12.92f) + 0.5f));
            }
            else if (val < 1.0)
            {
                return ((byte)((255.0f * ((1.055f * (float)Math.Pow((double)val, (1.0 / 2.4))) - 0.055f)) + 0.5f));
            }
            else
            {
                return (255);
            }
        }

        private static float sRgbToScRgb(byte bval)
        {
            float val = ((float)bval / 255.0f);

            if (!(val > 0.0))       // Handles NaN case too. (Though, NaN isn't actually
                                    // possible in this case.)
            {
                return (0.0f);
            }
            else if (val <= 0.04045)
            {
                return (val / 12.92f);
            }
            else if (val < 1.0f)
            {
                return (float)Math.Pow(((double)val + 0.055) / 1.055, 2.4);
            }
            else
            {
                return (1.0f);
            }
        }
    }
}
