sampler s0;
float4 PixelShaderFunction(float2 coords: TEXCOORD0) : COLOR0
{
	float4 color = tex2D(s0, coords);
	float avg = color[0] + color[1] + color[2];
	avg = avg / 3;
    return float4(avg, avg, avg, color[3]);
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}