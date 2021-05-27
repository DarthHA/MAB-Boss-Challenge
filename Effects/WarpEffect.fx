sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float uOpacity;
float3 uSecondaryColor;
float uTime;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uImageOffset;
float uIntensity;
float uProgress;
float2 uDirection;
float2 uZoom;
float2 uImageSize0;
float2 uImageSize1;


/*
float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
	float dist = sqrt((coords.x - 0.5) * (coords.x - 0.5) + (coords.y - 0.5) * (coords.y - 0.5));
	if (dist > 0.5) dist = 0.5f;
	float k = dist * 2;
	k = sqrt(k);
	float2 NewCoords = float2(0.5 + (coords.x - 0.5) * (1 - uProgress * k), 0.5 + (coords.y - 0.5) * (1 - uProgress * k));
	return tex2D(uImage0, float2(NewCoords.x, NewCoords.y));
}
*/


float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
	float distX = abs(coords.x - 0.5) * 2;
	float distY = abs(coords.y - 0.5) * 2;
	float2 NewCoord1 = float2(0.5 + (coords.x - 0.5) * (1 - uProgress * distX), 0.5 + (coords.y - 0.5) * (1 - uProgress * distY));
	float2 NewCoord2 = float2(0.5 + (coords.x - 0.5) * (1 - uProgress * distX * 0.95), 0.5 + (coords.y - 0.5) * (1 - uProgress * distY * 0.95));
	float2 NewCoord3 = float2(0.5 + (coords.x - 0.5) * (1 - uProgress * distX * 0.9), 0.5 + (coords.y - 0.5) * (1 - uProgress * distY * 0.9));
	return (tex2D(uImage0, float2(NewCoord1.x, NewCoord1.y)) + tex2D(uImage0, float2(NewCoord2.x, NewCoord2.y)) + tex2D(uImage0, float2(NewCoord3.x, NewCoord3.y))) / 3;
}

technique Technique1
{
	pass WarpEffect
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}