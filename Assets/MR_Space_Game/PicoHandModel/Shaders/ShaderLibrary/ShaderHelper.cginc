#ifndef MYCGINC_TOOL
#define MYCGINC_TOOL



//绘制圆形区域面积并对圆形能点的到圆心的距离进行非线性插值
half drawCircle(half2 centerPos,half2 uv, float radius, float powerValue)
{
    float len = distance(centerPos, uv);
    half lerpValue = 1 - saturate(len / radius);
    lerpValue = pow(lerpValue, powerValue);
    return lerpValue;
}

//光照模型算法

//菲涅尔算法
half fresnel(half3 vDir,half3 nDir)
{
    half3 n1 = normalize(vDir);
    half3 n2 = normalize(nDir);
    return saturate(dot(n1, n2));
}

//镜面反射
half phongSpecular(half3 rDir,half3 vDir,half power)
{
    half rDotv = saturate(dot(rDir, vDir));
    return pow(rDotv, power);
}

//半兰伯特漫反射
half halfLambert(float3 lDir,float3 nDir)
{
    half v = dot(lDir, nDir) * 0.5 + 0.5;
    return v;
}

//颜色混合算法
//正片叠底
//fixed4 c = a * b 
float4 multiply(float4 a,float4 b)
{
    return a * b;
}

//混合变暗
//fixed4 c = min(a,b)
float4 mixDark(float4 a,float4 b)
{
    return min(a, b);
}

//混合变亮
//fixed4 c = max(a,b)
float4 mixLight(float4 a,float4 b)
{
    return max(a, b);
}

//正常透明混合
//fixed4 c = a*(1 - b.a)+b*b.a
float4 mixAlpha(float4 a,float4 b)
{
    return a * (1 - b.a) + b * b.a;
}

//滤色
//1-(1-a）*(1-b)
float3 colourFilter(float3 col1,float3 col2)
{
    return 1 - (1 - col1) * (1 - col2);
}

//叠加
//fixed4 ifflag = step(a,fixed4(0.5,0.5,0.5,0.5))
//fixed4 c = iflag * a*b*2+(1-ifflag)*(1-(1-a)*(1-b)*2);

//强光
//fixed4 ifflag = step(b,fixed4(0.5,0.5,0.5,0.5))
//iflag * a*b*2+(1-ifflag)*(1-(1-a)*(1-b)*2);


//柔光
//fixed4 ifflag = step(b,fixed4(0.5,0.5,0.5,0.5))
//fixed3 c = ifflag * (a*b*2+a*a*(1-b*2))+(1-ifflag)*(a*(1-b)*2)+sqrt(a)*(2*b-1)

#endif
