Shader "GlowBeam/Default"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _Hue("Hue Adjust", Range(-0.5, 0.5)) = 0.0
        _Saturation("Saturation Adjust", Range(0.0, 2.0)) = 1.0
        _Lightness("Lightness Adjust", Range(0.0, 2.0)) = 1.0
        _ColorMul("Color Multiplier", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Hue;
            float _Saturation;
            float _Lightness;
            float4 _ColorMul;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float3 rgb2hsl(float3 color)
            {
                float r = color.r, g = color.g, b = color.b;
                float maxc = max(r, max(g, b));
                float minc = min(r, min(g, b));
                float h = 0.0;
                float s = 0.0;
                float l = (maxc + minc) * 0.5;

                if (maxc != minc)
                {
                    float d = maxc - minc;
                    s = l > 0.5 ? d / (2.0 - maxc - minc) : d / (maxc + minc);
                    if (maxc == r)
                        h = (g - b) / d + (g < b ? 6.0 : 0.0);
                    else if (maxc == g)
                        h = (b - r) / d + 2.0;
                    else
                        h = (r - g) / d + 4.0;

                    h /= 6.0;
                }

                return float3(h, s, l);
            }

            float hue2rgb(float p, float q, float t)
            {
                if(t < 0.0) t += 1.0;
                if(t > 1.0) t -= 1.0;
                if(t < 1.0/6.0) return p + (q - p) * 6.0 * t;
                if(t < 1.0/2.0) return q;
                if(t < 2.0/3.0) return p + (q - p) * (2.0/3.0 - t) * 6.0;
                return p;
            }

            float3 hsl2rgb(float3 hsl)
            {
                float h = hsl.x, s = hsl.y, l = hsl.z;
                if(s == 0.0)
                {
                    return float3(l, l, l);
                }
                float q = (l < 0.5) ? (l * (1.0 + s)) : (l + s - l * s);
                float p = 2.0 * l - q;

                float r = hue2rgb(p, q, h + 1.0/3.0);
                float g = hue2rgb(p, q, h);
                float b = hue2rgb(p, q, h - 1.0/3.0);

                return float3(r, g, b);
            }

            // Fragment shader: simply sample the texture at the passed UV coordinates
            float4 frag(v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);

                float3 hsl = rgb2hsl(col.rgb);

                hsl.x = frac(hsl.x + _Hue);
                hsl.y *= _Saturation;
                hsl.z *= _Lightness;

                float3 rgb = hsl2rgb(hsl);

                col.rgb = rgb * _ColorMul.rgb;
                col.a *= _ColorMul.a;

                return col;
            }
            ENDCG
        }
    }
}
