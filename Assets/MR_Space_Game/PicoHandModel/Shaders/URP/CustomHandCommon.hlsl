#ifndef COSTOM_HAND_COMMON
#define COSTOM_HAND_COMMON

#include "../ShaderLibrary/ShaderHelper.cginc"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"


TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);

CBUFFER_START(UnityPerMaterial)
float4 _MainTex_ST;
float4 _MainColor;

//透明
float _AlphaEnd;
float _AlphaSize;
float _AlphaPower;

//高光
half4 _SpecularColor;
half _SpecularRange;
half _SpecularPower;

//菲涅尔
float _FresnelRange;
float _FresnelPower;

//辉光1
float4 _GlowColor;
float _GlowRange;

//辉光2
float4 _GlowColor1;
float _GlowRange1;
float _StepValue;

//网格
half4 _GridColor;
float _GridRange;
float _GridCount;

//spot
half4 _SpotLightColor;


//Thumb
float4 _ThumbPos;
float _ThumbRadius;

//Index
float4 _IndexPos;
float _IndexRadius;

CBUFFER_END


struct Attributes
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
    float4 normal:NORMAL;
};

struct Varyings
{
    float4 pos : SV_POSITION;
    float2 uv : TEXCOORD0;
    float3 wsPos:TEXCOORD1;
    float3 nDir:TEXCOORD2;
    float3 lsPos:TEXCOORD3;
};

Varyings vert(Attributes v)
{
    Varyings o = (Varyings)0;
    VertexPositionInputs vertexPosition = GetVertexPositionInputs(v.vertex.xyz);
    o.pos = vertexPosition.positionCS;
    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    o.wsPos = vertexPosition.positionWS;
    o.nDir = TransformObjectToWorldNormal(v.normal.xyz);
    o.lsPos = v.vertex.xyz;
    return o;
}

half4 frag(Varyings i) : SV_Target
{
    float start = _AlphaEnd - _AlphaSize;
    float cur = saturate(i.uv.y - start);
    float ratio = saturate(cur / max(0.0001, _AlphaSize));
    float alpha = smoothstep(0, 1, pow(ratio, _AlphaPower));

  
    float3 vDir = normalize(_WorldSpaceCameraPos.xyz - i.wsPos);
    float3 lDir = normalize(half3(1, 2, 1) - i.wsPos);
    float3 rDir = reflect(-vDir, i.nDir);

    //漫反射
    float diffuseRate = halfLambert(lDir, i.nDir);
    float4 diffuseColor = _MainColor * diffuseRate;

    //镜面反射
    float specularRate = phongSpecular(rDir, vDir, _SpecularPower);
    float3 specularColor = _SpecularColor.rgb * specularRate * _SpecularRange;

    //菲涅尔基底
    float fresnelRate = fresnel(vDir, i.nDir);

    //构造网格
    float3 rampPos = frac(i.lsPos * _GridCount) - 1;
    rampPos = smoothstep(abs(rampPos), 0.2, 0.75);
    float4 grid = (rampPos.x) * _GridColor;

    //菲涅尔网格层
    float gridFresnel = pow(1 - fresnelRate, _GridRange);
    half3 gridColor = gridFresnel * grid.rgb * grid.a;

    //菲涅尔发光层
    float glowFresnel = pow(1 - fresnelRate, _GlowRange);
    float3 glowColor = glowFresnel * _GlowColor.rgb * _GlowColor.a;

    //菲涅尔硬边
    float glowFresnel1 = pow(1 - fresnelRate, _GlowRange1);
    glowFresnel1 = step(1 - _StepValue, glowFresnel1);
    float3 glowColor1 = glowFresnel1 * _GlowColor1.rgb * _GlowColor1.a;

    //菲涅尔最后颜色输出层
    half3 fresnelFinal = gridColor + glowColor + glowColor1;

    //基本纹理采样
    half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv) * diffuseColor;
    col.rgb += fresnelFinal;
    col.rgb += specularColor;

    //点亮算法
    //大拇指
    _ThumbPos.xyz = mul((float3x3)GetWorldToObjectMatrix(), _ThumbPos.xyz);
    float thumbRatio = 1 - saturate((distance(_ThumbPos.xyz, i.lsPos.xyz) / _ThumbRadius));
    float3 thumbCol = smoothstep(0, 1, thumbRatio) * _SpotLightColor.rgb;

    //食指
    _IndexPos.xyz = mul((float3x3)GetWorldToObjectMatrix(), _IndexPos.xyz);
    float indexRatio = 1 - saturate((distance(_IndexPos.xyz, i.lsPos.xyz) / _IndexRadius));
    float3 indexCol = smoothstep(0, 1, indexRatio) * _SpotLightColor.rgb;

    col.rgb = colourFilter(col.rgb, thumbCol);
    col.rgb = colourFilter(col.rgb, indexCol);

    //颜色混合算法
    half4 finalColor = half4(col.rgb, alpha * _MainColor.a);
    return finalColor;
}

#endif
