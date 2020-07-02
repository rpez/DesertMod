sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;

float4 GlyphBurning(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (!any(color))
        return color;

    float2 targetCoords = (uTargetPosition - uScreenPosition) / uScreenResolution;
    float2 distVec = (coords - targetCoords) * float2 (uScreenResolution.x / uScreenResolution.y, 1);
    float distance = length(distVec);
    float multiplier = (1.0 - distance);
    if (multiplier < 0.0) multiplier = 0.0;

    float oe = 1.0 + multiplier * 20;
    return float4 (color.r * oe, color.g * oe, color.b * oe, color.a);
}

technique Technique1
{
    pass GlyphBurning
    {
        PixelShader = compile ps_2_0 GlyphBurning();
    }
}