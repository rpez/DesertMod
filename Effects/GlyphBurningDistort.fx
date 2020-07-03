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

float distort = 500;

float4 GlyphBurningDistort(float2 coords : TEXCOORD0) : COLOR0
{
    float2 targetCoords = (uTargetPosition - uScreenPosition) / uScreenResolution;
    float2 distVec = (coords - targetCoords) * float2 (uScreenResolution.x / uScreenResolution.y, 1);
    float distance = length(distVec);

    float multiplier = uIntensity - distance;
    if (multiplier < 0.0) multiplier = 0.0;

    float2 size = float2 (0.128, 0.128);

    float factor = sin(length(float2 (coords.x + uProgress, coords.y) - size) * distort)
        + sin(length(coords - size) * distort)
        + sin(length(float2 (coords.x, coords.y + uProgress) - size) * distort);
    coords.xy = coords.xy + ((factor / distort));

    return tex2D(uImage0, coords);
}

technique Technique1
{
    pass GlyphBurningDistort
    {
        PixelShader = compile ps_2_0 GlyphBurningDistort();
    }
}